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
        private bool isOrganizing = false;
        private bool isCleaning = false; // Add cleaning state
        private SettingsManager settings;
        private RomOrganizer romOrganizer;
        private RomCleaner romCleaner; // Add ROM cleaner
        private RomDatabase database; // Add database reference

        public Form1()
        {
            InitializeComponent();
            settings = SettingsManager.Instance;
            romOrganizer = new RomOrganizer();
            database = new RomDatabase(); // Initialize database
            romCleaner = new RomCleaner(database); // Initialize ROM cleaner
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Custom console is already initialized with proper styling
            LoadUserSettings();
            InitializeUI();
            InitializeOrganizeTab();
            InitializeCleanTab(); // Add clean tab initialization
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

            // Load organize tab settings
            LoadOrganizeSettings();

            // NOTE: Removed window settings management to preserve original form size
        }

        private void LoadOrganizeSettings()
        {
            // Load last output directory if available
            if (!string.IsNullOrEmpty(settings.LastOutputDirectory) && Directory.Exists(settings.LastOutputDirectory))
            {
                outputDirectoryTextBox.Text = settings.LastOutputDirectory;
            }

            // Load last organization method
            switch (settings.LastOrganizationMethod)
            {
                case "Genre":
                    genreRadio.Checked = true;
                    break;
                case "Console":
                    consoleRadio.Checked = true;
                    break;
                default:
                    alphabeticalRadio.Checked = true;
                    break;
            }

            // Load last action preference
            if (settings.LastMoveFiles)
                moveRomsRadio.Checked = true;
            else
                copyRomsRadio.Checked = true;
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

            // Save organize preferences
            SaveOrganizeSettings();

            // NOTE: Removed window settings saving to preserve original form size
        }

        private void SaveOrganizeSettings()
        {
            // Save output directory
            if (!string.IsNullOrEmpty(outputDirectoryTextBox.Text) && outputDirectoryTextBox.Text != "Choose a folder for organized ROMs...")
            {
                settings.LastOutputDirectory = outputDirectoryTextBox.Text;
            }

            // Save organization method
            if (genreRadio.Checked)
                settings.LastOrganizationMethod = "Genre";
            else if (consoleRadio.Checked)
                settings.LastOrganizationMethod = "Console";
            else
                settings.LastOrganizationMethod = "Alphabetical";

            // Save action preference
            settings.LastMoveFiles = moveRomsRadio.Checked;
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

        private void InitializeOrganizeTab()
        {
            // Initialize organize console with welcome message
            organizeConsoleOutput.ClearText();
            organizeConsoleOutput.AddText("📁 ROM Organizer Console", Color.FromArgb(255, 140, 0));
            organizeConsoleOutput.AddText("", Color.White);
            AppendOrganizeConsoleText("[SYSTEM] ROM Organizer initialized and ready", Color.LimeGreen);

            // Connect event handlers for organize tab
            outputBrowseButton.Click += OutputBrowseButton_Click;
            organizeButton.Click += OrganizeButton_Click;

            // Show status based on current settings
            if (!string.IsNullOrEmpty(outputDirectoryTextBox.Text) && outputDirectoryTextBox.Text != "Choose a folder for organized ROMs...")
            {
                AppendOrganizeConsoleText($"[SETTINGS] Output directory: {outputDirectoryTextBox.Text}", Color.FromArgb(100, 200, 255));
            }

            string method = alphabeticalRadio.Checked ? "Alphabetical" : genreRadio.Checked ? "Genre" : "Console";
            string action = moveRomsRadio.Checked ? "Move" : "Copy";
            AppendOrganizeConsoleText($"[SETTINGS] Method: {method} | Action: {action}", Color.FromArgb(100, 200, 255));
            AppendOrganizeConsoleText("[INFO] Configure options and click 'Organize ROMs' to begin", Color.FromArgb(150, 150, 150));
            AppendOrganizeConsoleText("", Color.White);
        }

        private void InitializeCleanTab()
        {
            // Initialize clean console with welcome message
            cleanConsoleOutput.ClearText();
            cleanConsoleOutput.AddText("🧹 ROM Cleaner Console", Color.FromArgb(255, 165, 0));
            cleanConsoleOutput.AddText("", Color.White);
            AppendCleanConsoleText("[SYSTEM] ROM Cleaner initialized and ready", Color.LimeGreen);

            // Connect event handlers for clean tab
            renameButton.Click += RenameButton_Click;
            scanDuplicatesButton.Click += ScanDuplicatesButton_Click;
            removeDuplicatesButton.Click += RemoveDuplicatesButton_Click;

            // Show current settings
            string renameSetting = removeTagsRadio.Checked ? "Remove Tags" :
                                  standardFormatRadio.Checked ? "Standard Format" : "Custom Format";
            string detectionMethod = fileHashRadio.Checked ? "File Hash" :
                                   fileSizeNameRadio.Checked ? "Size + Name" : "Name Similarity";

            AppendCleanConsoleText($"[SETTINGS] Rename method: {renameSetting}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText($"[SETTINGS] Duplicate detection: {detectionMethod}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText($"[SETTINGS] Keep best version: {keepBestVersionCheckBox.Checked}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText($"[SETTINGS] Move duplicates: {moveDuplicatesCheckBox.Checked}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText("[INFO] Configure options and click the appropriate button to begin", Color.FromArgb(150, 150, 150));
            AppendCleanConsoleText("", Color.White);
        }

        #region Scan Tab Events (existing code)

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

        #endregion

        #region Organize Tab Events

        private void OutputBrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select Output Directory for Organized ROMs";
                dialog.ShowNewFolderButton = true;

                // Start from the last selected output directory if available
                if (!string.IsNullOrEmpty(settings.LastOutputDirectory) && Directory.Exists(settings.LastOutputDirectory))
                {
                    dialog.SelectedPath = settings.LastOutputDirectory;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    outputDirectoryTextBox.Text = dialog.SelectedPath;
                    settings.LastOutputDirectory = dialog.SelectedPath; // Save immediately
                    AppendOrganizeConsoleText($"[INFO] Output directory selected: {dialog.SelectedPath}", Color.FromArgb(100, 200, 255));
                }
            }
        }

        private async void OrganizeButton_Click(object sender, EventArgs e)
        {
            if (isOrganizing)
            {
                AppendOrganizeConsoleText("[WARNING] Organization already in progress!", Color.Orange);
                return;
            }

            if (isScanning)
            {
                AppendOrganizeConsoleText("[WARNING] Cannot organize while scanning is in progress!", Color.Orange);
                return;
            }

            // Validate inputs
            if (outputDirectoryTextBox.Text == "Choose a folder for organized ROMs..." || string.IsNullOrEmpty(outputDirectoryTextBox.Text))
            {
                AppendOrganizeConsoleText("[ERROR] Please select an output directory first!", Color.Red);
                return;
            }

            if (!Directory.Exists(outputDirectoryTextBox.Text))
            {
                AppendOrganizeConsoleText("[ERROR] Selected output directory does not exist!", Color.Red);
                return;
            }

            // Check if database has ROMs
            try
            {
                var romCount = database.GetAllRoms().Count;
                if (romCount == 0)
                {
                    AppendOrganizeConsoleText("[ERROR] No ROMs found in database! Please scan your ROM collection first.", Color.Red);
                    AppendOrganizeConsoleText("[INFO] Go to the 'Scan ROM' tab to scan your collection.", Color.FromArgb(150, 150, 150));
                    return;
                }

                AppendOrganizeConsoleText($"[INFO] Found {romCount} ROMs in database - ready to organize", Color.FromArgb(100, 200, 255));
            }
            catch (Exception ex)
            {
                AppendOrganizeConsoleText($"[ERROR] Database error: {ex.Message}", Color.Red);
                return;
            }

            await StartOrganization();
        }

        private async Task StartOrganization()
        {
            isOrganizing = true;

            // Update UI
            organizeButton.Text = "Organizing...";
            organizeButton.Enabled = false;
            outputBrowseButton.Enabled = false;

            // Clear console and show organization start
            organizeConsoleOutput.ClearText();
            organizeConsoleOutput.AddText("📁 ROM Organizer Console - Active", Color.FromArgb(255, 140, 0));
            organizeConsoleOutput.AddText("", Color.White);

            // Determine organization method and action
            string method = GetSelectedOrganizationMethod();
            bool moveFiles = moveRomsRadio.Checked;
            string action = moveFiles ? "Moving" : "Copying";

            AppendOrganizeConsoleText($"[ORGANIZE] Starting ROM organization...", Color.Yellow);
            AppendOrganizeConsoleText($"[ORGANIZE] Method: {method}", Color.FromArgb(100, 200, 255));
            AppendOrganizeConsoleText($"[ORGANIZE] Action: {action} files", Color.FromArgb(100, 200, 255));
            AppendOrganizeConsoleText($"[ORGANIZE] Output: {outputDirectoryTextBox.Text}", Color.FromArgb(100, 200, 255));
            AppendOrganizeConsoleText("", Color.White);

            try
            {
                RomOrganizer.OrganizeResult result = null;

                // Progress callback to update UI
                RomOrganizer.ProgressCallback progressCallback = (message, filesProcessed) => {
                    AppendOrganizeConsoleText($"[PROGRESS] {message}", Color.Cyan);
                };

                // Execute organization based on selected method
                switch (method)
                {
                    case "Alphabetical":
                        result = await romOrganizer.OrganizeAlphabeticalAsync(
                            outputDirectoryTextBox.Text,
                            moveFiles,
                            removeSpecialChars: true,
                            maxFilenameLength: 100,
                            maxFilesPerFolder: 1000,
                            progressCallback: progressCallback
                        );
                        break;

                    case "Console":
                        result = await romOrganizer.OrganizeByConsoleAsync(
                            outputDirectoryTextBox.Text,
                            moveFiles,
                            removeSpecialChars: true,
                            maxFilenameLength: 100,
                            progressCallback: progressCallback
                        );
                        break;

                    case "Genre":
                        // Auto-detect the best XML file for the collection
                        // The organizer will intelligently choose based on console types
                        result = await romOrganizer.OrganizeByGenreAsync(
                            outputDirectoryTextBox.Text,
                            moveFiles,
                            xmlFilePath: null, // Let the organizer auto-detect
                            removeSpecialChars: true,
                            maxFilenameLength: 100,
                            progressCallback: progressCallback
                        );
                        break;
                }

                // Display results
                if (result != null)
                {
                    AppendOrganizeConsoleText("", Color.White);
                    AppendOrganizeConsoleText($"[SUCCESS] Organization completed in {result.Duration.TotalSeconds:F1} seconds", Color.LimeGreen);
                    AppendOrganizeConsoleText("", Color.White);

                    // Show detailed results
                    AppendOrganizeConsoleText("[RESULTS] Organization summary:", Color.Yellow);
                    AppendOrganizeConsoleText($"[RESULTS] Total files processed: {result.FilesProcessed}", Color.FromArgb(100, 200, 255));

                    if (moveFiles)
                    {
                        AppendOrganizeConsoleText($"[RESULTS] Files moved: {result.FilesMoved}", Color.LimeGreen);
                    }
                    else
                    {
                        AppendOrganizeConsoleText($"[RESULTS] Files copied: {result.FilesCopied}", Color.LimeGreen);
                    }

                    if (result.FilesSkipped > 0)
                    {
                        AppendOrganizeConsoleText($"[RESULTS] Files skipped: {result.FilesSkipped}", Color.Orange);
                    }

                    // Show any errors
                    if (result.Errors.Count > 0)
                    {
                        AppendOrganizeConsoleText("", Color.White);
                        AppendOrganizeConsoleText("[ERRORS] Some issues occurred:", Color.Red);
                        foreach (var error in result.Errors.Take(10)) // Show first 10 errors
                        {
                            AppendOrganizeConsoleText($"[ERROR] {error}", Color.Red);
                        }
                        if (result.Errors.Count > 10)
                        {
                            AppendOrganizeConsoleText($"[ERROR] ... and {result.Errors.Count - 10} more errors", Color.Red);
                        }
                    }

                    AppendOrganizeConsoleText("", Color.White);
                    AppendOrganizeConsoleText("═══════════════════════════════════════", Color.Cyan);
                    AppendOrganizeConsoleText($"[COMPLETE] ROM organization finished using {method} method", Color.LimeGreen);
                    AppendOrganizeConsoleText($"[COMPLETE] Output location: {outputDirectoryTextBox.Text}", Color.FromArgb(150, 150, 150));
                    AppendOrganizeConsoleText("[READY] Ready for next operation", Color.LimeGreen);
                }
                else
                {
                    AppendOrganizeConsoleText("[ERROR] Organization failed - see error messages above", Color.Red);
                }
            }
            catch (Exception ex)
            {
                AppendOrganizeConsoleText($"[ERROR] Unexpected error: {ex.Message}", Color.Red);
                AppendOrganizeConsoleText("[ERROR] Please check the output directory and database", Color.Red);
            }
            finally
            {
                // Save settings
                SaveOrganizeSettings();

                // Reset UI
                isOrganizing = false;
                organizeButton.Text = "📁 Organize ROMs";
                organizeButton.Enabled = true;
                outputBrowseButton.Enabled = true;
            }
        }

        private string GetSelectedOrganizationMethod()
        {
            if (alphabeticalRadio.Checked)
                return "Alphabetical";
            else if (genreRadio.Checked)
                return "Genre";
            else if (consoleRadio.Checked)
                return "Console";
            else
                return "Alphabetical"; // Default fallback
        }

        #endregion

        #region Clean Tab Events

        private async void RenameButton_Click(object sender, EventArgs e)
        {
            if (isCleaning)
            {
                AppendCleanConsoleText("[WARNING] Cleaning operation already in progress!", Color.Orange);
                return;
            }

            if (isScanning || isOrganizing)
            {
                AppendCleanConsoleText("[WARNING] Cannot clean while other operations are in progress!", Color.Orange);
                return;
            }

            // Check if database has ROMs
            try
            {
                var romCount = database.GetAllRoms().Count;
                if (romCount == 0)
                {
                    AppendCleanConsoleText("[ERROR] No ROMs found in database! Please scan your ROM collection first.", Color.Red);
                    AppendCleanConsoleText("[INFO] Go to the 'Scan ROM' tab to scan your collection.", Color.FromArgb(150, 150, 150));
                    return;
                }

                AppendCleanConsoleText($"[INFO] Found {romCount} ROMs in database - ready to rename", Color.FromArgb(100, 200, 255));
            }
            catch (Exception ex)
            {
                AppendCleanConsoleText($"[ERROR] Database error: {ex.Message}", Color.Red);
                return;
            }

            await StartRenameOperation();
        }

        private async void ScanDuplicatesButton_Click(object sender, EventArgs e)
        {
            if (isCleaning)
            {
                AppendCleanConsoleText("[WARNING] Cleaning operation already in progress!", Color.Orange);
                return;
            }

            if (isScanning || isOrganizing)
            {
                AppendCleanConsoleText("[WARNING] Cannot scan duplicates while other operations are in progress!", Color.Orange);
                return;
            }

            // Check if database has ROMs
            try
            {
                var romCount = database.GetAllRoms().Count;
                if (romCount == 0)
                {
                    AppendCleanConsoleText("[ERROR] No ROMs found in database! Please scan your ROM collection first.", Color.Red);
                    AppendCleanConsoleText("[INFO] Go to the 'Scan ROM' tab to scan your collection.", Color.FromArgb(150, 150, 150));
                    return;
                }

                AppendCleanConsoleText($"[INFO] Found {romCount} ROMs in database - ready to scan for duplicates", Color.FromArgb(100, 200, 255));
            }
            catch (Exception ex)
            {
                AppendCleanConsoleText($"[ERROR] Database error: {ex.Message}", Color.Red);
                return;
            }

            await StartDuplicateScanOperation();
        }

        private async void RemoveDuplicatesButton_Click(object sender, EventArgs e)
        {
            if (isCleaning)
            {
                AppendCleanConsoleText("[WARNING] Cleaning operation already in progress!", Color.Orange);
                return;
            }

            if (isScanning || isOrganizing)
            {
                AppendCleanConsoleText("[WARNING] Cannot remove duplicates while other operations are in progress!", Color.Orange);
                return;
            }

            // Check if database has ROMs
            try
            {
                var romCount = database.GetAllRoms().Count;
                if (romCount == 0)
                {
                    AppendCleanConsoleText("[ERROR] No ROMs found in database! Please scan your ROM collection first.", Color.Red);
                    AppendCleanConsoleText("[INFO] Go to the 'Scan ROM' tab to scan your collection.", Color.FromArgb(150, 150, 150));
                    return;
                }

                AppendCleanConsoleText($"[INFO] Found {romCount} ROMs in database - ready to remove duplicates", Color.FromArgb(100, 200, 255));
            }
            catch (Exception ex)
            {
                AppendCleanConsoleText($"[ERROR] Database error: {ex.Message}", Color.Red);
                return;
            }

            await StartDuplicateRemovalOperation();
        }

        private async Task StartRenameOperation()
        {
            isCleaning = true;

            // Update UI
            renameButton.Text = "Renaming...";
            renameButton.Enabled = false;

            // Clear console and show rename start
            cleanConsoleOutput.ClearText();
            cleanConsoleOutput.AddText("🧹 ROM Cleaner Console - Renaming", Color.FromArgb(255, 165, 0));
            cleanConsoleOutput.AddText("", Color.White);

            // Configure rename settings
            var config = new RomCleaner.RenameConfiguration();

            if (removeTagsRadio.Checked)
                config.Convention = RomCleaner.NamingConvention.RemoveTags;
            else if (standardFormatRadio.Checked)
                config.Convention = RomCleaner.NamingConvention.StandardFormat;
            else
            {
                config.Convention = RomCleaner.NamingConvention.CustomFormat;
                config.CustomFormat = customFormatTextBox.Text;
            }

            AppendCleanConsoleText("[RENAME] Starting ROM rename operation...", Color.Yellow);
            AppendCleanConsoleText($"[RENAME] Method: {config.Convention}", Color.FromArgb(100, 200, 255));
            if (config.Convention == RomCleaner.NamingConvention.CustomFormat)
            {
                AppendCleanConsoleText($"[RENAME] Custom format: {config.CustomFormat}", Color.FromArgb(100, 200, 255));
            }
            AppendCleanConsoleText("", Color.White);

            try
            {
                // Progress callback to update UI
                var progress = new Progress<string>(message => {
                    AppendCleanConsoleText($"[PROGRESS] {message}", Color.Cyan);
                });

                var result = await romCleaner.RenameRomsAsync(config, progress);

                // Display results
                AppendCleanConsoleText("", Color.White);
                AppendCleanConsoleText($"[SUCCESS] Rename operation completed in {result.Duration.TotalSeconds:F1} seconds", Color.LimeGreen);
                AppendCleanConsoleText("", Color.White);

                AppendCleanConsoleText("[RESULTS] Rename summary:", Color.Yellow);
                AppendCleanConsoleText($"[RESULTS] Total files processed: {result.TotalFilesProcessed}", Color.FromArgb(100, 200, 255));
                AppendCleanConsoleText($"[RESULTS] Files renamed: {result.FilesRenamed}", Color.LimeGreen);
                AppendCleanConsoleText($"[RESULTS] Files skipped: {result.FilesSkipped}", Color.Orange);

                if (result.Errors > 0)
                {
                    AppendCleanConsoleText($"[RESULTS] Errors: {result.Errors}", Color.Red);
                    AppendCleanConsoleText("", Color.White);
                    AppendCleanConsoleText("[ERRORS] First few errors:", Color.Red);
                    foreach (var error in result.ErrorMessages.Take(5))
                    {
                        AppendCleanConsoleText($"[ERROR] {error}", Color.Red);
                    }
                    if (result.ErrorMessages.Count > 5)
                    {
                        AppendCleanConsoleText($"[ERROR] ... and {result.ErrorMessages.Count - 5} more errors", Color.Red);
                    }
                }

                AppendCleanConsoleText("", Color.White);
                AppendCleanConsoleText("═══════════════════════════════════════", Color.Cyan);
                AppendCleanConsoleText("[COMPLETE] ROM rename operation finished", Color.LimeGreen);
                AppendCleanConsoleText("[READY] Ready for next operation", Color.LimeGreen);
            }
            catch (Exception ex)
            {
                AppendCleanConsoleText($"[ERROR] Unexpected error during rename: {ex.Message}", Color.Red);
            }
            finally
            {
                isCleaning = false;
                renameButton.Text = "[R] Apply Renames";
                renameButton.Enabled = true;
            }
        }

        private async Task StartDuplicateScanOperation()
        {
            isCleaning = true;

            // Update UI
            scanDuplicatesButton.Text = "Scanning...";
            scanDuplicatesButton.Enabled = false;

            // Clear console and show scan start
            cleanConsoleOutput.ClearText();
            cleanConsoleOutput.AddText("🧹 ROM Cleaner Console - Scanning Duplicates", Color.FromArgb(255, 165, 0));
            cleanConsoleOutput.AddText("", Color.White);

            // Configure duplicate settings
            var config = new RomCleaner.DuplicateConfiguration
            {
                PreviewOnly = true // Only scan, don't remove
            };

            if (fileHashRadio.Checked)
                config.DetectionMethod = RomCleaner.DuplicateDetectionMethod.FileHash;
            else if (fileSizeNameRadio.Checked)
                config.DetectionMethod = RomCleaner.DuplicateDetectionMethod.FileSizeAndName;
            else
                config.DetectionMethod = RomCleaner.DuplicateDetectionMethod.NameSimilarity;

            config.KeepBestVersion = keepBestVersionCheckBox.Checked;
            config.MoveDuplicatesToFolder = moveDuplicatesCheckBox.Checked;

            AppendCleanConsoleText("[SCAN] Starting duplicate scan operation...", Color.Yellow);
            AppendCleanConsoleText($"[SCAN] Detection method: {config.DetectionMethod}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText($"[SCAN] Keep best version: {config.KeepBestVersion}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText("", Color.White);

            try
            {
                // Progress callback to update UI
                var progress = new Progress<string>(message => {
                    AppendCleanConsoleText($"[PROGRESS] {message}", Color.Cyan);
                });

                var result = await romCleaner.ScanForDuplicatesAsync(config, progress);

                // Display results
                AppendCleanConsoleText("", Color.White);
                AppendCleanConsoleText($"[SUCCESS] Duplicate scan completed in {result.Duration.TotalSeconds:F1} seconds", Color.LimeGreen);
                AppendCleanConsoleText("", Color.White);

                AppendCleanConsoleText("[RESULTS] Duplicate scan summary:", Color.Yellow);
                AppendCleanConsoleText($"[RESULTS] Total files scanned: {result.TotalFilesScanned}", Color.FromArgb(100, 200, 255));
                AppendCleanConsoleText($"[RESULTS] Duplicate groups found: {result.DuplicateGroupsFound}", Color.Orange);
                AppendCleanConsoleText($"[RESULTS] Duplicate files found: {result.DuplicateFilesFound}", Color.Orange);

                if (result.DuplicateGroupsFound > 0)
                {
                    AppendCleanConsoleText("", Color.White);
                    AppendCleanConsoleText("[DUPLICATES] Sample duplicate groups:", Color.Orange);
                    foreach (var group in result.DuplicateGroups.Take(5))
                    {
                        AppendCleanConsoleText($"[GROUP] {group.Files.Count} duplicates:", Color.Orange);
                        foreach (var file in group.Files.Take(3))
                        {
                            AppendCleanConsoleText($"  - {file.Name} ({romCleaner.FormatFileSize(file.Size)})", Color.FromArgb(150, 150, 150));
                        }
                        if (group.Files.Count > 3)
                        {
                            AppendCleanConsoleText($"  ... and {group.Files.Count - 3} more files", Color.FromArgb(150, 150, 150));
                        }
                        AppendCleanConsoleText("", Color.White);
                    }

                    if (result.DuplicateGroupsFound > 5)
                    {
                        AppendCleanConsoleText($"[INFO] ... and {result.DuplicateGroupsFound - 5} more duplicate groups", Color.FromArgb(150, 150, 150));
                    }

                    long potentialSpaceSaved = result.DuplicateGroups.Sum(g => g.Files.Skip(1).Sum(f => f.Size));
                    AppendCleanConsoleText($"[INFO] Potential space to reclaim: {romCleaner.FormatFileSize(potentialSpaceSaved)}", Color.Yellow);
                }
                else
                {
                    AppendCleanConsoleText("[INFO] No duplicates found in your collection!", Color.LimeGreen);
                }

                AppendCleanConsoleText("", Color.White);
                AppendCleanConsoleText("═══════════════════════════════════════", Color.Cyan);
                AppendCleanConsoleText("[COMPLETE] Duplicate scan finished", Color.LimeGreen);
                AppendCleanConsoleText("[INFO] Use 'Remove Duplicates' to clean your collection", Color.FromArgb(150, 150, 150));
                AppendCleanConsoleText("[READY] Ready for next operation", Color.LimeGreen);
            }
            catch (Exception ex)
            {
                AppendCleanConsoleText($"[ERROR] Unexpected error during duplicate scan: {ex.Message}", Color.Red);
            }
            finally
            {
                isCleaning = false;
                scanDuplicatesButton.Text = "[S] Scan for Duplicates";
                scanDuplicatesButton.Enabled = true;
            }
        }

        private async Task StartDuplicateRemovalOperation()
        {
            isCleaning = true;

            // Update UI
            removeDuplicatesButton.Text = "Removing...";
            removeDuplicatesButton.Enabled = false;

            // Clear console and show removal start
            cleanConsoleOutput.ClearText();
            cleanConsoleOutput.AddText("🧹 ROM Cleaner Console - Removing Duplicates", Color.FromArgb(255, 165, 0));
            cleanConsoleOutput.AddText("", Color.White);

            // Configure duplicate settings
            var config = new RomCleaner.DuplicateConfiguration
            {
                PreviewOnly = false // Actually remove duplicates
            };

            if (fileHashRadio.Checked)
                config.DetectionMethod = RomCleaner.DuplicateDetectionMethod.FileHash;
            else if (fileSizeNameRadio.Checked)
                config.DetectionMethod = RomCleaner.DuplicateDetectionMethod.FileSizeAndName;
            else
                config.DetectionMethod = RomCleaner.DuplicateDetectionMethod.NameSimilarity;

            config.KeepBestVersion = keepBestVersionCheckBox.Checked;
            config.MoveDuplicatesToFolder = moveDuplicatesCheckBox.Checked;
            config.DuplicatesFolderPath = "Duplicates";

            AppendCleanConsoleText("[REMOVE] Starting duplicate removal operation...", Color.Yellow);
            AppendCleanConsoleText($"[REMOVE] Detection method: {config.DetectionMethod}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText($"[REMOVE] Keep best version: {config.KeepBestVersion}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText($"[REMOVE] Move to folder: {config.MoveDuplicatesToFolder}", Color.FromArgb(100, 200, 255));
            AppendCleanConsoleText("", Color.White);

            try
            {
                // Progress callback to update UI
                var progress = new Progress<string>(message => {
                    AppendCleanConsoleText($"[PROGRESS] {message}", Color.Cyan);
                });

                var result = await romCleaner.RemoveDuplicatesAsync(config, progress);

                // Display results
                AppendCleanConsoleText("", Color.White);
                AppendCleanConsoleText($"[SUCCESS] Duplicate removal completed in {result.Duration.TotalSeconds:F1} seconds", Color.LimeGreen);
                AppendCleanConsoleText("", Color.White);

                AppendCleanConsoleText("[RESULTS] Duplicate removal summary:", Color.Yellow);
                AppendCleanConsoleText($"[RESULTS] Total files scanned: {result.TotalFilesScanned}", Color.FromArgb(100, 200, 255));
                AppendCleanConsoleText($"[RESULTS] Duplicate groups found: {result.DuplicateGroupsFound}", Color.Orange);
                AppendCleanConsoleText($"[RESULTS] Duplicate files found: {result.DuplicateFilesFound}", Color.Orange);

                if (config.MoveDuplicatesToFolder)
                {
                    AppendCleanConsoleText($"[RESULTS] Files moved to Duplicates folder: {result.FilesMoved}", Color.LimeGreen);
                }
                else
                {
                    AppendCleanConsoleText($"[RESULTS] Files removed: {result.FilesRemoved}", Color.LimeGreen);
                }

                AppendCleanConsoleText($"[RESULTS] Space reclaimed: {romCleaner.FormatFileSize(result.SpaceReclaimed)}", Color.LimeGreen);

                if (result.ErrorMessages.Count > 0)
                {
                    AppendCleanConsoleText("", Color.White);
                    AppendCleanConsoleText("[ERRORS] Some issues occurred:", Color.Red);
                    foreach (var error in result.ErrorMessages.Take(5))
                    {
                        AppendCleanConsoleText($"[ERROR] {error}", Color.Red);
                    }
                    if (result.ErrorMessages.Count > 5)
                    {
                        AppendCleanConsoleText($"[ERROR] ... and {result.ErrorMessages.Count - 5} more errors", Color.Red);
                    }
                }

                AppendCleanConsoleText("", Color.White);
                AppendCleanConsoleText("═══════════════════════════════════════", Color.Cyan);
                AppendCleanConsoleText("[COMPLETE] Duplicate removal finished", Color.LimeGreen);
                AppendCleanConsoleText("[READY] Ready for next operation", Color.LimeGreen);
            }
            catch (Exception ex)
            {
                AppendCleanConsoleText($"[ERROR] Unexpected error during duplicate removal: {ex.Message}", Color.Red);
            }
            finally
            {
                isCleaning = false;
                removeDuplicatesButton.Text = "[X] Remove Duplicates";
                removeDuplicatesButton.Enabled = true;
            }
        }

        #endregion

        #region Existing Scan Methods (keeping existing implementation)

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
                        // Suppress obsolete warning: ScanDirectory is used intentionally for memory mode
#pragma warning disable CS0618
                        var results = RomScanner.ScanDirectory(
                            directoryTextBox.Text,
                            recursive: recursiveCheckBox.Checked,
                            extractMetadata: metadataCheckBox.Checked,
                            progressCallback: progressCallback
                        );
#pragma warning restore CS0618

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

        #endregion

        #region Helper Methods

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

        private void AppendOrganizeConsoleText(string text, Color color)
        {
            // Use the organize console text box
            if (InvokeRequired)
            {
                Invoke(new Action(() => organizeConsoleOutput.AddText(text, color)));
            }
            else
            {
                organizeConsoleOutput.AddText(text, color);
            }
        }

        private void AppendCleanConsoleText(string text, Color color)
        {
            // Use the clean console text box
            if (InvokeRequired)
            {
                Invoke(new Action(() => cleanConsoleOutput.AddText(text, color)));
            }
            else
            {
                cleanConsoleOutput.AddText(text, color);
            }
        }

        #endregion

        #region Public Methods and Properties

        // Method to toggle between database and memory scanning modes
        public void SetScanningMode(bool useDatabase)
        {
            settings.UseDatabaseStorage = useDatabase;
            AppendConsoleText($"[CONFIG] Scanning mode set to: {(useDatabase ? "Database Storage" : "Memory Mode")}", Color.FromArgb(100, 200, 255));
        }

        // Method to get current scanning status
        public bool IsScanning => isScanning;

        // Method to get current organizing status
        public bool IsOrganizing => isOrganizing;

        // Method to get current cleaning status
        public bool IsCleaning => isCleaning;

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

        #endregion

        #region Event Handlers

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

        #endregion
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