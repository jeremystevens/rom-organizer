using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace rom_organizer
{
    public class RomScanner
    {
        // Supported ROM extensions
        private static readonly HashSet<string> SupportedExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".nes",".sfc",".smc",".gba",".gb",".gbc",".n64",
            ".gen",".bin",".md",".iso",".cue",".chd",".zip",".7z"
        };

        // Console map based on file extension
        private static readonly Dictionary<string, string> ConsoleMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {".nes", "Nintendo Entertainment System"},
            {".sfc", "Super Nintendo"},
            {".smc", "Super Nintendo"},
            {".gba", "Game Boy Advance"},
            {".gb",  "Game Boy"},
            {".gbc", "Game Boy Color"},
            {".n64", "Nintendo 64"},
            {".gen", "Sega Genesis"},
            {".bin", "Sega Genesis"},
            {".md",  "Sega Mega Drive"},
            {".iso", "Disc Images"},
            {".cue", "Disc Images"},
            {".chd", "Compressed Hunks of Data"},
            {".zip", "Archives"},
            {".7z",  "Archives"}
        };

        // Enhanced ROM info with metadata
        public class RomInfo
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public long Size { get; set; }
            public DateTime Modified { get; set; }
            public string Sha1 { get; set; }
            public string Ext { get; set; }
            public string Console { get; set; }

            // Enhanced metadata from XML
            public string Title { get; set; }
            public string Genre { get; set; }
            public string PrimaryGenre { get; set; }
            public int? Year { get; set; }
            public string Manufacturer { get; set; }
            public int? Players { get; set; }
            public string Story { get; set; }
            public string SortKey { get; set; }
            public string Initial { get; set; }
        }

        // Progress callback delegate
        public delegate void ProgressCallback(string message, int filesProcessed = 0);

        // XML metadata cache
        private static readonly Dictionary<string, Dictionary<string, XmlGameInfo>> XmlCache =
            new Dictionary<string, Dictionary<string, XmlGameInfo>>();

        private class XmlGameInfo
        {
            public string Title { get; set; }
            public string Genre { get; set; }
            public int? Year { get; set; }
            public string Manufacturer { get; set; }
            public int? Players { get; set; }
            public string Story { get; set; }
        }

        public static RomDatabase.ScanResult ScanDirectoryToDatabase(string dirPath, bool recursive = true,
            bool extractMetadata = true, ProgressCallback progressCallback = null)
        {
            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException($"Directory not found: {dirPath}");

            progressCallback?.Invoke("Initializing database...");
            var database = new RomDatabase();

            progressCallback?.Invoke("Scanning directory structure...");

            // Get XML files directory (project folder/xml files)
            string xmlDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xml files");
            if (extractMetadata && Directory.Exists(xmlDir))
            {
                progressCallback?.Invoke("Loading XML metadata files...");
                LoadXmlMetadata(xmlDir);
            }

            // Snapshot files first
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.EnumerateFiles(dirPath, "*.*", searchOption)
                                 .Where(f => SupportedExts.Contains(Path.GetExtension(f)))
                                 .ToArray();

            progressCallback?.Invoke($"Found {files.Length} ROM files to process");

            var results = new ConcurrentBag<RomInfo>();
            int processedCount = 0;

            // Process in parallel
            Parallel.ForEach(
                files,
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                file =>
                {
                    try
                    {
                        var fi = new FileInfo(file);
                        string ext = fi.Extension;
                        string console = ConsoleMap.TryGetValue(ext, out var consoleName) ? consoleName : "Unknown";

                        // Create base ROM info
                        var romInfo = new RomInfo
                        {
                            Name = fi.Name,
                            Path = fi.FullName,
                            Size = fi.Length,
                            Modified = fi.LastWriteTime,
                            Sha1 = ComputeSHA1(fi.FullName),
                            Ext = ext,
                            Console = console
                        };

                        // Add enhanced metadata
                        EnhanceWithMetadata(romInfo, extractMetadata);

                        results.Add(romInfo);

                        // Progress update (thread-safe)
                        int current = Interlocked.Increment(ref processedCount);
                        if (current % 50 == 0 || current == files.Length)
                        {
                            progressCallback?.Invoke($"Processing: {romInfo.Title ?? romInfo.Name}", current);
                        }
                    }
                    catch (Exception ex)
                    {
                        progressCallback?.Invoke($"Error reading {file}: {ex.Message}");
                    }
                });

            var romList = results.ToList();

            progressCallback?.Invoke("Saving to database...");

            // Save to database instead of JSON
            var scanResult = database.UpsertRoms(romList, dirPath, removeOrphans: true);

            progressCallback?.Invoke($"Database updated: {scanResult.FilesAdded} added, {scanResult.FilesUpdated} updated, {scanResult.FilesRemoved} removed");

            return scanResult;
        }

        // Keep the original method for backward compatibility, but mark it as deprecated
        [Obsolete("Use ScanDirectoryToDatabase instead for better performance with large collections")]
        public static List<RomInfo> ScanDirectory(string dirPath, bool recursive = true,
            bool extractMetadata = true, ProgressCallback progressCallback = null)
        {
            // This now just calls the database version and returns empty list
            // since the data is in the database
            ScanDirectoryToDatabase(dirPath, recursive, extractMetadata, progressCallback);
            return new List<RomInfo>(); // Return empty - data is in database
        }

        private static void LoadXmlMetadata(string xmlDir)
        {
            var xmlFiles = Directory.GetFiles(xmlDir, "*.xml");

            foreach (var xmlFile in xmlFiles)
            {
                try
                {
                    string consoleName = Path.GetFileNameWithoutExtension(xmlFile);
                    var gameMap = LoadConsoleXmlMap(xmlFile);
                    XmlCache[consoleName.ToLower()] = gameMap;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading XML {xmlFile}: {ex.Message}");
                }
            }
        }

        private static Dictionary<string, XmlGameInfo> LoadConsoleXmlMap(string xmlPath)
        {
            var mapping = new Dictionary<string, XmlGameInfo>();

            try
            {
                var doc = XDocument.Load(xmlPath);
                var games = doc.Descendants("game");

                foreach (var game in games)
                {
                    var gameName = game.Attribute("name")?.Value?.Trim();

                    var title = GetFirstText(game, "title", "gametitle") ?? gameName;
                    var genre = GetFirstText(game, "genre", "genres");
                    var yearStr = GetFirstText(game, "year", "releasedate", "release_year");
                    var manufacturer = GetFirstText(game, "manufacturer", "publisher", "developer", "company");
                    var playersStr = GetFirstText(game, "players", "player", "numplayers");
                    var story = GetFirstText(game, "story", "description", "desc", "overview", "long_description", "storyline");

                    var gameInfo = new XmlGameInfo
                    {
                        Title = title,
                        Genre = genre,
                        Year = TryParseInt(yearStr),
                        Manufacturer = manufacturer,
                        Players = TryParseInt(playersStr),
                        Story = story
                    };

                    if (!string.IsNullOrEmpty(gameName))
                    {
                        mapping[MakeNormalizedKey(gameName)] = gameInfo;

                        // Also add by title if different
                        if (!string.IsNullOrEmpty(title) && !title.Equals(gameName, StringComparison.OrdinalIgnoreCase))
                        {
                            mapping[MakeNormalizedKey(title)] = gameInfo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing XML: {ex.Message}");
            }

            return mapping;
        }

        private static string GetFirstText(XElement element, params string[] names)
        {
            foreach (var name in names)
            {
                var value = element.Element(name)?.Value?.Trim();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return null;
        }

        private static int? TryParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var match = Regex.Match(value, @"-?\d+");
            if (match.Success && int.TryParse(match.Value, out int result))
                return result;
            else
                return null;
        }

        private static void EnhanceWithMetadata(RomInfo romInfo, bool extractMetadata)
        {
            // Clean filename to get title
            string cleanTitle = CleanFilenameToTitle(Path.GetFileNameWithoutExtension(romInfo.Name));
            romInfo.Title = cleanTitle;
            romInfo.SortKey = MakeSortKey(cleanTitle);
            romInfo.Initial = MakeInitial(cleanTitle);

            if (!extractMetadata)
                return;

            // Try to get metadata from XML
            var xmlMeta = LookupFromXml(romInfo.Console, cleanTitle);
            if (xmlMeta != null)
            {
                romInfo.Title = xmlMeta.Title ?? cleanTitle;
                romInfo.Genre = xmlMeta.Genre;
                romInfo.PrimaryGenre = GetPrimaryGenre(xmlMeta.Genre);
                romInfo.Year = xmlMeta.Year;
                romInfo.Manufacturer = xmlMeta.Manufacturer;
                romInfo.Players = xmlMeta.Players;
                romInfo.Story = xmlMeta.Story;

                // Update sort key with proper title
                romInfo.SortKey = MakeSortKey(romInfo.Title);
                romInfo.Initial = MakeInitial(romInfo.Title);
            }
        }

        private static XmlGameInfo LookupFromXml(string consoleName, string fallbackTitle)
        {
            if (string.IsNullOrEmpty(consoleName))
                return null;

            string consoleKey = consoleName.ToLower();

            // Try exact match first
            if (XmlCache.TryGetValue(consoleKey, out var gameMap))
            {
                string lookupKey = MakeNormalizedKey(fallbackTitle);
                if (gameMap.TryGetValue(lookupKey, out var gameInfo))
                    return gameInfo;
            }

            // Try partial console name matches
            foreach (var kvp in XmlCache)
            {
                if (consoleKey.Contains(kvp.Key) || kvp.Key.Contains(consoleKey))
                {
                    string lookupKey = MakeNormalizedKey(fallbackTitle);
                    if (kvp.Value.TryGetValue(lookupKey, out var gameInfo))
                        return gameInfo;
                }
            }

            return null;
        }

        // Text processing helpers
        private static readonly Regex RegionPatterns = new Regex(@"\([^)]*\)|\[[^\]]*\]", RegexOptions.IgnoreCase);
        private static readonly Regex SeparatorClean = new Regex(@"[_\.]+");
        private static readonly Regex AlnumOnly = new Regex(@"[^a-z0-9]");

        private static string CleanFilenameToTitle(string stem)
        {
            string s = stem;

            // Remove region info like (USA), [!], etc.
            s = RegionPatterns.Replace(s, "");

            // Replace separators with spaces
            s = SeparatorClean.Replace(s, " ");

            // Clean up whitespace
            s = Regex.Replace(s, @"\s+", " ").Trim();

            // Handle "Game, The" -> "The Game"
            if (s.ToLower().EndsWith(", the"))
            {
                s = "The " + s.Substring(0, s.Length - 5).Trim();
            }

            return s;
        }

        private static string MakeSortKey(string title)
        {
            return AlnumOnly.Replace(title.ToLower(), "");
        }

        private static string MakeNormalizedKey(string name)
        {
            return MakeSortKey(CleanFilenameToTitle(name));
        }

        private static string MakeInitial(string title)
        {
            foreach (char ch in title)
            {
                if (char.IsLetter(ch))
                    return ch.ToString().ToUpper();
            }
            return "#";
        }

        private static string GetPrimaryGenre(string genreValue)
        {
            if (string.IsNullOrEmpty(genreValue))
                return null;

            string[] parts = genreValue.Split('/');
            return parts.Length > 0 ? parts[0].Trim() : null;
        }

        private static string ComputeSHA1(string filePath)
        {
            using (var sha1 = SHA1.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = sha1.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}