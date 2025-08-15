using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using RetroArcadeUI;
using rom_organizer;

namespace rom_organizer
{
    public partial class Form1 : Form
    {
        private Timer scanTimer;
        private bool isScanning = false;
        private SettingsManager settings;

        public Form1()
        {
            InitializeComponent();
            settings = SettingsManager.Instance;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Custom console is already initialized with proper styling
            LoadUserSettings();
            InitializeUI();
        }

        private void LoadUserSettings()
        {
            // Restore directory selection
            if (settings.IsDirectoryValid())
            {
                directoryTextBox.Text = settings.LastSelectedDirectory;
            }

            // Restore scan preferences
            recursiveCheckBox.Checked = settings.RecursiveScanning;
            metadataCheckBox.Checked = settings.ExtractMetadata;

            // Restore window settings
            var windowSettings = settings.WindowSettings;
            if (windowSettings.X > 0 && windowSettings.Y > 0)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(windowSettings.X, windowSettings.Y);
            }

            this.Size = new Size(windowSettings.Width, windowSettings.Height);

            if (windowSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void SaveUserSettings()
        {
            // Save current directory
            if (!string.IsNullOrEmpty(directoryTextBox.Text) && directoryTextBox.Text != "Choose a folder...")
            {
                settings.LastSelectedDirectory = directoryTextBox.Text;
            }

            // Save scan preferences
            settings.RecursiveScanning = recursiveCheckBox.Checked;
            settings.ExtractMetadata = metadataCheckBox.Checked;

            // Save window settings
            var windowSettings = new WindowSettings();

            if (this.WindowState == FormWindowState.Normal)
            {
                windowSettings.Width = this.Width;
                windowSettings.Height = this.Height;
                windowSettings.X = this.Location.X;
                windowSettings.Y = this.Location.Y;
                windowSettings.IsMaximized = false;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                windowSettings.IsMaximized = true;
                // Keep the restored bounds for when window is restored
                windowSettings.Width = this.RestoreBounds.Width;
                windowSettings.Height = this.RestoreBounds.Height;
                windowSettings.X = this.RestoreBounds.X;
                windowSettings.Y = this.RestoreBounds.Y;
            }

            settings.WindowSettings = windowSettings;
        }

        private void InitializeUI()
        {
            // Initialize console with welcome message
            consoleOutput.ClearText();
            consoleOutput.AddText("🖥️ ROM Scanner Console v2.0", Color.FromArgb(100, 200, 255));
            consoleOutput.AddText("", Color.White);
            AppendConsoleText("[SYSTEM] ROM Scanner initialized and ready", Color.LimeGreen);

            // Show loaded directory if available
            if (settings.IsDirectoryValid())
            {
                AppendConsoleText($"[SETTINGS] Loaded directory: {settings.LastSelectedDirectory}", Color.FromArgb(100, 200, 255));
                AppendConsoleText("[INFO] Click 'Start Scan' to scan the loaded directory", Color.FromArgb(150, 150, 150));
            }
            else
            {
                AppendConsoleText("[INFO] Select a directory and click 'Start Scan' to begin", Color.FromArgb(150, 150, 150));
            }

            AppendConsoleText("", Color.White);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select ROM Directory";
                dialog.ShowNewFolderButton = true;

                // Start from the last selected directory if available
                if (settings.IsDirectoryValid())
                {
                    dialog.SelectedPath = settings.LastSelectedDirectory;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    directoryTextBox.Text = dialog.SelectedPath;
                    settings.LastSelectedDirectory = dialog.SelectedPath; // Save immediately
                    AppendConsoleText($"[INFO] Directory selected and saved: {dialog.SelectedPath}", Color.FromArgb(100, 200, 255));
                }
            }
        }

        private async void startScanButton_Click(object sender, EventArgs e)
        {
            if (isScanning)
            {
                AppendConsoleText("[WARNING] Scan already in progress!", Color.Orange);
                return;
            }

            if (directoryTextBox.Text == "Choose a folder..." || string.IsNullOrEmpty(directoryTextBox.Text))
            {
                AppendConsoleText("[ERROR] Please select a ROM directory first!", Color.Red);
                return;
            }

            if (!Directory.Exists(directoryTextBox.Text))
            {
                AppendConsoleText("[ERROR] Selected directory does not exist!", Color.Red);
                return;
            }

            // Choose scanning method based on user preference
            if (settings.UseDatabaseStorage)
            {
                await StartDatabaseScan();
            }
            else
            {
                await StartMemoryScan();
            }
        }

