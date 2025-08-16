namespace rom_organizer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.arcadeFormSkin1 = new RetroArcadeUI.ArcadeFormSkin();
            this.arcadeTabControl1 = new RetroArcadeUI.ArcadeTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.scanMainPanel = new RetroArcadeUI.ArcadePanel();
            this.titleLabel = new RetroArcadeUI.ArcadeLabel();
            this.directoryGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.directoryLabel = new RetroArcadeUI.ArcadeLabel();
            this.directoryTextBox = new RetroArcadeUI.ArcadeTextBox();
            this.browseButton = new RetroArcadeUI.ArcadeButton();
            this.optionsGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.recursiveCheckBox = new RetroArcadeUI.ArcadeCheckBox();
            this.metadataCheckBox = new RetroArcadeUI.ArcadeCheckBox();
            this.startScanButton = new RetroArcadeUI.ArcadeButton();
            this.scanProgress = new RetroArcadeUI.ArcadeProgressBar();
            this.consoleGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.consoleOutput = new RetroArcadeUI.ArcadeScrollableTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.organizeMainPanel = new RetroArcadeUI.ArcadePanel();
            this.organizeTitleLabel = new RetroArcadeUI.ArcadeLabel();
            this.organizationMethodGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.alphabeticalRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.genreRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.consoleRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.actionOptionsGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.moveRomsRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.copyRomsRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.outputDirectoryGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.outputDirectoryLabel = new RetroArcadeUI.ArcadeLabel();
            this.outputDirectoryTextBox = new RetroArcadeUI.ArcadeTextBox();
            this.outputBrowseButton = new RetroArcadeUI.ArcadeButton();
            this.organizeButton = new RetroArcadeUI.ArcadeButton();
            this.organizeOutputGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.organizeConsoleOutput = new RetroArcadeUI.ArcadeScrollableTextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.cleanMainPanel = new RetroArcadeUI.ArcadePanel();
            this.moveDuplicatesCheckBox = new RetroArcadeUI.ArcadeCheckBox();
            this.cleanTitleLabel = new RetroArcadeUI.ArcadeLabel();
            this.renameGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.renameHeaderLabel = new RetroArcadeUI.ArcadeLabel();
            this.namingConventionGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.removeTagsRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.standardFormatRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.customFormatRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.customFormatTextBox = new RetroArcadeUI.ArcadeTextBox();
            this.renameButton = new RetroArcadeUI.ArcadeButton();
            this.removeDuplicatesButton = new RetroArcadeUI.ArcadeButton();
            this.scanDuplicatesButton = new RetroArcadeUI.ArcadeButton();
            this.deduplicateGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.deduplicateHeaderLabel = new RetroArcadeUI.ArcadeLabel();
            this.detectionMethodGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.fileHashRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.fileSizeNameRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.nameSimilarityRadio = new RetroArcadeUI.ArcadeRadioButton();
            this.duplicateHandlingGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.keepBestVersionCheckBox = new RetroArcadeUI.ArcadeCheckBox();
            this.cleanOutputGroup = new RetroArcadeUI.ArcadeGroupBox();
            this.cleanConsoleOutput = new RetroArcadeUI.ArcadeScrollableTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.retroGroupBox1 = new RetroForms.RetroGroupBox();
            this.arcadeFormSkin1.SuspendLayout();
            this.arcadeTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.scanMainPanel.SuspendLayout();
            this.directoryGroup.SuspendLayout();
            this.optionsGroup.SuspendLayout();
            this.consoleGroup.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.organizeMainPanel.SuspendLayout();
            this.organizationMethodGroup.SuspendLayout();
            this.actionOptionsGroup.SuspendLayout();
            this.outputDirectoryGroup.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.cleanMainPanel.SuspendLayout();
            this.renameGroup.SuspendLayout();
            this.namingConventionGroup.SuspendLayout();
            this.deduplicateGroup.SuspendLayout();
            this.detectionMethodGroup.SuspendLayout();
            this.duplicateHandlingGroup.SuspendLayout();
            this.cleanOutputGroup.SuspendLayout();
            this.retroGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // arcadeFormSkin1
            // 
            this.arcadeFormSkin1.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(20)))), ((int)(((byte)(147)))));
            this.arcadeFormSkin1.AnimatedScanLines = true;
            this.arcadeFormSkin1.BackColor = System.Drawing.Color.White;
            this.arcadeFormSkin1.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(30)))));
            this.arcadeFormSkin1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.arcadeFormSkin1.Controls.Add(this.arcadeTabControl1);
            this.arcadeFormSkin1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arcadeFormSkin1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.arcadeFormSkin1.FormTitle = "ROM MANAGER";
            this.arcadeFormSkin1.GlowingTitle = true;
            this.arcadeFormSkin1.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(40)))));
            this.arcadeFormSkin1.Location = new System.Drawing.Point(0, 0);
            this.arcadeFormSkin1.Name = "arcadeFormSkin1";
            this.arcadeFormSkin1.ShowLogo = true;
            this.arcadeFormSkin1.ShowWindowControls = true;
            this.arcadeFormSkin1.Size = new System.Drawing.Size(1400, 900);
            this.arcadeFormSkin1.TabIndex = 0;
            // 
            // arcadeTabControl1
            // 
            this.arcadeTabControl1.Controls.Add(this.tabPage1);
            this.arcadeTabControl1.Controls.Add(this.tabPage2);
            this.arcadeTabControl1.Controls.Add(this.tabPage5);
            this.arcadeTabControl1.Controls.Add(this.tabPage3);
            this.arcadeTabControl1.Controls.Add(this.tabPage4);
            this.arcadeTabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.arcadeTabControl1.ItemSize = new System.Drawing.Size(120, 35);
            this.arcadeTabControl1.Location = new System.Drawing.Point(0, 51);
            this.arcadeTabControl1.Name = "arcadeTabControl1";
            this.arcadeTabControl1.Padding = new System.Drawing.Point(0, 0);
            this.arcadeTabControl1.SelectedIndex = 0;
            this.arcadeTabControl1.Size = new System.Drawing.Size(1397, 846);
            this.arcadeTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.arcadeTabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.tabPage1.Controls.Add(this.scanMainPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1389, 803);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Scan ROM";
            // 
            // scanMainPanel
            // 
            this.scanMainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.scanMainPanel.Controls.Add(this.directoryTextBox);
            this.scanMainPanel.Controls.Add(this.directoryLabel);
            this.scanMainPanel.Controls.Add(this.startScanButton);
            this.scanMainPanel.Controls.Add(this.optionsGroup);
            this.scanMainPanel.Controls.Add(this.titleLabel);
            this.scanMainPanel.Controls.Add(this.directoryGroup);
            this.scanMainPanel.Controls.Add(this.scanProgress);
            this.scanMainPanel.Controls.Add(this.consoleGroup);
            this.scanMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scanMainPanel.GlowBorder = true;
            this.scanMainPanel.Location = new System.Drawing.Point(3, 3);
            this.scanMainPanel.Name = "scanMainPanel";
            this.scanMainPanel.ScanLines = true;
            this.scanMainPanel.Size = new System.Drawing.Size(1383, 797);
            this.scanMainPanel.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Glowing = false;
            this.titleLabel.Location = new System.Drawing.Point(28, 30);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(400, 50);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Scan & Index ROMs";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.titleLabel.TextStyle = RetroArcadeUI.ArcadeLabel.ArcadeTextStyle.Normal;
            // 
            // directoryGroup
            // 
            this.directoryGroup.BackColor = System.Drawing.Color.White;
            this.directoryGroup.Controls.Add(this.browseButton);
            this.directoryGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.directoryGroup.GlowBorder = false;
            this.directoryGroup.HeaderText = "";
            this.directoryGroup.Location = new System.Drawing.Point(60, 100);
            this.directoryGroup.Name = "directoryGroup";
            this.directoryGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.directoryGroup.ScanLines = false;
            this.directoryGroup.Size = new System.Drawing.Size(1200, 277);
            this.directoryGroup.TabIndex = 1;
            // 
            // directoryLabel
            // 
            this.directoryLabel.BackColor = System.Drawing.Color.Transparent;
            this.directoryLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.directoryLabel.ForeColor = System.Drawing.Color.White;
            this.directoryLabel.Glowing = false;
            this.directoryLabel.Location = new System.Drawing.Point(76, 130);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(130, 30);
            this.directoryLabel.TabIndex = 0;
            this.directoryLabel.Text = "ROM Directory:";
            this.directoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.directoryLabel.TextStyle = RetroArcadeUI.ArcadeLabel.ArcadeTextStyle.Normal;
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.directoryTextBox.Glowing = false;
            this.directoryTextBox.Location = new System.Drawing.Point(212, 130);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(800, 30);
            this.directoryTextBox.TabIndex = 1;
            this.directoryTextBox.Text = "Choose a folder...";
            // 
            // browseButton
            // 
            this.browseButton.BackColor = System.Drawing.Color.Transparent;
            this.browseButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(144)))), ((int)(((byte)(220)))));
            this.browseButton.CrispText = true;
            this.browseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.browseButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.browseButton.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.browseButton.Glowing = false;
            this.browseButton.Location = new System.Drawing.Point(974, 30);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(100, 30);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse";
            this.browseButton.TextColor = System.Drawing.Color.White;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // optionsGroup
            // 
            this.optionsGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(27)))));
            this.optionsGroup.Controls.Add(this.recursiveCheckBox);
            this.optionsGroup.Controls.Add(this.metadataCheckBox);
            this.optionsGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.optionsGroup.GlowBorder = true;
            this.optionsGroup.HeaderText = "Scan Options";
            this.optionsGroup.Location = new System.Drawing.Point(80, 171);
            this.optionsGroup.Name = "optionsGroup";
            this.optionsGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.optionsGroup.ScanLines = false;
            this.optionsGroup.Size = new System.Drawing.Size(400, 139);
            this.optionsGroup.TabIndex = 2;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.recursiveCheckBox.Checked = true;
            this.recursiveCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.recursiveCheckBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.recursiveCheckBox.ForeColor = System.Drawing.Color.White;
            this.recursiveCheckBox.Glowing = false;
            this.recursiveCheckBox.Location = new System.Drawing.Point(20, 45);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(350, 25);
            this.recursiveCheckBox.TabIndex = 0;
            this.recursiveCheckBox.Text = "Scan subdirectories recursively";
            // 
            // metadataCheckBox
            // 
            this.metadataCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.metadataCheckBox.Checked = true;
            this.metadataCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.metadataCheckBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.metadataCheckBox.ForeColor = System.Drawing.Color.White;
            this.metadataCheckBox.Glowing = false;
            this.metadataCheckBox.Location = new System.Drawing.Point(20, 80);
            this.metadataCheckBox.Name = "metadataCheckBox";
            this.metadataCheckBox.Size = new System.Drawing.Size(200, 25);
            this.metadataCheckBox.TabIndex = 1;
            this.metadataCheckBox.Text = "Extract metadata";
            // 
            // startScanButton
            // 
            this.startScanButton.BackColor = System.Drawing.Color.Transparent;
            this.startScanButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(144)))), ((int)(((byte)(220)))));
            this.startScanButton.CrispText = true;
            this.startScanButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.startScanButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.startScanButton.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.startScanButton.Glowing = false;
            this.startScanButton.Location = new System.Drawing.Point(80, 326);
            this.startScanButton.Name = "startScanButton";
            this.startScanButton.Size = new System.Drawing.Size(150, 40);
            this.startScanButton.TabIndex = 3;
            this.startScanButton.Text = "⚡ Start Scan";
            this.startScanButton.TextColor = System.Drawing.Color.White;
            this.startScanButton.Click += new System.EventHandler(this.startScanButton_Click);
            // 
            // scanProgress
            // 
            this.scanProgress.Animated = true;
            this.scanProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.scanProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.scanProgress.Location = new System.Drawing.Point(289, 326);
            this.scanProgress.Maximum = 100;
            this.scanProgress.Name = "scanProgress";
            this.scanProgress.Size = new System.Drawing.Size(400, 20);
            this.scanProgress.TabIndex = 4;
            this.scanProgress.Value = 0;
            this.scanProgress.Visible = false;
            // 
            // consoleGroup
            // 
            this.consoleGroup.Controls.Add(this.consoleOutput);
            this.consoleGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.consoleGroup.GlowBorder = false;
            this.consoleGroup.HeaderText = "Scan Output";
            this.consoleGroup.Location = new System.Drawing.Point(60, 400);
            this.consoleGroup.Name = "consoleGroup";
            this.consoleGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.consoleGroup.ScanLines = false;
            this.consoleGroup.Size = new System.Drawing.Size(1200, 330);
            this.consoleGroup.TabIndex = 5;
            // 
            // consoleOutput
            // 
            this.consoleOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.consoleOutput.Location = new System.Drawing.Point(20, 40);
            this.consoleOutput.Name = "consoleOutput";
            this.consoleOutput.Size = new System.Drawing.Size(1160, 270);
            this.consoleOutput.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.tabPage2.Controls.Add(this.organizeMainPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 39);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1389, 803);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sort ROMs";
            // 
            // organizeMainPanel
            // 
            this.organizeMainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.organizeMainPanel.Controls.Add(this.organizeConsoleOutput);
            this.organizeMainPanel.Controls.Add(this.organizeTitleLabel);
            this.organizeMainPanel.Controls.Add(this.retroGroupBox1);
            this.organizeMainPanel.Controls.Add(this.organizeOutputGroup);
            this.organizeMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.organizeMainPanel.GlowBorder = true;
            this.organizeMainPanel.Location = new System.Drawing.Point(3, 3);
            this.organizeMainPanel.Name = "organizeMainPanel";
            this.organizeMainPanel.ScanLines = true;
            this.organizeMainPanel.Size = new System.Drawing.Size(1383, 797);
            this.organizeMainPanel.TabIndex = 0;
            // 
            // organizeTitleLabel
            // 
            this.organizeTitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.organizeTitleLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.organizeTitleLabel.ForeColor = System.Drawing.Color.White;
            this.organizeTitleLabel.Glowing = false;
            this.organizeTitleLabel.Location = new System.Drawing.Point(58, 0);
            this.organizeTitleLabel.Name = "organizeTitleLabel";
            this.organizeTitleLabel.Size = new System.Drawing.Size(261, 50);
            this.organizeTitleLabel.TabIndex = 0;
            this.organizeTitleLabel.Text = "Organize ROMs";
            this.organizeTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.organizeTitleLabel.TextStyle = RetroArcadeUI.ArcadeLabel.ArcadeTextStyle.Normal;
            // 
            // organizationMethodGroup
            // 
            this.organizationMethodGroup.Controls.Add(this.alphabeticalRadio);
            this.organizationMethodGroup.Controls.Add(this.genreRadio);
            this.organizationMethodGroup.Controls.Add(this.consoleRadio);
            this.organizationMethodGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.organizationMethodGroup.GlowBorder = false;
            this.organizationMethodGroup.HeaderText = "Organizational Method";
            this.organizationMethodGroup.Location = new System.Drawing.Point(20, 19);
            this.organizationMethodGroup.Name = "organizationMethodGroup";
            this.organizationMethodGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.organizationMethodGroup.ScanLines = false;
            this.organizationMethodGroup.Size = new System.Drawing.Size(400, 150);
            this.organizationMethodGroup.TabIndex = 1;
            // 
            // alphabeticalRadio
            // 
            this.alphabeticalRadio.BackColor = System.Drawing.Color.Transparent;
            this.alphabeticalRadio.Checked = true;
            this.alphabeticalRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.alphabeticalRadio.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.alphabeticalRadio.ForeColor = System.Drawing.Color.White;
            this.alphabeticalRadio.Glowing = false;
            this.alphabeticalRadio.Location = new System.Drawing.Point(20, 45);
            this.alphabeticalRadio.Name = "alphabeticalRadio";
            this.alphabeticalRadio.Size = new System.Drawing.Size(200, 25);
            this.alphabeticalRadio.TabIndex = 0;
            this.alphabeticalRadio.Text = "Alphabetical (A-Z)";
            // 
            // genreRadio
            // 
            this.genreRadio.BackColor = System.Drawing.Color.Transparent;
            this.genreRadio.Checked = false;
            this.genreRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.genreRadio.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.genreRadio.ForeColor = System.Drawing.Color.White;
            this.genreRadio.Glowing = false;
            this.genreRadio.Location = new System.Drawing.Point(20, 75);
            this.genreRadio.Name = "genreRadio";
            this.genreRadio.Size = new System.Drawing.Size(250, 25);
            this.genreRadio.TabIndex = 1;
            this.genreRadio.Text = "By Genre (from XML)";
            // 
            // consoleRadio
            // 
            this.consoleRadio.BackColor = System.Drawing.Color.Transparent;
            this.consoleRadio.Checked = false;
            this.consoleRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.consoleRadio.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.consoleRadio.ForeColor = System.Drawing.Color.White;
            this.consoleRadio.Glowing = false;
            this.consoleRadio.Location = new System.Drawing.Point(20, 105);
            this.consoleRadio.Name = "consoleRadio";
            this.consoleRadio.Size = new System.Drawing.Size(300, 25);
            this.consoleRadio.TabIndex = 2;
            this.consoleRadio.Text = "By Console (by file extension)";
            // 
            // actionOptionsGroup
            // 
            this.actionOptionsGroup.Controls.Add(this.moveRomsRadio);
            this.actionOptionsGroup.Controls.Add(this.copyRomsRadio);
            this.actionOptionsGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.actionOptionsGroup.GlowBorder = false;
            this.actionOptionsGroup.HeaderText = "Action Options";
            this.actionOptionsGroup.Location = new System.Drawing.Point(437, 19);
            this.actionOptionsGroup.Name = "actionOptionsGroup";
            this.actionOptionsGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.actionOptionsGroup.ScanLines = false;
            this.actionOptionsGroup.Size = new System.Drawing.Size(364, 150);
            this.actionOptionsGroup.TabIndex = 2;
            // 
            // moveRomsRadio
            // 
            this.moveRomsRadio.BackColor = System.Drawing.Color.Transparent;
            this.moveRomsRadio.Checked = false;
            this.moveRomsRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.moveRomsRadio.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.moveRomsRadio.ForeColor = System.Drawing.Color.White;
            this.moveRomsRadio.Glowing = false;
            this.moveRomsRadio.Location = new System.Drawing.Point(20, 45);
            this.moveRomsRadio.Name = "moveRomsRadio";
            this.moveRomsRadio.Size = new System.Drawing.Size(150, 25);
            this.moveRomsRadio.TabIndex = 0;
            this.moveRomsRadio.Text = "Move ROMs";
            // 
            // copyRomsRadio
            // 
            this.copyRomsRadio.BackColor = System.Drawing.Color.Transparent;
            this.copyRomsRadio.Checked = true;
            this.copyRomsRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.copyRomsRadio.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.copyRomsRadio.ForeColor = System.Drawing.Color.White;
            this.copyRomsRadio.Glowing = false;
            this.copyRomsRadio.Location = new System.Drawing.Point(190, 45);
            this.copyRomsRadio.Name = "copyRomsRadio";
            this.copyRomsRadio.Size = new System.Drawing.Size(150, 25);
            this.copyRomsRadio.TabIndex = 1;
            this.copyRomsRadio.Text = "Copy ROMs";
            // 
            // outputDirectoryGroup
            // 
            this.outputDirectoryGroup.Controls.Add(this.outputDirectoryLabel);
            this.outputDirectoryGroup.Controls.Add(this.outputDirectoryTextBox);
            this.outputDirectoryGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.outputDirectoryGroup.GlowBorder = false;
            this.outputDirectoryGroup.HeaderText = "";
            this.outputDirectoryGroup.Location = new System.Drawing.Point(20, 194);
            this.outputDirectoryGroup.Name = "outputDirectoryGroup";
            this.outputDirectoryGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.outputDirectoryGroup.ScanLines = false;
            this.outputDirectoryGroup.Size = new System.Drawing.Size(781, 80);
            this.outputDirectoryGroup.TabIndex = 3;
            // 
            // outputDirectoryLabel
            // 
            this.outputDirectoryLabel.BackColor = System.Drawing.Color.Transparent;
            this.outputDirectoryLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.outputDirectoryLabel.ForeColor = System.Drawing.Color.White;
            this.outputDirectoryLabel.Glowing = false;
            this.outputDirectoryLabel.Location = new System.Drawing.Point(20, 30);
            this.outputDirectoryLabel.Name = "outputDirectoryLabel";
            this.outputDirectoryLabel.Size = new System.Drawing.Size(130, 30);
            this.outputDirectoryLabel.TabIndex = 0;
            this.outputDirectoryLabel.Text = "Output directory:";
            this.outputDirectoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputDirectoryLabel.TextStyle = RetroArcadeUI.ArcadeLabel.ArcadeTextStyle.Normal;
            // 
            // outputDirectoryTextBox
            // 
            this.outputDirectoryTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.outputDirectoryTextBox.Glowing = false;
            this.outputDirectoryTextBox.Location = new System.Drawing.Point(160, 30);
            this.outputDirectoryTextBox.Name = "outputDirectoryTextBox";
            this.outputDirectoryTextBox.ReadOnly = true;
            this.outputDirectoryTextBox.Size = new System.Drawing.Size(475, 30);
            this.outputDirectoryTextBox.TabIndex = 1;
            this.outputDirectoryTextBox.Text = "Choose a folder for organized ROMs...";
            // 
            // outputBrowseButton
            // 
            this.outputBrowseButton.BackColor = System.Drawing.Color.Transparent;
            this.outputBrowseButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(144)))), ((int)(((byte)(220)))));
            this.outputBrowseButton.CrispText = true;
            this.outputBrowseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.outputBrowseButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.outputBrowseButton.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.outputBrowseButton.Glowing = false;
            this.outputBrowseButton.Location = new System.Drawing.Point(677, 224);
            this.outputBrowseButton.Name = "outputBrowseButton";
            this.outputBrowseButton.Size = new System.Drawing.Size(100, 30);
            this.outputBrowseButton.TabIndex = 2;
            this.outputBrowseButton.Text = "Browse";
            this.outputBrowseButton.TextColor = System.Drawing.Color.White;
            this.outputBrowseButton.Click += new System.EventHandler(this.outputBrowseButton_Click);
            // 
            // organizeButton
            // 
            this.organizeButton.BackColor = System.Drawing.Color.Transparent;
            this.organizeButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.organizeButton.CrispText = true;
            this.organizeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.organizeButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.organizeButton.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.organizeButton.Glowing = false;
            this.organizeButton.Location = new System.Drawing.Point(20, 308);
            this.organizeButton.Name = "organizeButton";
            this.organizeButton.Size = new System.Drawing.Size(180, 40);
            this.organizeButton.TabIndex = 4;
            this.organizeButton.Text = "Organize ROMs";
            this.organizeButton.TextColor = System.Drawing.Color.White;
            this.organizeButton.Click += new System.EventHandler(this.organizeButton_Click);
            // 
            // organizeOutputGroup
            // 
            this.organizeOutputGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.organizeOutputGroup.GlowBorder = false;
            this.organizeOutputGroup.HeaderText = "Organize Output";
            this.organizeOutputGroup.Location = new System.Drawing.Point(60, 444);
            this.organizeOutputGroup.Name = "organizeOutputGroup";
            this.organizeOutputGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.organizeOutputGroup.ScanLines = false;
            this.organizeOutputGroup.Size = new System.Drawing.Size(1200, 329);
            this.organizeOutputGroup.TabIndex = 5;
            // 
            // organizeConsoleOutput
            // 
            this.organizeConsoleOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.organizeConsoleOutput.Location = new System.Drawing.Point(66, 479);
            this.organizeConsoleOutput.Name = "organizeConsoleOutput";
            this.organizeConsoleOutput.Size = new System.Drawing.Size(1188, 284);
            this.organizeConsoleOutput.TabIndex = 0;
            this.organizeConsoleOutput.Text = "Ready to organize ROMs...";
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.tabPage5.Controls.Add(this.cleanMainPanel);
            this.tabPage5.Location = new System.Drawing.Point(4, 39);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1389, 803);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Clean ROMs";
            // 
            // cleanMainPanel
            // 
            this.cleanMainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.cleanMainPanel.Controls.Add(this.moveDuplicatesCheckBox);
            this.cleanMainPanel.Controls.Add(this.cleanTitleLabel);
            this.cleanMainPanel.Controls.Add(this.renameGroup);
            this.cleanMainPanel.Controls.Add(this.removeDuplicatesButton);
            this.cleanMainPanel.Controls.Add(this.scanDuplicatesButton);
            this.cleanMainPanel.Controls.Add(this.deduplicateGroup);
            this.cleanMainPanel.Controls.Add(this.cleanOutputGroup);
            this.cleanMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cleanMainPanel.GlowBorder = true;
            this.cleanMainPanel.Location = new System.Drawing.Point(3, 3);
            this.cleanMainPanel.Name = "cleanMainPanel";
            this.cleanMainPanel.ScanLines = true;
            this.cleanMainPanel.Size = new System.Drawing.Size(1383, 797);
            this.cleanMainPanel.TabIndex = 0;
            // 
            // moveDuplicatesCheckBox
            // 
            this.moveDuplicatesCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.moveDuplicatesCheckBox.Checked = true;
            this.moveDuplicatesCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.moveDuplicatesCheckBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.moveDuplicatesCheckBox.ForeColor = System.Drawing.Color.White;
            this.moveDuplicatesCheckBox.Glowing = false;
            this.moveDuplicatesCheckBox.Location = new System.Drawing.Point(995, 195);
            this.moveDuplicatesCheckBox.Name = "moveDuplicatesCheckBox";
            this.moveDuplicatesCheckBox.Size = new System.Drawing.Size(230, 35);
            this.moveDuplicatesCheckBox.TabIndex = 0;
            this.moveDuplicatesCheckBox.Text = "Move duplicates to separate folder";
            // 
            // cleanTitleLabel
            // 
            this.cleanTitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.cleanTitleLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.cleanTitleLabel.ForeColor = System.Drawing.Color.White;
            this.cleanTitleLabel.Glowing = false;
            this.cleanTitleLabel.Location = new System.Drawing.Point(60, 30);
            this.cleanTitleLabel.Name = "cleanTitleLabel";
            this.cleanTitleLabel.Size = new System.Drawing.Size(400, 50);
            this.cleanTitleLabel.TabIndex = 0;
            this.cleanTitleLabel.Text = "Clean & Optimize ROMs";
            this.cleanTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cleanTitleLabel.TextStyle = RetroArcadeUI.ArcadeLabel.ArcadeTextStyle.Normal;
            // 
            // renameGroup
            // 
            this.renameGroup.Controls.Add(this.renameHeaderLabel);
            this.renameGroup.Controls.Add(this.namingConventionGroup);
            this.renameGroup.Controls.Add(this.renameButton);
            this.renameGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.renameGroup.GlowBorder = false;
            this.renameGroup.HeaderText = "";
            this.renameGroup.Location = new System.Drawing.Point(60, 100);
            this.renameGroup.Name = "renameGroup";
            this.renameGroup.Padding = new System.Windows.Forms.Padding(10);
            this.renameGroup.ScanLines = false;
            this.renameGroup.Size = new System.Drawing.Size(580, 280);
            this.renameGroup.TabIndex = 1;
            // 
            // renameHeaderLabel
            // 
            this.renameHeaderLabel.BackColor = System.Drawing.Color.Transparent;
            this.renameHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.renameHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.renameHeaderLabel.Glowing = false;
            this.renameHeaderLabel.Location = new System.Drawing.Point(20, 15);
            this.renameHeaderLabel.Name = "renameHeaderLabel";
            this.renameHeaderLabel.Size = new System.Drawing.Size(250, 30);
            this.renameHeaderLabel.TabIndex = 0;
            this.renameHeaderLabel.Text = "[R] Rename ROMs";
            this.renameHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.renameHeaderLabel.TextStyle = RetroArcadeUI.ArcadeLabel.ArcadeTextStyle.Normal;
            // 
            // namingConventionGroup
            // 
            this.namingConventionGroup.Controls.Add(this.removeTagsRadio);
            this.namingConventionGroup.Controls.Add(this.standardFormatRadio);
            this.namingConventionGroup.Controls.Add(this.customFormatRadio);
            this.namingConventionGroup.Controls.Add(this.customFormatTextBox);
            this.namingConventionGroup.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.namingConventionGroup.GlowBorder = false;
            this.namingConventionGroup.HeaderText = "Naming Convention";
            this.namingConventionGroup.Location = new System.Drawing.Point(20, 55);
            this.namingConventionGroup.Name = "namingConventionGroup";
            this.namingConventionGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.namingConventionGroup.ScanLines = false;
            this.namingConventionGroup.Size = new System.Drawing.Size(540, 170);
            this.namingConventionGroup.TabIndex = 1;
            // 
            // removeTagsRadio
            // 
            this.removeTagsRadio.BackColor = System.Drawing.Color.Transparent;
            this.removeTagsRadio.Checked = true;
            this.removeTagsRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.removeTagsRadio.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.removeTagsRadio.ForeColor = System.Drawing.Color.White;
            this.removeTagsRadio.Glowing = false;
            this.removeTagsRadio.Location = new System.Drawing.Point(20, 40);
            this.removeTagsRadio.Name = "removeTagsRadio";
            this.removeTagsRadio.Size = new System.Drawing.Size(300, 25);
            this.removeTagsRadio.TabIndex = 0;
            this.removeTagsRadio.Text = "Remove region/language tags";
            // 
            // standardFormatRadio
            // 
            this.standardFormatRadio.BackColor = System.Drawing.Color.Transparent;
            this.standardFormatRadio.Checked = false;
            this.standardFormatRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.standardFormatRadio.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.standardFormatRadio.ForeColor = System.Drawing.Color.White;
            this.standardFormatRadio.Glowing = false;
            this.standardFormatRadio.Location = new System.Drawing.Point(20, 70);
            this.standardFormatRadio.Name = "standardFormatRadio";
            this.standardFormatRadio.Size = new System.Drawing.Size(300, 25);
            this.standardFormatRadio.TabIndex = 1;
            this.standardFormatRadio.Text = "Standard format: Game Name (Region)";
            // 
            // customFormatRadio
            // 
            this.customFormatRadio.BackColor = System.Drawing.Color.Transparent;
            this.customFormatRadio.Checked = false;
            this.customFormatRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.customFormatRadio.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.customFormatRadio.ForeColor = System.Drawing.Color.White;
            this.customFormatRadio.Glowing = false;
            this.customFormatRadio.Location = new System.Drawing.Point(20, 100);
            this.customFormatRadio.Name = "customFormatRadio";
            this.customFormatRadio.Size = new System.Drawing.Size(150, 25);
            this.customFormatRadio.TabIndex = 2;
            this.customFormatRadio.Text = "Custom format:";
            // 
            // customFormatTextBox
            // 
            this.customFormatTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.customFormatTextBox.Glowing = false;
            this.customFormatTextBox.Location = new System.Drawing.Point(40, 130);
            this.customFormatTextBox.Name = "customFormatTextBox";
            this.customFormatTextBox.ReadOnly = false;
            this.customFormatTextBox.Size = new System.Drawing.Size(480, 25);
            this.customFormatTextBox.TabIndex = 3;
            this.customFormatTextBox.Text = "e.g., {name} [{region}] ({year})";
            // 
            // renameButton
            // 
            this.renameButton.BackColor = System.Drawing.Color.Transparent;
            this.renameButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(144)))), ((int)(((byte)(220)))));
            this.renameButton.CrispText = true;
            this.renameButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.renameButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.renameButton.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.renameButton.Glowing = false;
            this.renameButton.Location = new System.Drawing.Point(20, 235);
            this.renameButton.Name = "renameButton";
            this.renameButton.Size = new System.Drawing.Size(150, 35);
            this.renameButton.TabIndex = 2;
            this.renameButton.Text = "[R] Apply Renames";
            this.renameButton.TextColor = System.Drawing.Color.White;
            // 
            // removeDuplicatesButton
            // 
            this.removeDuplicatesButton.BackColor = System.Drawing.Color.Transparent;
            this.removeDuplicatesButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.removeDuplicatesButton.CrispText = true;
            this.removeDuplicatesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.removeDuplicatesButton.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.removeDuplicatesButton.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.removeDuplicatesButton.Glowing = false;
            this.removeDuplicatesButton.Location = new System.Drawing.Point(995, 335);
            this.removeDuplicatesButton.Name = "removeDuplicatesButton";
            this.removeDuplicatesButton.Size = new System.Drawing.Size(170, 35);
            this.removeDuplicatesButton.TabIndex = 4;
            this.removeDuplicatesButton.Text = "[X] Remove Duplicates";
            this.removeDuplicatesButton.TextColor = System.Drawing.Color.White;
            // 
            // scanDuplicatesButton
            // 
            this.scanDuplicatesButton.BackColor = System.Drawing.Color.Transparent;
            this.scanDuplicatesButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(144)))), ((int)(((byte)(220)))));
            this.scanDuplicatesButton.CrispText = true;
            this.scanDuplicatesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.scanDuplicatesButton.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.scanDuplicatesButton.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.scanDuplicatesButton.Glowing = false;
            this.scanDuplicatesButton.Location = new System.Drawing.Point(695, 335);
            this.scanDuplicatesButton.Name = "scanDuplicatesButton";
            this.scanDuplicatesButton.Size = new System.Drawing.Size(160, 35);
            this.scanDuplicatesButton.TabIndex = 3;
            this.scanDuplicatesButton.Text = "[S] Scan for Duplicates";
            this.scanDuplicatesButton.TextColor = System.Drawing.Color.White;
            // 
            // deduplicateGroup
            // 
            this.deduplicateGroup.Controls.Add(this.deduplicateHeaderLabel);
            this.deduplicateGroup.Controls.Add(this.detectionMethodGroup);
            this.deduplicateGroup.Controls.Add(this.duplicateHandlingGroup);
            this.deduplicateGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.deduplicateGroup.GlowBorder = false;
            this.deduplicateGroup.HeaderText = "";
            this.deduplicateGroup.Location = new System.Drawing.Point(660, 100);
            this.deduplicateGroup.Name = "deduplicateGroup";
            this.deduplicateGroup.Padding = new System.Windows.Forms.Padding(10);
            this.deduplicateGroup.ScanLines = false;
            this.deduplicateGroup.Size = new System.Drawing.Size(600, 280);
            this.deduplicateGroup.TabIndex = 2;
            // 
            // deduplicateHeaderLabel
            // 
            this.deduplicateHeaderLabel.BackColor = System.Drawing.Color.Transparent;
            this.deduplicateHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.deduplicateHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.deduplicateHeaderLabel.Glowing = false;
            this.deduplicateHeaderLabel.Location = new System.Drawing.Point(20, 15);
            this.deduplicateHeaderLabel.Name = "deduplicateHeaderLabel";
            this.deduplicateHeaderLabel.Size = new System.Drawing.Size(300, 30);
            this.deduplicateHeaderLabel.TabIndex = 0;
            this.deduplicateHeaderLabel.Text = "[D] Remove Duplicates";
            this.deduplicateHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deduplicateHeaderLabel.TextStyle = RetroArcadeUI.ArcadeLabel.ArcadeTextStyle.Normal;
            // 
            // detectionMethodGroup
            // 
            this.detectionMethodGroup.Controls.Add(this.fileHashRadio);
            this.detectionMethodGroup.Controls.Add(this.fileSizeNameRadio);
            this.detectionMethodGroup.Controls.Add(this.nameSimilarityRadio);
            this.detectionMethodGroup.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.detectionMethodGroup.GlowBorder = false;
            this.detectionMethodGroup.HeaderText = "Detection Method";
            this.detectionMethodGroup.Location = new System.Drawing.Point(20, 55);
            this.detectionMethodGroup.Name = "detectionMethodGroup";
            this.detectionMethodGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.detectionMethodGroup.ScanLines = false;
            this.detectionMethodGroup.Size = new System.Drawing.Size(280, 120);
            this.detectionMethodGroup.TabIndex = 1;
            // 
            // fileHashRadio
            // 
            this.fileHashRadio.BackColor = System.Drawing.Color.Transparent;
            this.fileHashRadio.Checked = true;
            this.fileHashRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.fileHashRadio.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.fileHashRadio.ForeColor = System.Drawing.Color.White;
            this.fileHashRadio.Glowing = false;
            this.fileHashRadio.Location = new System.Drawing.Point(15, 40);
            this.fileHashRadio.Name = "fileHashRadio";
            this.fileHashRadio.Size = new System.Drawing.Size(250, 35);
            this.fileHashRadio.TabIndex = 0;
            this.fileHashRadio.Text = "File hash (MD5/SHA1) — Most accurate";
            // 
            // fileSizeNameRadio
            // 
            this.fileSizeNameRadio.BackColor = System.Drawing.Color.Transparent;
            this.fileSizeNameRadio.Checked = false;
            this.fileSizeNameRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.fileSizeNameRadio.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.fileSizeNameRadio.ForeColor = System.Drawing.Color.White;
            this.fileSizeNameRadio.Glowing = false;
            this.fileSizeNameRadio.Location = new System.Drawing.Point(15, 75);
            this.fileSizeNameRadio.Name = "fileSizeNameRadio";
            this.fileSizeNameRadio.Size = new System.Drawing.Size(250, 25);
            this.fileSizeNameRadio.TabIndex = 1;
            this.fileSizeNameRadio.Text = "File size and name comparison";
            // 
            // nameSimilarityRadio
            // 
            this.nameSimilarityRadio.BackColor = System.Drawing.Color.Transparent;
            this.nameSimilarityRadio.Checked = false;
            this.nameSimilarityRadio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.nameSimilarityRadio.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.nameSimilarityRadio.ForeColor = System.Drawing.Color.White;
            this.nameSimilarityRadio.Glowing = false;
            this.nameSimilarityRadio.Location = new System.Drawing.Point(15, 100);
            this.nameSimilarityRadio.Name = "nameSimilarityRadio";
            this.nameSimilarityRadio.Size = new System.Drawing.Size(250, 25);
            this.nameSimilarityRadio.TabIndex = 2;
            this.nameSimilarityRadio.Text = "Name similarity only";
            // 
            // duplicateHandlingGroup
            // 
            this.duplicateHandlingGroup.Controls.Add(this.keepBestVersionCheckBox);
            this.duplicateHandlingGroup.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.duplicateHandlingGroup.GlowBorder = false;
            this.duplicateHandlingGroup.HeaderText = "Duplicate Handling";
            this.duplicateHandlingGroup.Location = new System.Drawing.Point(320, 55);
            this.duplicateHandlingGroup.Name = "duplicateHandlingGroup";
            this.duplicateHandlingGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.duplicateHandlingGroup.ScanLines = false;
            this.duplicateHandlingGroup.Size = new System.Drawing.Size(260, 120);
            this.duplicateHandlingGroup.TabIndex = 2;
            // 
            // keepBestVersionCheckBox
            // 
            this.keepBestVersionCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.keepBestVersionCheckBox.Checked = true;
            this.keepBestVersionCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.keepBestVersionCheckBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.keepBestVersionCheckBox.ForeColor = System.Drawing.Color.White;
            this.keepBestVersionCheckBox.Glowing = false;
            this.keepBestVersionCheckBox.Location = new System.Drawing.Point(15, 75);
            this.keepBestVersionCheckBox.Name = "keepBestVersionCheckBox";
            this.keepBestVersionCheckBox.Size = new System.Drawing.Size(230, 50);
            this.keepBestVersionCheckBox.TabIndex = 1;
            this.keepBestVersionCheckBox.Text = "Keep best version (prefer no-intro/redump)";
            // 
            // cleanOutputGroup
            // 
            this.cleanOutputGroup.Controls.Add(this.cleanConsoleOutput);
            this.cleanOutputGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.cleanOutputGroup.GlowBorder = false;
            this.cleanOutputGroup.HeaderText = "Clean & Optimize Output";
            this.cleanOutputGroup.Location = new System.Drawing.Point(60, 400);
            this.cleanOutputGroup.Name = "cleanOutputGroup";
            this.cleanOutputGroup.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.cleanOutputGroup.ScanLines = false;
            this.cleanOutputGroup.Size = new System.Drawing.Size(1200, 330);
            this.cleanOutputGroup.TabIndex = 3;
            // 
            // cleanConsoleOutput
            // 
            this.cleanConsoleOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cleanConsoleOutput.Location = new System.Drawing.Point(20, 40);
            this.cleanConsoleOutput.Name = "cleanConsoleOutput";
            this.cleanConsoleOutput.Size = new System.Drawing.Size(1160, 270);
            this.cleanConsoleOutput.TabIndex = 0;
            this.cleanConsoleOutput.Text = "Ready to clean and optimize ROM collection...\nSelect renaming convention or dupli" +
    "cate detection method and click the appropriate button.";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.tabPage3.Location = new System.Drawing.Point(4, 39);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1389, 803);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "View ROMs";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.tabPage4.Location = new System.Drawing.Point(4, 39);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1389, 803);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Settings";
            // 
            // retroGroupBox1
            // 
            this.retroGroupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.retroGroupBox1.Controls.Add(this.outputBrowseButton);
            this.retroGroupBox1.Controls.Add(this.outputDirectoryGroup);
            this.retroGroupBox1.Controls.Add(this.organizeButton);
            this.retroGroupBox1.Controls.Add(this.actionOptionsGroup);
            this.retroGroupBox1.Controls.Add(this.organizationMethodGroup);
            this.retroGroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.retroGroupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.retroGroupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.retroGroupBox1.Location = new System.Drawing.Point(60, 51);
            this.retroGroupBox1.Name = "retroGroupBox1";
            this.retroGroupBox1.Size = new System.Drawing.Size(1200, 369);
            this.retroGroupBox1.TabIndex = 6;
            this.retroGroupBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Controls.Add(this.arcadeFormSkin1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ROM Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.arcadeFormSkin1.ResumeLayout(false);
            this.arcadeTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.scanMainPanel.ResumeLayout(false);
            this.directoryGroup.ResumeLayout(false);
            this.optionsGroup.ResumeLayout(false);
            this.consoleGroup.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.organizeMainPanel.ResumeLayout(false);
            this.organizationMethodGroup.ResumeLayout(false);
            this.actionOptionsGroup.ResumeLayout(false);
            this.outputDirectoryGroup.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.cleanMainPanel.ResumeLayout(false);
            this.renameGroup.ResumeLayout(false);
            this.namingConventionGroup.ResumeLayout(false);
            this.deduplicateGroup.ResumeLayout(false);
            this.detectionMethodGroup.ResumeLayout(false);
            this.duplicateHandlingGroup.ResumeLayout(false);
            this.cleanOutputGroup.ResumeLayout(false);
            this.retroGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RetroArcadeUI.ArcadeFormSkin arcadeFormSkin1;
        private RetroArcadeUI.ArcadeTabControl arcadeTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;

        // ROM Scanner Controls
        private RetroArcadeUI.ArcadePanel scanMainPanel;
        private RetroArcadeUI.ArcadeLabel titleLabel;
        private RetroArcadeUI.ArcadeGroupBox directoryGroup;
        private RetroArcadeUI.ArcadeLabel directoryLabel;
        private RetroArcadeUI.ArcadeTextBox directoryTextBox;
        private RetroArcadeUI.ArcadeButton browseButton;
        private RetroArcadeUI.ArcadeGroupBox optionsGroup;
        private RetroArcadeUI.ArcadeCheckBox recursiveCheckBox;
        private RetroArcadeUI.ArcadeCheckBox metadataCheckBox;
        private RetroArcadeUI.ArcadeButton startScanButton;
        private RetroArcadeUI.ArcadeProgressBar scanProgress;
        private RetroArcadeUI.ArcadeGroupBox consoleGroup;
        private RetroArcadeUI.ArcadeScrollableTextBox consoleOutput;

        // Organize ROMs Controls
        private RetroArcadeUI.ArcadePanel organizeMainPanel;
        private RetroArcadeUI.ArcadeLabel organizeTitleLabel;
        private RetroArcadeUI.ArcadeGroupBox organizationMethodGroup;
        private RetroArcadeUI.ArcadeRadioButton alphabeticalRadio;
        private RetroArcadeUI.ArcadeRadioButton genreRadio;
        private RetroArcadeUI.ArcadeRadioButton consoleRadio;
        private RetroArcadeUI.ArcadeGroupBox actionOptionsGroup;
        private RetroArcadeUI.ArcadeRadioButton moveRomsRadio;
        private RetroArcadeUI.ArcadeRadioButton copyRomsRadio;
        private RetroArcadeUI.ArcadeGroupBox outputDirectoryGroup;
        private RetroArcadeUI.ArcadeLabel outputDirectoryLabel;
        private RetroArcadeUI.ArcadeTextBox outputDirectoryTextBox;
        private RetroArcadeUI.ArcadeButton outputBrowseButton;
        private RetroArcadeUI.ArcadeButton organizeButton;
        private RetroArcadeUI.ArcadeGroupBox organizeOutputGroup;
        private RetroArcadeUI.ArcadeScrollableTextBox organizeConsoleOutput;

        // Clean ROMs Tab Controls
        private RetroArcadeUI.ArcadePanel cleanMainPanel;
        private RetroArcadeUI.ArcadeLabel cleanTitleLabel;
        private RetroArcadeUI.ArcadeGroupBox renameGroup;
        private RetroArcadeUI.ArcadeLabel renameHeaderLabel;
        private RetroArcadeUI.ArcadeGroupBox namingConventionGroup;
        private RetroArcadeUI.ArcadeRadioButton removeTagsRadio;
        private RetroArcadeUI.ArcadeRadioButton standardFormatRadio;
        private RetroArcadeUI.ArcadeRadioButton customFormatRadio;
        private RetroArcadeUI.ArcadeTextBox customFormatTextBox;
        private RetroArcadeUI.ArcadeButton renameButton;
        private RetroArcadeUI.ArcadeGroupBox deduplicateGroup;
        private RetroArcadeUI.ArcadeLabel deduplicateHeaderLabel;
        private RetroArcadeUI.ArcadeGroupBox detectionMethodGroup;
        private RetroArcadeUI.ArcadeRadioButton fileHashRadio;
        private RetroArcadeUI.ArcadeRadioButton fileSizeNameRadio;
        private RetroArcadeUI.ArcadeRadioButton nameSimilarityRadio;
        private RetroArcadeUI.ArcadeGroupBox duplicateHandlingGroup;
        private RetroArcadeUI.ArcadeCheckBox moveDuplicatesCheckBox;
        private RetroArcadeUI.ArcadeCheckBox keepBestVersionCheckBox;
        private RetroArcadeUI.ArcadeButton scanDuplicatesButton;
        private RetroArcadeUI.ArcadeButton removeDuplicatesButton;
        private RetroArcadeUI.ArcadeGroupBox cleanOutputGroup;
        private RetroArcadeUI.ArcadeScrollableTextBox cleanConsoleOutput;
        private RetroForms.RetroGroupBox retroGroupBox1;
    }
}