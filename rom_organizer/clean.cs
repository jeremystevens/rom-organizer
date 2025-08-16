using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace rom_organizer
{
    public class RomCleaner
    {
        private readonly RomDatabase _database;

        public RomCleaner(RomDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        #region Enums and Configuration Classes

        public enum NamingConvention
        {
            RemoveTags,
            StandardFormat,
            CustomFormat
        }

        public enum DuplicateDetectionMethod
        {
            FileHash,
            FileSizeAndName,
            NameSimilarity
        }

        public class RenameConfiguration
        {
            public NamingConvention Convention { get; set; } = NamingConvention.RemoveTags;
            public string CustomFormat { get; set; } = "{name} [{region}] ({year})";
            public bool PreviewOnly { get; set; } = false;
        }

        public class DuplicateConfiguration
        {
            public DuplicateDetectionMethod DetectionMethod { get; set; } = DuplicateDetectionMethod.FileHash;
            public bool KeepBestVersion { get; set; } = true;
            public bool MoveDuplicatesToFolder { get; set; } = true;
            public string DuplicatesFolderPath { get; set; } = "Duplicates";
            public bool PreviewOnly { get; set; } = false;
        }

        #endregion

        #region Result Classes

        public class RenameResult
        {
            public int TotalFilesProcessed { get; set; }
            public int FilesRenamed { get; set; }
            public int FilesSkipped { get; set; }
            public int Errors { get; set; }
            public List<RenameOperation> Operations { get; set; } = new List<RenameOperation>();
            public List<string> ErrorMessages { get; set; } = new List<string>();
            public TimeSpan Duration { get; set; }
        }

        public class RenameOperation
        {
            public string OriginalPath { get; set; }
            public string NewPath { get; set; }
            public string OriginalName { get; set; }
            public string NewName { get; set; }
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class DuplicateResult
        {
            public int TotalFilesScanned { get; set; }
            public int DuplicateGroupsFound { get; set; }
            public int DuplicateFilesFound { get; set; }
            public int FilesRemoved { get; set; }
            public int FilesMoved { get; set; }
            public long SpaceReclaimed { get; set; }
            public List<DuplicateGroup> DuplicateGroups { get; set; } = new List<DuplicateGroup>();
            public List<string> ErrorMessages { get; set; } = new List<string>();
            public TimeSpan Duration { get; set; }
        }

        public class DuplicateGroup
        {
            public string Identifier { get; set; } // Hash, size+name, etc.
            public List<DuplicateFile> Files { get; set; } = new List<DuplicateFile>();
            public DuplicateFile KeptFile { get; set; }
            public List<DuplicateFile> RemovedFiles { get; set; } = new List<DuplicateFile>();
        }

        public class DuplicateFile
        {
            public int RomId { get; set; }
            public string Path { get; set; }
            public string Name { get; set; }
            public long Size { get; set; }
            public string Hash { get; set; }
            public int QualityScore { get; set; } // For determining "best" version
            public bool IsNoIntro { get; set; }
            public bool IsRedump { get; set; }
            public bool HasRegionTags { get; set; }
        }

        #endregion

        #region Rename Functionality

        public async Task<RenameResult> RenameRomsAsync(RenameConfiguration config,
            IProgress<string> progress = null)
        {
            var startTime = DateTime.Now;
            var result = new RenameResult();

            try
            {
                progress?.Report("Starting ROM rename operation...");

                // Get all ROMs from database
                var roms = _database.GetAllRoms();
                result.TotalFilesProcessed = roms.Count;

                progress?.Report($"Processing {roms.Count} ROMs...");

                foreach (var rom in roms)
                {
                    try
                    {
                        if (!File.Exists(rom.Path))
                        {
                            result.FilesSkipped++;
                            continue;
                        }

                        var newName = GenerateNewName(rom, config);

                        if (newName == Path.GetFileNameWithoutExtension(rom.Name))
                        {
                            result.FilesSkipped++;
                            continue;
                        }

                        var operation = new RenameOperation
                        {
                            OriginalPath = rom.Path,
                            OriginalName = rom.Name,
                            NewName = newName + rom.Extension
                        };

                        var directory = Path.GetDirectoryName(rom.Path);
                        operation.NewPath = Path.Combine(directory, operation.NewName);

                        if (!config.PreviewOnly)
                        {
                            // Perform actual rename
                            File.Move(rom.Path, operation.NewPath);

                            // Update database with new path and name
                            await UpdateRomPathInDatabase(rom, operation.NewPath, operation.NewName);

                            operation.Success = true;
                            result.FilesRenamed++;
                        }
                        else
                        {
                            operation.Success = true;
                        }

                        result.Operations.Add(operation);
                        progress?.Report($"Processed: {rom.Name} -> {operation.NewName}");
                    }
                    catch (Exception ex)
                    {
                        result.Errors++;
                        result.ErrorMessages.Add($"Error renaming {rom.Name}: {ex.Message}");

                        result.Operations.Add(new RenameOperation
                        {
                            OriginalPath = rom.Path,
                            OriginalName = rom.Name,
                            Success = false,
                            ErrorMessage = ex.Message
                        });
                    }
                }

                result.Duration = DateTime.Now - startTime;
                progress?.Report($"Rename operation completed. {result.FilesRenamed} files renamed, {result.FilesSkipped} skipped, {result.Errors} errors.");

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Critical error during rename operation: {ex.Message}");
                result.Duration = DateTime.Now - startTime;
                return result;
            }
        }

        private string GenerateNewName(RomDatabase.RomRecord rom, RenameConfiguration config)
        {
            var baseName = Path.GetFileNameWithoutExtension(rom.Name);

            switch (config.Convention)
            {
                case NamingConvention.RemoveTags:
                    return RemoveRegionLanguageTags(baseName);

                case NamingConvention.StandardFormat:
                    return GenerateStandardFormat(rom);

                case NamingConvention.CustomFormat:
                    return GenerateCustomFormat(rom, config.CustomFormat);

                default:
                    return baseName;
            }
        }

        private string RemoveRegionLanguageTags(string name)
        {
            // Remove common ROM tags like (USA), [!], (E), etc.
            var patterns = new[]
            {
                @"\s*\([^)]*\)\s*", // Remove anything in parentheses
                @"\s*\[[^\]]*\]\s*", // Remove anything in square brackets
                @"\s*\{[^}]*\}\s*",  // Remove anything in curly braces
                @"\s+", // Replace multiple spaces with single space
            };

            var result = name;
            foreach (var pattern in patterns.Take(3)) // Don't apply space reduction yet
            {
                result = Regex.Replace(result, pattern, " ");
            }

            // Now clean up spaces
            result = Regex.Replace(result, @"\s+", " ").Trim();

            return result;
        }

        private string GenerateStandardFormat(RomDatabase.RomRecord rom)
        {
            var baseName = RemoveRegionLanguageTags(Path.GetFileNameWithoutExtension(rom.Name));

            // Try to extract region from original name
            var regionMatch = Regex.Match(rom.Name, @"\(([^)]*(?:USA|Europe|Japan|World)[^)]*)\)");
            var region = regionMatch.Success ? regionMatch.Groups[1].Value : "Unknown";

            return $"{baseName} ({region})";
        }

        private string GenerateCustomFormat(RomDatabase.RomRecord rom, string format)
        {
            var baseName = RemoveRegionLanguageTags(Path.GetFileNameWithoutExtension(rom.Name));

            // Extract region from original name
            var regionMatch = Regex.Match(rom.Name, @"\(([^)]*(?:USA|Europe|Japan|World)[^)]*)\)");
            var region = regionMatch.Success ? regionMatch.Groups[1].Value : "Unknown";

            var result = format
                .Replace("{name}", baseName)
                .Replace("{title}", rom.Title ?? baseName)
                .Replace("{region}", region)
                .Replace("{year}", rom.Year?.ToString() ?? "Unknown")
                .Replace("{manufacturer}", rom.Manufacturer ?? "Unknown")
                .Replace("{genre}", rom.PrimaryGenre ?? "Unknown")
                .Replace("{console}", rom.ConsoleName ?? "Unknown");

            return result;
        }

        private async Task UpdateRomPathInDatabase(RomDatabase.RomRecord originalRom, string newPath, string newName)
        {
            await Task.Run(() => {
                // Create a RomScanner.RomInfo object from the existing ROM data
                // This allows us to use the existing UpdateRom method without breaking functionality
                var romInfo = new RomScanner.RomInfo
                {
                    Name = newName,
                    Path = newPath,
                    Size = originalRom.Size,
                    Modified = originalRom.Modified,
                    Sha1 = originalRom.Sha1,
                    Ext = originalRom.Extension,
                    Console = originalRom.ConsoleName,
                    Title = originalRom.Title,
                    SortKey = originalRom.SortKey,
                    Initial = originalRom.Initial,
                    Genre = originalRom.Genre,
                    PrimaryGenre = originalRom.PrimaryGenre,
                    Year = originalRom.Year,
                    Manufacturer = originalRom.Manufacturer,
                    Players = originalRom.Players,
                    Story = originalRom.Story
                };

                // Use a temporary connection to update the ROM
                // We'll create a simple wrapper to use the existing UpdateRom method
                using (var connection = new System.Data.SQLite.SQLiteConnection(_database.GetConnectionString()))
                {
                    connection.Open();

                    // Get console ID for the update
                    var consoleId = GetConsoleIdFromDatabase(connection, originalRom.ConsoleName);

                    // Use reflection or create a simple update method that works with existing structure
                    UpdateRomInDatabase(connection, originalRom.Id, romInfo, consoleId);
                }
            });
        }

        private string GetDatabaseConnectionString()
        {
            // We need access to the connection string - this would require a small addition to RomDatabase
            // For now, we'll work around this limitation
            return _database.GetConnectionString();
        }

        private int GetConsoleIdFromDatabase(System.Data.SQLite.SQLiteConnection connection, string consoleName)
        {
            var sql = "SELECT id FROM consoles WHERE name = @name";
            using (var cmd = new System.Data.SQLite.SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@name", consoleName);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 1; // Default to console ID 1 if not found
            }
        }

        private void UpdateRomInDatabase(System.Data.SQLite.SQLiteConnection connection, int romId, RomScanner.RomInfo romInfo, int consoleId)
        {
            var sql = @"
                UPDATE roms SET 
                    name = @name, path = @path, size = @size, modified = @modified, sha1 = @sha1, ext = @ext, console_id = @consoleId,
                    title = @title, sort_key = @sortKey, initial = @initial, genre = @genre, primary_genre = @primaryGenre,
                    year = @year, manufacturer = @manufacturer, players = @players, story = @story
                WHERE id = @id";

            using (var cmd = new System.Data.SQLite.SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id", romId);
                cmd.Parameters.AddWithValue("@name", romInfo.Name);
                cmd.Parameters.AddWithValue("@path", romInfo.Path);
                cmd.Parameters.AddWithValue("@size", romInfo.Size);
                cmd.Parameters.AddWithValue("@modified", ((DateTimeOffset)romInfo.Modified).ToUnixTimeSeconds());
                cmd.Parameters.AddWithValue("@sha1", (object)romInfo.Sha1 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ext", romInfo.Ext);
                cmd.Parameters.AddWithValue("@consoleId", consoleId);
                cmd.Parameters.AddWithValue("@title", romInfo.Title ?? romInfo.Name);
                cmd.Parameters.AddWithValue("@sortKey", romInfo.SortKey ?? "");
                cmd.Parameters.AddWithValue("@initial", romInfo.Initial ?? "#");
                cmd.Parameters.AddWithValue("@genre", (object)romInfo.Genre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@primaryGenre", (object)romInfo.PrimaryGenre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@year", (object)romInfo.Year ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@manufacturer", (object)romInfo.Manufacturer ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@players", (object)romInfo.Players ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@story", (object)romInfo.Story ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Duplicate Detection and Removal

        public async Task<DuplicateResult> ScanForDuplicatesAsync(DuplicateConfiguration config,
            IProgress<string> progress = null)
        {
            var startTime = DateTime.Now;
            var result = new DuplicateResult();

            try
            {
                progress?.Report("Starting duplicate scan...");

                var roms = _database.GetAllRoms();
                result.TotalFilesScanned = roms.Count;

                progress?.Report($"Scanning {roms.Count} ROMs for duplicates...");

                var duplicateGroups = await FindDuplicateGroups(roms, config, progress);

                result.DuplicateGroups = duplicateGroups;
                result.DuplicateGroupsFound = duplicateGroups.Count;
                result.DuplicateFilesFound = duplicateGroups.Sum(g => g.Files.Count - 1); // Subtract 1 for the original

                result.Duration = DateTime.Now - startTime;
                progress?.Report($"Duplicate scan completed. Found {result.DuplicateGroupsFound} groups with {result.DuplicateFilesFound} duplicate files.");

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Error during duplicate scan: {ex.Message}");
                result.Duration = DateTime.Now - startTime;
                return result;
            }
        }

        public async Task<DuplicateResult> RemoveDuplicatesAsync(DuplicateConfiguration config,
            IProgress<string> progress = null)
        {
            var scanResult = await ScanForDuplicatesAsync(config, progress);

            if (!config.PreviewOnly && scanResult.DuplicateGroups.Any())
            {
                progress?.Report("Removing duplicates...");

                foreach (var group in scanResult.DuplicateGroups)
                {
                    ProcessDuplicateGroup(group, config, scanResult, progress);
                }
            }

            return scanResult;
        }

        private async Task<List<DuplicateGroup>> FindDuplicateGroups(List<RomDatabase.RomRecord> roms,
            DuplicateConfiguration config, IProgress<string> progress)
        {
            var groups = new Dictionary<string, DuplicateGroup>();

            foreach (var rom in roms)
            {
                if (!File.Exists(rom.Path))
                    continue;

                var identifier = await GenerateIdentifier(rom, config.DetectionMethod, progress);

                if (string.IsNullOrEmpty(identifier))
                    continue;

                if (!groups.ContainsKey(identifier))
                {
                    groups[identifier] = new DuplicateGroup { Identifier = identifier };
                }

                var duplicateFile = new DuplicateFile
                {
                    RomId = rom.Id,
                    Path = rom.Path,
                    Name = rom.Name,
                    Size = rom.Size,
                    Hash = rom.Sha1,
                    QualityScore = CalculateQualityScore(rom),
                    IsNoIntro = rom.Name.Contains("[!]") || rom.Name.ToLower().Contains("no-intro"),
                    IsRedump = rom.Name.ToLower().Contains("redump"),
                    HasRegionTags = Regex.IsMatch(rom.Name, @"\([^)]*(?:USA|Europe|Japan|World)[^)]*\)")
                };

                groups[identifier].Files.Add(duplicateFile);
            }

            // Return only groups with more than one file
            return groups.Values.Where(g => g.Files.Count > 1).ToList();
        }

        private async Task<string> GenerateIdentifier(RomDatabase.RomRecord rom,
            DuplicateDetectionMethod method, IProgress<string> progress)
        {
            switch (method)
            {
                case DuplicateDetectionMethod.FileHash:
                    return await GetFileHash(rom.Path, progress);

                case DuplicateDetectionMethod.FileSizeAndName:
                    var baseName = RemoveRegionLanguageTags(Path.GetFileNameWithoutExtension(rom.Name));
                    return $"{rom.Size}_{baseName.ToLowerInvariant()}";

                case DuplicateDetectionMethod.NameSimilarity:
                    return RemoveRegionLanguageTags(Path.GetFileNameWithoutExtension(rom.Name)).ToLowerInvariant();

                default:
                    return null;
            }
        }

        private async Task<string> GetFileHash(string filePath, IProgress<string> progress)
        {
            try
            {
                progress?.Report($"Calculating hash for: {Path.GetFileName(filePath)}");

                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = await Task.Run(() => md5.ComputeHash(stream));
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
            catch (Exception ex)
            {
                progress?.Report($"Error calculating hash for {filePath}: {ex.Message}");
                return null;
            }
        }

        private int CalculateQualityScore(RomDatabase.RomRecord rom)
        {
            var score = 0;
            var name = rom.Name.ToLower();

            // Higher score is better
            if (name.Contains("[!]")) score += 100; // Good dump
            if (name.Contains("no-intro")) score += 90;
            if (name.Contains("redump")) score += 90;
            if (name.Contains("(usa)")) score += 20;
            if (name.Contains("(world)")) score += 15;
            if (name.Contains("(europe)")) score += 10;
            if (name.Contains("(japan)")) score += 10;

            // Penalties
            if (name.Contains("[b]")) score -= 50; // Bad dump
            if (name.Contains("[h]")) score -= 30; // Hack
            if (name.Contains("[t]")) score -= 20; // Translation
            if (name.Contains("[o]")) score -= 10; // Overdump
            if (name.Contains("[a]")) score -= 5;  // Alternate

            return score;
        }

        private void ProcessDuplicateGroup(DuplicateGroup group, DuplicateConfiguration config,
            DuplicateResult result, IProgress<string> progress)
        {
            try
            {
                // Determine which file to keep
                var filesToKeep = config.KeepBestVersion
                    ? group.Files.OrderByDescending(f => f.QualityScore).Take(1)
                    : group.Files.Take(1);

                group.KeptFile = filesToKeep.First();
                group.RemovedFiles = group.Files.Except(filesToKeep).ToList();

                foreach (var fileToRemove in group.RemovedFiles)
                {
                    try
                    {
                        if (config.MoveDuplicatesToFolder)
                        {
                            MoveToDuplicatesFolder(fileToRemove, config.DuplicatesFolderPath);
                            result.FilesMoved++;
                        }
                        else
                        {
                            File.Delete(fileToRemove.Path);
                            result.FilesRemoved++;
                        }

                        result.SpaceReclaimed += fileToRemove.Size;
                        progress?.Report($"Processed duplicate: {fileToRemove.Name}");
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessages.Add($"Error processing {fileToRemove.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Error processing duplicate group: {ex.Message}");
            }
        }

        private void MoveToDuplicatesFolder(DuplicateFile file, string duplicatesFolderPath)
        {
            var sourceDir = Path.GetDirectoryName(file.Path);
            var duplicatesDir = Path.IsPathRooted(duplicatesFolderPath)
                ? duplicatesFolderPath
                : Path.Combine(sourceDir, duplicatesFolderPath);

            Directory.CreateDirectory(duplicatesDir);

            var destinationPath = Path.Combine(duplicatesDir, Path.GetFileName(file.Path));

            // Handle name conflicts
            var counter = 1;
            while (File.Exists(destinationPath))
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(file.Path);
                var extension = Path.GetExtension(file.Path);
                destinationPath = Path.Combine(duplicatesDir, $"{nameWithoutExt}_{counter}{extension}");
                counter++;
            }

            File.Move(file.Path, destinationPath);
        }

        #endregion

        #region Utility Methods

        public string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
        }

        public string GenerateRenameSummary(RenameResult result)
        {
            return $"Rename Summary:\n" +
                   $"Total files processed: {result.TotalFilesProcessed}\n" +
                   $"Files renamed: {result.FilesRenamed}\n" +
                   $"Files skipped: {result.FilesSkipped}\n" +
                   $"Errors: {result.Errors}\n" +
                   $"Duration: {result.Duration.TotalSeconds:F1} seconds";
        }

        public string GenerateDuplicateSummary(DuplicateResult result)
        {
            return $"Duplicate Summary:\n" +
                   $"Total files scanned: {result.TotalFilesScanned}\n" +
                   $"Duplicate groups found: {result.DuplicateGroupsFound}\n" +
                   $"Duplicate files found: {result.DuplicateFilesFound}\n" +
                   $"Files removed: {result.FilesRemoved}\n" +
                   $"Files moved: {result.FilesMoved}\n" +
                   $"Space reclaimed: {FormatFileSize(result.SpaceReclaimed)}\n" +
                   $"Duration: {result.Duration.TotalSeconds:F1} seconds";
        }

        #endregion
    }
}