        private async Task StartDatabaseScan()
        {
            isScanning = true;

            // Update UI
            scanProgress.Visible = true;
            scanProgress.Value = 0;
            startScanButton.Text = "Scanning to Database...";
            startScanButton.Enabled = false;
            browseButton.Enabled = false;

            // Clear console and show scan start
            consoleOutput.ClearText();
            consoleOutput.AddText("🖥️ ROM Scanner Console - Database Mode", Color.FromArgb(100, 200, 255));
            consoleOutput.AddText("", Color.White);

            AppendConsoleText("[SCAN] Starting enhanced ROM scan with database storage...", Color.Yellow);
            AppendConsoleText($"[SCAN] Directory: {directoryTextBox.Text}", Color.FromArgb(100, 200, 255));
            AppendConsoleText($"[SCAN] Recursive: {recursiveCheckBox.Checked}", Color.FromArgb(100, 200, 255));
            AppendConsoleText($"[SCAN] Extract Metadata: {metadataCheckBox.Checked}", Color.FromArgb(100, 200, 255));
            AppendConsoleText("", Color.White);

            try
            {
                // Start progress animation
                StartProgressAnimation();

                // Run the enhanced ROM scanner with database storage
                var scanResult = await Task.Run(() => {
                    try
                    {
                        // Progress callback to update UI
                        RomScanner.ProgressCallback progressCallback = (message, filesProcessed) => {
                            AppendConsoleText($"[SCAN] {message}", Color.Cyan);
                        };

                        // Perform the enhanced scan with database storage
                        var result = RomScanner.ScanDirectoryToDatabase(
                            directoryTextBox.Text,
                            recursive: recursiveCheckBox.Checked,
                            extractMetadata: metadataCheckBox.Checked,
                            progressCallback: progressCallback
                        );

                        return result;
                    }
                    catch (Exception ex)
                    {
                        // Handle any scanning errors
                        AppendConsoleText($"[ERROR] Scan failed: {ex.Message}", Color.Red);
                        return null;
                    }
                });

                // Stop progress animation
                StopProgressAnimation();

                if (scanResult != null)
                {
                    // Display scan results
                    AppendConsoleText("", Color.White);
                    AppendConsoleText($"[SUCCESS] Scan completed in {scanResult.Duration.TotalSeconds:F1} seconds", Color.LimeGreen);
                    AppendConsoleText("", Color.White);

                    // Show database statistics
                    AppendConsoleText("[DATABASE] Scan results:", Color.Yellow);
                    AppendConsoleText($"[DATABASE] Files found: {scanResult.FilesFound}", Color.FromArgb(100, 200, 255));
                    AppendConsoleText($"[DATABASE] Files added: {scanResult.FilesAdded}", Color.LimeGreen);
                    AppendConsoleText($"[DATABASE] Files updated: {scanResult.FilesUpdated}", Color.Orange);

                    if (scanResult.FilesRemoved > 0)
                    {
                        AppendConsoleText($"[DATABASE] Files removed: {scanResult.FilesRemoved}", Color.Red);
                    }

                    AppendConsoleText("", Color.White);

                    // Get and display collection statistics from database
                    try
                    {
                        var database = new RomDatabase();
                        var consoleStats = database.GetConsoleStats();
                        var genreStats = database.GetGenreStats();

                        AppendConsoleText("[COLLECTION] Console breakdown:", Color.FromArgb(255, 165, 0));
                        foreach (var consoleStat in consoleStats.Take(8)) // Show top 8 consoles
                        {
                            AppendConsoleText($"  ✓ {consoleStat.Key}: {consoleStat.Value} games", Color.LimeGreen);
                        }

                        if (consoleStats.Count > 8)
                        {
                            var remaining = consoleStats.Skip(8).Sum(x => x.Value);
                            AppendConsoleText($"  ... and {remaining} games in {consoleStats.Count - 8} other systems", Color.FromArgb(150, 150, 150));
                        }

                        AppendConsoleText("", Color.White);

                        if (metadataCheckBox.Checked && genreStats.Count > 0)
                        {
                            AppendConsoleText("[METADATA] Top genres:", Color.Magenta);
                            foreach (var genreStat in genreStats.Take(5)) // Show top 5 genres
                            {
                                AppendConsoleText($"  ✓ {genreStat.Key}: {genreStat.Value} games", Color.Magenta);
                            }

                            if (genreStats.Count > 5)
                            {
                                AppendConsoleText($"  ... and {genreStats.Count - 5} other genres", Color.FromArgb(150, 150, 150));
                            }

                            AppendConsoleText("", Color.White);
                        }

                        // Final summary
                        int totalRoms = consoleStats.Values.Sum();
                        AppendConsoleText("═══════════════════════════════════════", Color.Cyan);
                        AppendConsoleText("[SUMMARY] Collection overview:", Color.Yellow);
                        AppendConsoleText($"[SUMMARY] Total ROMs in database: {totalRoms:N0}", Color.Yellow);
                        AppendConsoleText($"[SUMMARY] Console systems: {consoleStats.Count}", Color.Yellow);

                        if (genreStats.Count > 0)
                        {
                            AppendConsoleText($"[SUMMARY] Unique genres: {genreStats.Count}", Color.Yellow);
                        }

                        AppendConsoleText($"[SUMMARY] Database location: rom_collection.db", Color.FromArgb(150, 150, 150));
                        AppendConsoleText("", Color.White);
                        AppendConsoleText("[READY] Database synchronized - ready for viewing and organizing", Color.LimeGreen);

                        // Save scan statistics
                        var scanStats = new ScanStatistics
                        {
                            TotalRoms = totalRoms,
                            TotalConsoles = consoleStats.Count,
                            UniqueGenres = genreStats.Count,
                            ScanDuration = scanResult.Duration,
                            TotalSizeBytes = 0 // You may want to add this to your scan result
                        };
                        settings.Settings.LastScanStats = scanStats;
                        settings.Settings.LastScanTime = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        AppendConsoleText($"[WARNING] Could not retrieve collection stats: {ex.Message}", Color.Orange);
                        AppendConsoleText("[READY] Scan completed - data saved to database", Color.LimeGreen);
                    }
                }
                else
                {
                    AppendConsoleText("[ERROR] Scan failed - see error messages above", Color.Red);
                }
            }
            catch (Exception ex)
            {
                StopProgressAnimation();
                AppendConsoleText($"[ERROR] Unexpected error: {ex.Message}", Color.Red);
                AppendConsoleText("[ERROR] Please check the directory path and database permissions", Color.Red);
            }
            finally
            {
                // Save settings including scan preferences
                SaveUserSettings();

                // Reset UI
                isScanning = false;
                scanProgress.Visible = false;
                startScanButton.Text = "⚡ Start Scan";
                startScanButton.Enabled = true;
                browseButton.Enabled = true;
            }
        }

