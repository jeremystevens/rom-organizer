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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Custom console is already initialized with proper styling
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select ROM Directory";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    directoryTextBox.Text = dialog.SelectedPath;
                    AppendConsoleText($"[INFO] Directory selected: {dialog.SelectedPath}", Color.FromArgb(100, 200, 255));
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

            // Start real ROM scanning
            await StartRealScan();
        }

        private async Task StartRealScan()
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
            consoleOutput.AddText("🖥️ ROM Scanner Console", Color.FromArgb(100, 200, 255));
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

        // Timer cleanup will be handled by the Designer's Dispose method
        // No need to override Dispose here since it's already in the Designer.cs
    }
}