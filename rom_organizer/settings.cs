using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace rom_organizer
{
    /// <summary>
    /// Manages application settings with automatic persistence
    /// </summary>
    public class SettingsManager
    {
        private static SettingsManager _instance;
        private static readonly object _lock = new object();
        private readonly string _settingsPath;
        private AppSettings _settings;

        private SettingsManager()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RomOrganizer"
            );

            // Ensure directory exists
            Directory.CreateDirectory(appDataPath);

            _settingsPath = Path.Combine(appDataPath, "settings.json");
            LoadSettings();
        }

        /// <summary>
        /// Gets the singleton instance of SettingsManager
        /// </summary>
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new SettingsManager();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets or sets the last selected ROM directory
        /// </summary>
        public string LastSelectedDirectory
        {
            get => _settings.LastSelectedDirectory ?? "";
            set
            {
                if (_settings.LastSelectedDirectory != value)
                {
                    _settings.LastSelectedDirectory = value;
                    SaveSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether recursive scanning is enabled
        /// </summary>
        public bool RecursiveScanning
        {
            get => _settings.RecursiveScanning;
            set
            {
                if (_settings.RecursiveScanning != value)
                {
                    _settings.RecursiveScanning = value;
                    SaveSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether metadata extraction is enabled
        /// </summary>
        public bool ExtractMetadata
        {
            get => _settings.ExtractMetadata;
            set
            {
                if (_settings.ExtractMetadata != value)
                {
                    _settings.ExtractMetadata = value;
                    SaveSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to use database storage
        /// </summary>
        public bool UseDatabaseStorage
        {
            get => _settings.UseDatabaseStorage;
            set
            {
                if (_settings.UseDatabaseStorage != value)
                {
                    _settings.UseDatabaseStorage = value;
                    SaveSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets the window size and position
        /// </summary>
        public WindowSettings WindowSettings
        {
            get => _settings.WindowSettings ?? new WindowSettings();
            set
            {
                _settings.WindowSettings = value;
                SaveSettings();
            }
        }

        /// <summary>
        /// Gets the full settings object (for advanced scenarios)
        /// </summary>
        public AppSettings Settings => _settings;

        /// <summary>
        /// Loads settings from disk
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    _settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    _settings = new AppSettings();
                }
            }
            catch (Exception ex)
            {
                // If settings file is corrupted, create new default settings
                _settings = new AppSettings();
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves settings to disk
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(_settings, options);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a directory is valid and exists
        /// </summary>
        public bool IsDirectoryValid()
        {
            return !string.IsNullOrEmpty(LastSelectedDirectory) &&
                   Directory.Exists(LastSelectedDirectory);
        }

        /// <summary>
        /// Resets all settings to defaults
        /// </summary>
        public void ResetToDefaults()
        {
            _settings = new AppSettings();
            SaveSettings();
        }

        /// <summary>
        /// Gets the settings file path for debugging
        /// </summary>
        public string GetSettingsPath() => _settingsPath;
    }

    /// <summary>
    /// Application settings data structure
    /// </summary>
    public class AppSettings
    {
        public string LastSelectedDirectory { get; set; } = "";
        public bool RecursiveScanning { get; set; } = true;
        public bool ExtractMetadata { get; set; } = true;
        public bool UseDatabaseStorage { get; set; } = true;
        public DateTime LastScanTime { get; set; } = DateTime.MinValue;
        public WindowSettings WindowSettings { get; set; } = new WindowSettings();
        public ScanStatistics LastScanStats { get; set; } = new ScanStatistics();
    }

    /// <summary>
    /// Window position and size settings
    /// </summary>
    public class WindowSettings
    {
        public int Width { get; set; } = 1200;
        public int Height { get; set; } = 800;
        public int X { get; set; } = -1; // -1 means center on screen
        public int Y { get; set; } = -1;
        public bool IsMaximized { get; set; } = false;
        public int SelectedTabIndex { get; set; } = 0;
    }

    /// <summary>
    /// Statistics from the last scan
    /// </summary>
    public class ScanStatistics
    {
        public int TotalRoms { get; set; } = 0;
        public int TotalConsoles { get; set; } = 0;
        public long TotalSizeBytes { get; set; } = 0;
        public int UniqueGenres { get; set; } = 0;
        public TimeSpan ScanDuration { get; set; } = TimeSpan.Zero;
    }
}