        private async Task StartMemoryScan()
        {
            isScanning = true;

            // Update UI
            scanProgress.Visible = true;
            scanProgress.Value = 0;
            startScanButton.Text = "Scanning...";
            startScanButton.Enabled = false;
            browseButton.Enabled = false;

            // Clear console and show scan start
            consoleOutput.ClearText();
            consoleOutput.AddText("🖥️ ROM Scanner Console - Memory Mode", Color.FromArgb(100, 200, 255));
            consoleOutput.AddText("", Color.White);

            AppendConsoleText("[SCAN] Starting enhanced ROM scan...", Color.Yellow);
            AppendConsoleText($"[SCAN] Directory: {directoryTextBox.Text}", Color.FromArgb(100, 200, 255));
            AppendConsoleText($"[SCAN] Recursive: {recursiveCheckBox.Checked}", Color.FromArgb(100, 200, 255));
            AppendConsoleText($"[SCAN] Extract Metadata: {metadataCheckBox.Checked}", Color.FromArgb(100, 200, 255));
            AppendConsoleText("", Color.White);

            try
            {
                // Start progress animation
                StartProgressAnimation();

                // Run the enhanced ROM scanner in a background task
                var romInfos = await Task.Run(() => {
                    try
                    {
                        // Progress callback to update UI
                        RomScanner.ProgressCallback progressCallback = (message, filesProcessed) => {
                            AppendConsoleText($"[SCAN] {message}", Color.Cyan);

                            // Update progress for file processing
                            if (filesProcessed > 0)
                            {
                                // This will be called from background thread, so we need to invoke
                                Invoke(new Action(() => {
                                    // Don't set exact progress since we don't know total upfront
                                    // Just keep the animation running
                                }));
                            }
                        };

                        // Perform the enhanced scan with XML metadata
                        var results = RomScanner.ScanDirectory(
                            directoryTextBox.Text,
                            recursive: recursiveCheckBox.Checked,
                            extractMetadata: metadataCheckBox.Checked,
                            progressCallback: progressCallback
                        );

                        return results;
                    }
                    catch (Exception ex)
                    {
                        // Handle any scanning errors
                        AppendConsoleText($"[ERROR] Scan failed: {ex.Message}", Color.Red);
                        return null;
                    }
                });

                // Stop progress animation
                StopProgressAnimation();

                if (romInfos != null && romInfos.Count > 0)
                {
                    // Display enhanced results
                    AppendConsoleText("", Color.White);
                    AppendConsoleText($"[SUCCESS] Found {romInfos.Count} ROM files", Color.LimeGreen);
                    AppendConsoleText("", Color.White);

                    // Group by console for better display
                    var groupedRoms = romInfos.GroupBy(rom => rom.Console).OrderBy(g => g.Key);

                    foreach (var consoleGroup in groupedRoms)
                    {
                        var consoleRoms = consoleGroup.ToList();
                        AppendConsoleText($"[{consoleGroup.Key}] {consoleRoms.Count} files", Color.FromArgb(255, 165, 0));

                        // Show enhanced info for first few files
                        foreach (var rom in consoleRoms.Take(3))
                        {
                            string sizeStr = FormatFileSize(rom.Size);
                            string title = rom.Title ?? rom.Name;
                            string extraInfo = "";

                            if (metadataCheckBox.Checked && !string.IsNullOrEmpty(rom.Genre))
                            {
                                extraInfo = $" | {rom.Genre}";
                                if (rom.Year.HasValue)
                                    extraInfo += $" | {rom.Year}";
                            }

                            AppendConsoleText($"  ✓ {title} ({sizeStr}){extraInfo}", Color.LimeGreen);
                        }

                        if (consoleRoms.Count > 3)
                        {
                            AppendConsoleText($"  ... and {consoleRoms.Count - 3} more files", Color.FromArgb(150, 150, 150));
                        }

                        AppendConsoleText("", Color.White);
                    }

                    // Show metadata statistics
                    if (metadataCheckBox.Checked)
                    {
                        int withMetadata = romInfos.Where(r => !string.IsNullOrEmpty(r.Genre)).Count();
                        int withYear = romInfos.Where(r => r.Year.HasValue).Count();
                        int withStory = romInfos.Where(r => !string.IsNullOrEmpty(r.Story)).Count();

                        AppendConsoleText("[METADATA] Enhanced metadata extracted:", Color.Magenta);
                        AppendConsoleText($"[METADATA] Games with genre info: {withMetadata}", Color.Magenta);
                        AppendConsoleText($"[METADATA] Games with year info: {withYear}", Color.Magenta);
                        AppendConsoleText($"[METADATA] Games with story info: {withStory}", Color.Magenta);

                        string appDir = AppDomain.CurrentDomain.BaseDirectory;
                        string jsonPath = Path.Combine(appDir, "rom_metadata.json");
                        AppendConsoleText($"[METADATA] Enhanced data saved to: {jsonPath}", Color.Magenta);
                        AppendConsoleText("", Color.White);
                    }

                    // Final summary with enhanced stats
                    AppendConsoleText("═══════════════════════════════════════", Color.Cyan);
                    AppendConsoleText("[SUCCESS] Enhanced ROM scan completed!", Color.LimeGreen);
                    AppendConsoleText($"[SUMMARY] Processed {romInfos.Count} ROM files", Color.Yellow);
                    AppendConsoleText($"[SUMMARY] {groupedRoms.Count()} different console types detected", Color.Yellow);

                    long totalSize = romInfos.Sum(r => r.Size);
                    AppendConsoleText($"[SUMMARY] Total collection size: {FormatFileSize(totalSize)}", Color.Yellow);

                    if (metadataCheckBox.Checked)
                    {
                        var uniqueGenres = romInfos.Where(r => !string.IsNullOrEmpty(r.PrimaryGenre))
                                                  .Select(r => r.PrimaryGenre)
                                                  .Distinct()
                                                  .Count();
                        AppendConsoleText($"[SUMMARY] {uniqueGenres} unique genres identified", Color.Yellow);
                    }

                    AppendConsoleText("", Color.White);
                    AppendConsoleText("[READY] System ready for next operation", Color.LimeGreen);

                    // Save scan statistics for memory mode (reuse the totalSize variable from above)
                    var scanStats = new ScanStatistics
                    {
                        TotalRoms = romInfos.Count,
                        TotalConsoles = groupedRoms.Count(),
                        TotalSizeBytes = totalSize,
                        UniqueGenres = metadataCheckBox.Checked ?
                            romInfos.Where(r => !string.IsNullOrEmpty(r.PrimaryGenre)).Select(r => r.PrimaryGenre).Distinct().Count() : 0,
                        ScanDuration = TimeSpan.Zero // You may want to track this
                    };
                    settings.Settings.LastScanStats = scanStats;
                    settings.Settings.LastScanTime = DateTime.Now;
                }
                else
                {
                    AppendConsoleText("[WARNING] No ROM files found in the selected directory", Color.Orange);
                    AppendConsoleText("[INFO] Supported formats: .nes, .sfc, .smc, .gba, .gb, .gbc, .n64, .gen, .bin, .md, .iso, .cue, .chd, .zip, .7z", Color.FromArgb(150, 150, 150));
                }
            }
            catch (Exception ex)
            {
                StopProgressAnimation();
                AppendConsoleText($"[ERROR] Unexpected error: {ex.Message}", Color.Red);
                AppendConsoleText("[ERROR] Please check the directory path and try again", Color.Red);
            }
            finally
            {
                // Save settings including scan preferences
                SaveUserSettings();

                // Reset UI
                isScanning = false;
                scanProgress.Visible = false;
                startScanButton.Text = "⚡ Start Scan";
                startScanButton.Enabled = true;
                browseButton.Enabled = true;
            }
        }

