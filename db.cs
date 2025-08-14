using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace rom_organizer
{
    public class RomDatabase
    {
        private readonly string _connectionString;
        private readonly string _dbPath;

        public RomDatabase(string databasePath = null)
        {
            _dbPath = databasePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rom_collection.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            // Create tables with proper relationships and constraints
            var createTablesSQL = @"
                -- Main ROMs table
                CREATE TABLE IF NOT EXISTS roms (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    path TEXT NOT NULL UNIQUE,
                    size INTEGER NOT NULL,
                    modified INTEGER NOT NULL,  -- Unix timestamp
                    sha1 TEXT,
                    ext TEXT NOT NULL,
                    console_id INTEGER NOT NULL,
                    
                    -- Enhanced metadata fields
                    title TEXT NOT NULL,
                    sort_key TEXT NOT NULL,
                    initial TEXT NOT NULL,
                    
                    -- Metadata from XML
                    genre TEXT,
                    primary_genre TEXT,
                    year INTEGER,
                    manufacturer TEXT,
                    players INTEGER,
                    story TEXT,
                    
                    -- Timestamps for data management
                    created_at INTEGER NOT NULL DEFAULT (strftime('%s', 'now')),
                    updated_at INTEGER NOT NULL DEFAULT (strftime('%s', 'now')),
                    
                    FOREIGN KEY (console_id) REFERENCES consoles (id)
                );

                -- Consoles lookup table
                CREATE TABLE IF NOT EXISTS consoles (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL UNIQUE,
                    short_name TEXT,
                    manufacturer TEXT,
                    release_year INTEGER,
                    created_at INTEGER NOT NULL DEFAULT (strftime('%s', 'now'))
                );

                -- Genres lookup table for normalization
                CREATE TABLE IF NOT EXISTS genres (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL UNIQUE,
                    created_at INTEGER NOT NULL DEFAULT (strftime('%s', 'now'))
                );

                -- ROM-Genre many-to-many relationship
                CREATE TABLE IF NOT EXISTS rom_genres (
                    rom_id INTEGER NOT NULL,
                    genre_id INTEGER NOT NULL,
                    PRIMARY KEY (rom_id, genre_id),
                    FOREIGN KEY (rom_id) REFERENCES roms (id) ON DELETE CASCADE,
                    FOREIGN KEY (genre_id) REFERENCES genres (id) ON DELETE CASCADE
                );

                -- Scan history for tracking changes
                CREATE TABLE IF NOT EXISTS scan_history (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    scan_path TEXT NOT NULL,
                    files_found INTEGER NOT NULL,
                    files_added INTEGER NOT NULL,
                    files_updated INTEGER NOT NULL,
                    files_removed INTEGER NOT NULL,
                    scan_duration INTEGER NOT NULL,  -- milliseconds
                    metadata_extracted BOOLEAN NOT NULL DEFAULT 0,
                    created_at INTEGER NOT NULL DEFAULT (strftime('%s', 'now'))
                );

                -- Indexes for performance
                CREATE INDEX IF NOT EXISTS idx_roms_console ON roms (console_id);
                CREATE INDEX IF NOT EXISTS idx_roms_title ON roms (title);
                CREATE INDEX IF NOT EXISTS idx_roms_sort_key ON roms (sort_key);
                CREATE INDEX IF NOT EXISTS idx_roms_initial ON roms (initial);
                CREATE INDEX IF NOT EXISTS idx_roms_primary_genre ON roms (primary_genre);
                CREATE INDEX IF NOT EXISTS idx_roms_year ON roms (year);
                CREATE INDEX IF NOT EXISTS idx_roms_path ON roms (path);
                CREATE INDEX IF NOT EXISTS idx_roms_sha1 ON roms (sha1);
                CREATE INDEX IF NOT EXISTS idx_roms_updated_at ON roms (updated_at);

                -- Triggers to maintain data consistency
                CREATE TRIGGER IF NOT EXISTS update_roms_timestamp 
                AFTER UPDATE ON roms
                BEGIN
                    UPDATE roms SET updated_at = strftime('%s', 'now') WHERE id = NEW.id;
                END;
            ";

            using var command = new SQLiteCommand(createTablesSQL, connection);
            command.ExecuteNonQuery();

            // Insert default console data
            InsertDefaultConsoles(connection);
        }

        private void InsertDefaultConsoles(SQLiteConnection connection)
        {
            var consoles = new Dictionary<string, (string shortName, string manufacturer, int? year)>
            {
                {"Nintendo Entertainment System", ("NES", "Nintendo", 1985)},
                {"Super Nintendo", ("SNES", "Nintendo", 1991)},
                {"Game Boy", ("GB", "Nintendo", 1989)},
                {"Game Boy Color", ("GBC", "Nintendo", 1998)},
                {"Game Boy Advance", ("GBA", "Nintendo", 2001)},
                {"Nintendo 64", ("N64", "Nintendo", 1996)},
                {"Sega Genesis", ("Genesis", "Sega", 1989)},
                {"Sega Mega Drive", ("MD", "Sega", 1988)},
                {"Disc Images", ("Disc", "Various", null)},
                {"Compressed Hunks of Data", ("CHD", "Various", null)},
                {"Archives", ("Archive", "Various", null)},
                {"Unknown", ("UNK", "Unknown", null)}
            };

            foreach (var console in consoles)
            {
                var sql = @"
                    INSERT OR IGNORE INTO consoles (name, short_name, manufacturer, release_year) 
                    VALUES (@name, @shortName, @manufacturer, @year)";

                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@name", console.Key);
                cmd.Parameters.AddWithValue("@shortName", console.Value.shortName);
                cmd.Parameters.AddWithValue("@manufacturer", console.Value.manufacturer);
                cmd.Parameters.AddWithValue("@year", (object)console.Value.year ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public class RomRecord
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public long Size { get; set; }
            public DateTime Modified { get; set; }
            public string Sha1 { get; set; }
            public string Extension { get; set; }
            public int ConsoleId { get; set; }
            public string ConsoleName { get; set; }
            public string Title { get; set; }
            public string SortKey { get; set; }
            public string Initial { get; set; }
            public string Genre { get; set; }
            public string PrimaryGenre { get; set; }
            public int? Year { get; set; }
            public string Manufacturer { get; set; }
            public int? Players { get; set; }
            public string Story { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class ScanResult
        {
            public int FilesFound { get; set; }
            public int FilesAdded { get; set; }
            public int FilesUpdated { get; set; }
            public int FilesRemoved { get; set; }
            public TimeSpan Duration { get; set; }
            public bool MetadataExtracted { get; set; }
        }

        // CRUD operations with transaction safety
        public ScanResult UpsertRoms(List<RomScanner.RomInfo> romInfos, string scanPath, bool removeOrphans = true)
        {
            var startTime = DateTime.Now;
            var result = new ScanResult { MetadataExtracted = true };

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var scannedPaths = new HashSet<string>();

                foreach (var romInfo in romInfos)
                {
                    scannedPaths.Add(romInfo.Path);
                    UpsertSingleRom(connection, romInfo, ref result);
                }

                // Remove orphaned entries if requested
                if (removeOrphans)
                {
                    result.FilesRemoved = RemoveOrphanedRoms(connection, scanPath, scannedPaths);
                }

                // Record scan history
                RecordScanHistory(connection, scanPath, result, startTime);

                transaction.Commit();
                result.Duration = DateTime.Now - startTime;
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void UpsertSingleRom(SQLiteConnection connection, RomScanner.RomInfo romInfo, ref ScanResult result)
        {
            // Get console ID
            int consoleId = GetOrCreateConsoleId(connection, romInfo.Console);

            // Check if ROM exists
            var existingId = GetRomIdByPath(connection, romInfo.Path);

            if (existingId.HasValue)
            {
                // Update existing ROM
                UpdateRom(connection, existingId.Value, romInfo, consoleId);
                result.FilesUpdated++;
            }
            else
            {
                // Insert new ROM
                InsertRom(connection, romInfo, consoleId);
                result.FilesAdded++;
            }

            result.FilesFound++;
        }

        private int GetOrCreateConsoleId(SQLiteConnection connection, string consoleName)
        {
            var sql = "SELECT id FROM consoles WHERE name = @name";
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@name", consoleName);

            var result = cmd.ExecuteScalar();
            if (result != null)
                return Convert.ToInt32(result);

            // Create new console
            var insertSql = "INSERT INTO consoles (name, short_name) VALUES (@name, @shortName); SELECT last_insert_rowid();";
            using var insertCmd = new SQLiteCommand(insertSql, connection);
            insertCmd.Parameters.AddWithValue("@name", consoleName);
            insertCmd.Parameters.AddWithValue("@shortName", consoleName.Substring(0, Math.Min(10, consoleName.Length)));

            return Convert.ToInt32(insertCmd.ExecuteScalar());
        }

        private int? GetRomIdByPath(SQLiteConnection connection, string path)
        {
            var sql = "SELECT id FROM roms WHERE path = @path";
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@path", path);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : null;
        }

        private void InsertRom(SQLiteConnection connection, RomScanner.RomInfo romInfo, int consoleId)
        {
            var sql = @"
                INSERT INTO roms (
                    name, path, size, modified, sha1, ext, console_id,
                    title, sort_key, initial, genre, primary_genre, 
                    year, manufacturer, players, story
                ) VALUES (
                    @name, @path, @size, @modified, @sha1, @ext, @consoleId,
                    @title, @sortKey, @initial, @genre, @primaryGenre,
                    @year, @manufacturer, @players, @story
                )";

            using var cmd = new SQLiteCommand(sql, connection);
            AddRomParameters(cmd, romInfo, consoleId);
            cmd.ExecuteNonQuery();
        }

        private void UpdateRom(SQLiteConnection connection, int romId, RomScanner.RomInfo romInfo, int consoleId)
        {
            var sql = @"
                UPDATE roms SET 
                    name = @name, size = @size, modified = @modified, sha1 = @sha1, ext = @ext, console_id = @consoleId,
                    title = @title, sort_key = @sortKey, initial = @initial, genre = @genre, primary_genre = @primaryGenre,
                    year = @year, manufacturer = @manufacturer, players = @players, story = @story
                WHERE id = @id";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", romId);
            AddRomParameters(cmd, romInfo, consoleId);
            cmd.ExecuteNonQuery();
        }

        private void AddRomParameters(SQLiteCommand cmd, RomScanner.RomInfo romInfo, int consoleId)
        {
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
        }

        private int RemoveOrphanedRoms(SQLiteConnection connection, string scanPath, HashSet<string> scannedPaths)
        {
            // Find ROMs that are in the database but weren't found in the scan
            var sql = "SELECT id, path FROM roms WHERE path LIKE @scanPath || '%'";
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@scanPath", scanPath);

            var toRemove = new List<int>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var path = reader.GetString("path");
                if (!scannedPaths.Contains(path) && !File.Exists(path))
                {
                    toRemove.Add(reader.GetInt32("id"));
                }
            }
            reader.Close();

            // Remove orphaned entries
            if (toRemove.Count > 0)
            {
                var deleteSql = $"DELETE FROM roms WHERE id IN ({string.Join(",", toRemove)})";
                using var deleteCmd = new SQLiteCommand(deleteSql, connection);
                deleteCmd.ExecuteNonQuery();
            }

            return toRemove.Count;
        }

        private void RecordScanHistory(SQLiteConnection connection, string scanPath, ScanResult result, DateTime startTime)
        {
            var sql = @"
                INSERT INTO scan_history (
                    scan_path, files_found, files_added, files_updated, files_removed,
                    scan_duration, metadata_extracted
                ) VALUES (
                    @scanPath, @filesFound, @filesAdded, @filesUpdated, @filesRemoved,
                    @duration, @metadataExtracted
                )";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@scanPath", scanPath);
            cmd.Parameters.AddWithValue("@filesFound", result.FilesFound);
            cmd.Parameters.AddWithValue("@filesAdded", result.FilesAdded);
            cmd.Parameters.AddWithValue("@filesUpdated", result.FilesUpdated);
            cmd.Parameters.AddWithValue("@filesRemoved", result.FilesRemoved);
            cmd.Parameters.AddWithValue("@duration", (int)(DateTime.Now - startTime).TotalMilliseconds);
            cmd.Parameters.AddWithValue("@metadataExtracted", result.MetadataExtracted);
            cmd.ExecuteNonQuery();
        }

        // Query methods for the UI
        public List<RomRecord> GetAllRoms(string orderBy = "title")
        {
            return QueryRoms($"ORDER BY {orderBy}");
        }

        public List<RomRecord> GetRomsByConsole(string consoleName, string orderBy = "title")
        {
            return QueryRoms("WHERE c.name = @console ORDER BY " + orderBy,
                             ("@console", consoleName));
        }

        public List<RomRecord> GetRomsByGenre(string genre, string orderBy = "title")
        {
            return QueryRoms("WHERE r.primary_genre = @genre ORDER BY " + orderBy,
                             ("@genre", genre));
        }

        public List<RomRecord> GetRomsByInitial(string initial, string orderBy = "title")
        {
            return QueryRoms("WHERE r.initial = @initial ORDER BY " + orderBy,
                             ("@initial", initial));
        }

        public List<RomRecord> SearchRoms(string searchTerm, string orderBy = "title")
        {
            var term = $"%{searchTerm}%";
            return QueryRoms(@"WHERE r.title LIKE @search OR r.name LIKE @search 
                              OR r.manufacturer LIKE @search ORDER BY " + orderBy,
                             ("@search", term));
        }

        private List<RomRecord> QueryRoms(string whereOrderClause, params (string key, object value)[] parameters)
        {
            var sql = $@"
                SELECT r.*, c.name as console_name
                FROM roms r
                JOIN consoles c ON r.console_id = c.id
                {whereOrderClause}";

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var cmd = new SQLiteCommand(sql, connection);

            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.key, param.value);
            }

            var results = new List<RomRecord>();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                results.Add(MapRomRecord(reader));
            }

            return results;
        }

        private RomRecord MapRomRecord(SQLiteDataReader reader)
        {
            return new RomRecord
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Path = reader.GetString("path"),
                Size = reader.GetInt64("size"),
                Modified = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64("modified")).DateTime,
                Sha1 = reader.IsDBNull("sha1") ? null : reader.GetString("sha1"),
                Extension = reader.GetString("ext"),
                ConsoleId = reader.GetInt32("console_id"),
                ConsoleName = reader.GetString("console_name"),
                Title = reader.GetString("title"),
                SortKey = reader.GetString("sort_key"),
                Initial = reader.GetString("initial"),
                Genre = reader.IsDBNull("genre") ? null : reader.GetString("genre"),
                PrimaryGenre = reader.IsDBNull("primary_genre") ? null : reader.GetString("primary_genre"),
                Year = reader.IsDBNull("year") ? null : reader.GetInt32("year"),
                Manufacturer = reader.IsDBNull("manufacturer") ? null : reader.GetString("manufacturer"),
                Players = reader.IsDBNull("players") ? null : reader.GetInt32("players"),
                Story = reader.IsDBNull("story") ? null : reader.GetString("story"),
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64("created_at")).DateTime,
                UpdatedAt = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64("updated_at")).DateTime
            };
        }

        // Statistics and summary methods
        public Dictionary<string, int> GetConsoleStats()
        {
            var sql = @"
                SELECT c.name, COUNT(r.id) as count
                FROM consoles c
                LEFT JOIN roms r ON c.id = r.console_id
                GROUP BY c.id, c.name
                HAVING count > 0
                ORDER BY count DESC";

            return ExecuteStatQuery(sql);
        }

        public Dictionary<string, int> GetGenreStats()
        {
            var sql = @"
                SELECT primary_genre as name, COUNT(*) as count
                FROM roms 
                WHERE primary_genre IS NOT NULL
                GROUP BY primary_genre
                ORDER BY count DESC";

            return ExecuteStatQuery(sql);
        }

        private Dictionary<string, int> ExecuteStatQuery(string sql)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var cmd = new SQLiteCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            var results = new Dictionary<string, int>();
            while (reader.Read())
            {
                results[reader.GetString("name")] = reader.GetInt32("count");
            }

            return results;
        }

        public void Vacuum()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var cmd = new SQLiteCommand("VACUUM;", connection);
            cmd.ExecuteNonQuery();
        }
    }
}