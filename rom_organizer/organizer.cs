using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace rom_organizer
{
    public class RomOrganizer
    {
        private readonly RomDatabase _database;

        // Progress callback matching RomScanner pattern
        public delegate void ProgressCallback(string message, int filesProcessed = 0);

        // Console mapping (matching your scan.cs mapping)
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

        public class OrganizeResult
        {
            public int TotalFiles { get; set; }
            public int FilesProcessed { get; set; }
            public int FilesSkipped { get; set; }
            public int FilesMoved { get; set; }
            public int FilesCopied { get; set; }
            public TimeSpan Duration { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
        }

        public RomOrganizer(RomDatabase database = null)
        {
            _database = database ?? new RomDatabase();
        }

        // ==================== ALPHABETICAL ORGANIZATION ====================
        public async Task<OrganizeResult> OrganizeAlphabeticalAsync(
            string outputPath,
            bool moveFiles,
            bool removeSpecialChars = true,
            int? maxFilenameLength = null,
            int maxFilesPerFolder = 1000,
            ProgressCallback progressCallback = null)
        {
            var startTime = DateTime.Now;
            var result = new OrganizeResult();

            try
            {
                progressCallback?.Invoke("Loading ROMs from database...");
                var roms = _database.GetAllRoms("title");
                result.TotalFiles = roms.Count;

                var folderCounts = new Dictionary<string, int>();
                var operations = new List<(RomDatabase.RomRecord rom, string destPath)>();

                // Phase 1: Plan all operations (fast - no file I/O)
                progressCallback?.Invoke("Planning file operations...");
                foreach (var rom in roms)
                {
                    // Quick existence check without FileInfo overhead
                    if (!File.Exists(rom.Path))
                    {
                        result.FilesSkipped++;
                        continue;
                    }

                    // CRITICAL FIX: Use actual filename with extension, not title
                    string cleanName = CleanFilename(rom.Name, removeSpecialChars, maxFilenameLength);
                    string folderName = GetFirstLetterFolder(cleanName);

                    // Check folder limits
                    int currentCount = folderCounts.ContainsKey(folderName) ? folderCounts[folderName] : 0;
                    if (currentCount >= maxFilesPerFolder)
                    {
                        result.FilesSkipped++;
                        continue;
                    }

                    // Build destination path
                    string destDir = Path.Combine(outputPath, folderName);
                    string destPath = Path.Combine(destDir, cleanName);

                    if (File.Exists(destPath))
                    {
                        result.FilesSkipped++;
                        continue;
                    }

                    operations.Add((rom, destPath));
                    folderCounts[folderName] = currentCount + 1;
                }

                // Phase 2: Execute operations with database batching
                await ExecuteOperationsBatch(operations, moveFiles, result, progressCallback);

                result.Duration = DateTime.Now - startTime;
                progressCallback?.Invoke($"Alphabetical organization complete: {result.FilesProcessed} processed", result.FilesProcessed);

                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Organization failed: {ex.Message}");
                result.Duration = DateTime.Now - startTime;
                return result;
            }
        }

        // ==================== CONSOLE ORGANIZATION ====================
        public async Task<OrganizeResult> OrganizeByConsoleAsync(
            string outputPath,
            bool moveFiles,
            bool removeSpecialChars = true,
            int? maxFilenameLength = null,
            ProgressCallback progressCallback = null)
        {
            var startTime = DateTime.Now;
            var result = new OrganizeResult();

            try
            {
                progressCallback?.Invoke("Loading ROMs from database...");
                var roms = _database.GetAllRoms("title");
                result.TotalFiles = roms.Count;

                var operations = new List<(RomDatabase.RomRecord rom, string destPath)>();
                string baseOutputPath = Path.Combine(outputPath, "By Console");

                // Phase 1: Plan operations
                progressCallback?.Invoke("Planning console organization...");
                foreach (var rom in roms)
                {
                    if (!File.Exists(rom.Path))
                    {
                        result.FilesSkipped++;
                        continue;
                    }

                    // Determine console folder and clean filename with extension
                    string consoleName = GetConsoleFromExtension(rom.Extension) ?? "Unknown";
                    string cleanName = CleanFilename(rom.Name, removeSpecialChars, maxFilenameLength);

                    string destDir = Path.Combine(baseOutputPath, consoleName);
                    string destPath = Path.Combine(destDir, cleanName);

                    if (File.Exists(destPath))
                    {
                        result.FilesSkipped++;
                        continue;
                    }

                    operations.Add((rom, destPath));
                }

                // Phase 2: Execute in batches
                await ExecuteOperationsBatch(operations, moveFiles, result, progressCallback);

                result.Duration = DateTime.Now - startTime;
                progressCallback?.Invoke($"Console organization complete: {result.FilesProcessed} processed", result.FilesProcessed);

                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Organization failed: {ex.Message}");
                result.Duration = DateTime.Now - startTime;
                return result;
            }
        }

        // ==================== GENRE ORGANIZATION ====================
        public async Task<OrganizeResult> OrganizeByGenreAsync(
            string outputPath,
            bool moveFiles,
            string xmlFilePath = null,
            bool removeSpecialChars = true,
            int? maxFilenameLength = null,
            ProgressCallback progressCallback = null)
        {
            var startTime = DateTime.Now;
            var result = new OrganizeResult();

            try
            {
                progressCallback?.Invoke("Loading ROMs from database...");
                var roms = _database.GetAllRoms("title");
                result.TotalFiles = roms.Count;

                // Smart XML file selection based on console types in the collection
                Dictionary<string, string> genreMap = null;
                if (string.IsNullOrEmpty(xmlFilePath))
                {
                    // Auto-detect appropriate XML files based on ROM extensions
                    xmlFilePath = FindBestXmlFile(roms, progressCallback);
                }

                if (!string.IsNullOrEmpty(xmlFilePath) && File.Exists(xmlFilePath))
                {
                    progressCallback?.Invoke($"Loading genre metadata from XML: {Path.GetFileName(xmlFilePath)}");
                    genreMap = LoadGenreMapping(xmlFilePath);
                }
                else
                {
                    progressCallback?.Invoke("No suitable XML metadata file found - using database genres only");
                }

                var operations = new List<(RomDatabase.RomRecord rom, string destPath)>();
                string baseOutputPath = Path.Combine(outputPath, "By Genre");

                // Phase 1: Plan operations
                progressCallback?.Invoke("Planning genre organization...");
                foreach (var rom in roms)
                {
                    if (!File.Exists(rom.Path))
                    {
                        result.FilesSkipped++;
                        continue;
                    }

                    // Determine genre and clean filename with extension
                    string genre = DetermineGenre(rom, genreMap);
                    string cleanName = CleanFilename(rom.Name, removeSpecialChars, maxFilenameLength);

                    string destDir = Path.Combine(baseOutputPath, genre);
                    string destPath = Path.Combine(destDir, cleanName);

                    if (File.Exists(destPath))
                    {
                        result.FilesSkipped++;
                        continue;
                    }

                    operations.Add((rom, destPath));
                }

                // Phase 2: Execute in batches
                await ExecuteOperationsBatch(operations, moveFiles, result, progressCallback);

                result.Duration = DateTime.Now - startTime;
                progressCallback?.Invoke($"Genre organization complete: {result.FilesProcessed} processed", result.FilesProcessed);

                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Organization failed: {ex.Message}");
                result.Duration = DateTime.Now - startTime;
                return result;
            }
        }

        private string FindBestXmlFile(List<RomDatabase.RomRecord> roms, ProgressCallback progressCallback)
        {
            try
            {
                string xmlDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xml files");
                if (!Directory.Exists(xmlDir))
                    return null;

                // Get all available XML files
                var xmlFiles = Directory.GetFiles(xmlDir, "*.xml");
                if (xmlFiles.Length == 0)
                    return null;

                // Count ROM extensions in the collection
                var extensionCounts = new Dictionary<string, int>();
                foreach (var rom in roms)
                {
                    string ext = rom.Extension?.ToLower() ?? "";
                    extensionCounts[ext] = (extensionCounts.ContainsKey(ext) ? extensionCounts[ext] : 0) + 1;
                }

                // Find the most common console type
                string primaryConsole = null;
                int maxCount = 0;
                foreach (var extCount in extensionCounts)
                {
                    if (extCount.Value > maxCount)
                    {
                        maxCount = extCount.Value;
                        if (ConsoleMap.TryGetValue(extCount.Key, out string consoleName))
                        {
                            primaryConsole = consoleName;
                        }
                    }
                }

                if (string.IsNullOrEmpty(primaryConsole))
                    return xmlFiles[0]; // Fallback to first XML file

                // Look for XML file matching the primary console
                string expectedXmlFile = Path.Combine(xmlDir, $"{primaryConsole}.xml");
                if (File.Exists(expectedXmlFile))
                {
                    progressCallback?.Invoke($"[INFO] Auto-selected XML for primary console: {primaryConsole}");
                    return expectedXmlFile;
                }

                // If exact match not found, look for partial matches
                foreach (var xmlFile in xmlFiles)
                {
                    string xmlName = Path.GetFileNameWithoutExtension(xmlFile);
                    if (xmlName.ToLower().Contains(primaryConsole.ToLower()) ||
                        primaryConsole.ToLower().Contains(xmlName.ToLower()))
                    {
                        progressCallback?.Invoke($"[INFO] Found partial match XML: {Path.GetFileName(xmlFile)}");
                        return xmlFile;
                    }
                }

                // Fallback to first available XML file
                progressCallback?.Invoke($"[INFO] Using fallback XML: {Path.GetFileName(xmlFiles[0])}");
                return xmlFiles[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        // ==================== OPTIMIZED HELPER METHODS ====================

        private async Task ExecuteOperationsBatch(List<(RomDatabase.RomRecord rom, string destPath)> operations, bool moveFiles, OrganizeResult result, ProgressCallback progressCallback)
        {
            progressCallback?.Invoke($"Executing {operations.Count} file operations...");
            int processed = 0;
            const int batchSize = 50;

            // Collect operations for database batching
            var moveUpdates = new Dictionary<int, string>();
            var copyInserts = new List<RomScanner.RomInfo>();

            for (int i = 0; i < operations.Count; i += batchSize)
            {
                var batch = operations.Skip(i).Take(batchSize).ToList();

                // Process file operations synchronously in batches
                foreach (var (rom, destPath) in batch)
                {
                    try
                    {
                        // Create directory
                        string destDir = Path.GetDirectoryName(destPath);
                        Directory.CreateDirectory(destDir);

                        if (moveFiles)
                        {
                            File.Move(rom.Path, destPath);
                            moveUpdates[rom.Id] = destPath; // Collect for batch database update
                            result.FilesMoved++;
                        }
                        else
                        {
                            File.Copy(rom.Path, destPath, overwrite: false);

                            // Prepare ROM info for batch database insert
                            var fileInfo = new FileInfo(destPath);
                            var romInfo = new RomScanner.RomInfo
                            {
                                Name = fileInfo.Name,
                                Path = destPath,
                                Size = fileInfo.Length,
                                Modified = fileInfo.LastWriteTime,
                                Ext = fileInfo.Extension.ToLower(),
                                Console = GetConsoleFromExtension(fileInfo.Extension) ?? "Unknown",
                                Title = rom.Title,
                                Genre = rom.Genre,
                                PrimaryGenre = rom.PrimaryGenre,
                                Year = rom.Year,
                                Manufacturer = rom.Manufacturer,
                                Players = rom.Players,
                                Story = rom.Story,
                                SortKey = rom.SortKey,
                                Initial = rom.Initial
                            };
                            copyInserts.Add(romInfo);
                            result.FilesCopied++;
                        }

                        processed++;
                    }
                    catch (Exception ex)
                    {
                        result.FilesSkipped++;
                        result.Errors.Add($"Failed to process {rom.Name}: {ex.Message}");
                        processed++;
                    }
                }

                result.FilesProcessed = processed;
                progressCallback?.Invoke($"Processed {processed}/{operations.Count} files", processed);

                // Batch database operations every few batches to reduce lock contention
                if (i % (batchSize * 5) == 0 || i + batchSize >= operations.Count)
                {
                    await ProcessDatabaseBatch(moveUpdates, copyInserts);
                    moveUpdates.Clear();
                    copyInserts.Clear();
                }
            }

            // Final database batch if any remaining
            if (moveUpdates.Count > 0 || copyInserts.Count > 0)
            {
                await ProcessDatabaseBatch(moveUpdates, copyInserts);
            }
        }

        private async Task ProcessDatabaseBatch(Dictionary<int, string> moveUpdates, List<RomScanner.RomInfo> copyInserts)
        {
            try
            {
                // Process database operations in a single task to avoid concurrent access
                await Task.Run(() =>
                {
                    // Batch update moved files
                    if (moveUpdates.Count > 0)
                    {
                        _database.UpdateRomPaths(moveUpdates);
                    }

                    // Batch insert copied files
                    if (copyInserts.Count > 0)
                    {
                        _database.UpsertRoms(copyInserts, Path.GetDirectoryName(copyInserts[0].Path), false);
                    }
                });
            }
            catch (Exception)
            {
                // Database errors are logged but don't stop file operations
            }
        }

        private async Task<bool> ProcessFileOperationFast(RomDatabase.RomRecord rom, string destinationPath, bool moveFile)
        {
            try
            {
                // Create directory only once per batch - faster than checking each time
                string destDir = Path.GetDirectoryName(destinationPath);
                Directory.CreateDirectory(destDir);

                if (moveFile)
                {
                    // Move file - no Task.Run wrapper for better performance
                    File.Move(rom.Path, destinationPath);

                    // Update database synchronously to avoid locking issues
                    _database.UpdateRomPath(rom.Id, destinationPath);
                }
                else
                {
                    // Copy file - no Task.Run wrapper for better performance
                    File.Copy(rom.Path, destinationPath, overwrite: false);

                    // Database operations will be batched later to avoid locking
                    // Return info for batch processing
                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void InsertCopiedRom(RomDatabase.RomRecord originalRom, string newPath)
        {
            var fileInfo = new FileInfo(newPath);

            // Create new ROM info for the copied file
            var romInfo = new RomScanner.RomInfo
            {
                Name = fileInfo.Name,
                Path = newPath,
                Size = fileInfo.Length,
                Modified = fileInfo.LastWriteTime,
                Ext = fileInfo.Extension.ToLower(),
                Console = GetConsoleFromExtension(fileInfo.Extension) ?? "Unknown",
                Title = originalRom.Title,
                Genre = originalRom.Genre,
                PrimaryGenre = originalRom.PrimaryGenre,
                Year = originalRom.Year,
                Manufacturer = originalRom.Manufacturer,
                Players = originalRom.Players,
                Story = originalRom.Story,
                SortKey = originalRom.SortKey,
                Initial = originalRom.Initial
            };

            // Insert the copied ROM as a new record
            _database.UpsertRoms(new List<RomScanner.RomInfo> { romInfo }, Path.GetDirectoryName(newPath), false);
        }

        private static readonly Regex InvalidCharsRegex = new Regex(@"[^\w\s\-_\.]", RegexOptions.Compiled);
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

        private string CleanFilename(string filename, bool removeSpecialChars, int? maxLength)
        {
            if (string.IsNullOrEmpty(filename))
                return "unknown";

            // CRITICAL FIX: Preserve the original extension
            string originalExtension = Path.GetExtension(filename);
            string nameWithoutExt = Path.GetFileNameWithoutExtension(filename);

            if (removeSpecialChars)
            {
                // Only clean the name part, preserve extension
                nameWithoutExt = InvalidCharsRegex.Replace(nameWithoutExt, "");
                nameWithoutExt = WhitespaceRegex.Replace(nameWithoutExt, " ").Trim();
            }

            if (maxLength.HasValue && nameWithoutExt.Length > maxLength.Value)
            {
                int maxNameLength = maxLength.Value - originalExtension.Length;
                if (maxNameLength > 0)
                {
                    nameWithoutExt = nameWithoutExt.Substring(0, Math.Min(nameWithoutExt.Length, maxNameLength));
                }
            }

            // Always rebuild with original extension - THIS FIXES THE EXTENSION BUG
            return nameWithoutExt + originalExtension;
        }

        private string GetFirstLetterFolder(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return "_";

            char firstChar = filename.ToUpper()[0];
            return char.IsLetter(firstChar) ? firstChar.ToString() : "_";
        }

        private string GetConsoleFromExtension(string extension)
        {
            return ConsoleMap.TryGetValue(extension?.ToLower() ?? "", out string console) ? console : "Unknown";
        }

        private Dictionary<string, string> LoadGenreMapping(string xmlFilePath)
        {
            var genreMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var doc = XDocument.Load(xmlFilePath);
                var games = doc.Descendants("game");

                foreach (var game in games)
                {
                    var nameAttr = game.Attribute("name")?.Value;
                    var genreElement = game.Element("genre")?.Value;

                    if (!string.IsNullOrEmpty(nameAttr) && !string.IsNullOrEmpty(genreElement))
                    {
                        // Take the first genre if multiple are separated by '/'
                        string primaryGenre = genreElement.Split('/')[0].Trim();

                        // Create variations of the name for better matching
                        var variations = CreateNameVariations(nameAttr);
                        foreach (var variation in variations)
                        {
                            genreMap[variation] = primaryGenre;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // If XML parsing fails, return empty map
            }

            return genreMap;
        }

        private List<string> CreateNameVariations(string name)
        {
            var variations = new List<string> { name };

            // Remove parentheses and brackets content
            var withoutParens = Regex.Replace(name, @"\([^)]*\)", "").Trim();
            var withoutBrackets = Regex.Replace(withoutParens, @"\[[^\]]*\]", "").Trim();

            if (!string.IsNullOrEmpty(withoutBrackets))
                variations.Add(withoutBrackets);

            // Handle "The" prefix
            if (name.ToLower().StartsWith("the "))
            {
                variations.Add(name.Substring(4).Trim());
            }

            return variations.Distinct().ToList();
        }

        private string DetermineGenre(RomDatabase.RomRecord rom, Dictionary<string, string> genreMap)
        {
            // First, check if we already have genre info in the database
            if (!string.IsNullOrEmpty(rom.PrimaryGenre))
                return SanitizeFolderName(rom.PrimaryGenre);

            // If XML mapping is available, try to find genre
            if (genreMap != null)
            {
                var title = rom.Title ?? rom.Name;
                var variations = CreateNameVariations(title);

                foreach (var variation in variations)
                {
                    if (genreMap.TryGetValue(variation, out string genre))
                    {
                        return SanitizeFolderName(genre);
                    }
                }
            }

            return "Unknown";
        }

        private string SanitizeFolderName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                return "Unknown";

            // Remove invalid folder characters
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                folderName = folderName.Replace(c, '_');
            }

            return folderName.Trim();
        }
    }
}