        private void StartProgressAnimation()
        {
            scanTimer = new Timer { Interval = 100 };
            int progress = 0;

            scanTimer.Tick += (s, e) => {
                progress += 2;
                if (progress > 100) progress = 0;
                scanProgress.Value = progress;
            };

            scanTimer.Start();
        }

        private void StopProgressAnimation()
        {
            if (scanTimer != null)
            {
                scanTimer.Stop();
                scanTimer.Dispose();
                scanTimer = null;
            }
            scanProgress.Value = 100;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        private void AppendConsoleText(string text, Color color)
        {
            // Use the new custom scrollable text box
            if (InvokeRequired)
            {
                Invoke(new Action(() => consoleOutput.AddText(text, color)));
            }
            else
            {
                consoleOutput.AddText(text, color);
            }
        }

        // Method to toggle between database and memory scanning modes
        public void SetScanningMode(bool useDatabase)
        {
            settings.UseDatabaseStorage = useDatabase;
            AppendConsoleText($"[CONFIG] Scanning mode set to: {(useDatabase ? "Database Storage" : "Memory Mode")}", Color.FromArgb(100, 200, 255));
        }

        // Method to get current scanning status
        public bool IsScanning => isScanning;

        // Public method to get the current ROM directory (for other tabs to use)
        public string GetCurrentDirectory()
        {
            return settings.IsDirectoryValid() ? settings.LastSelectedDirectory : "";
        }

        // Public method to get last scan statistics (for other tabs to use)
        public ScanStatistics GetLastScanStats()
        {
            return settings.Settings.LastScanStats;
        }

        // Handle form closing to save settings
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveUserSettings();
        }

        // Handle checkbox changes to save preferences immediately
        private void recursiveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.RecursiveScanning = recursiveCheckBox.Checked;
        }

        private void metadataCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.ExtractMetadata = metadataCheckBox.Checked;
        }

        // Timer cleanup - handled by the Designer's Dispose method
        // The Form1.Designer.cs already contains the Dispose override
    }

    // Supporting classes and data structures that might be referenced
    public class ScanResult
    {
        public TimeSpan Duration { get; set; }
        public int FilesFound { get; set; }
        public int FilesAdded { get; set; }
        public int FilesUpdated { get; set; }
        public int FilesRemoved { get; set; }
    }
}