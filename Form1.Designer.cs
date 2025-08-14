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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();

            // ROM Scanner Controls for TabPage1
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

            this.arcadeFormSkin1.SuspendLayout();
            this.arcadeTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.scanMainPanel.SuspendLayout();
            this.directoryGroup.SuspendLayout();
            this.optionsGroup.SuspendLayout();
            this.consoleGroup.SuspendLayout();
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
            this.arcadeFormSkin1.Font = new System.Drawing.Font("Orbitron", 12F, System.Drawing.FontStyle.Bold);
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
            this.arcadeTabControl1.Controls.Add(this.tabPage3);
            this.arcadeTabControl1.Controls.Add(this.tabPage4);
            this.arcadeTabControl1.Font = new System.Drawing.Font("Orbitron", 10F, System.Drawing.FontStyle.Bold);
            this.arcadeTabControl1.ItemSize = new System.Drawing.Size(120, 35);
            this.arcadeTabControl1.Location = new System.Drawing.Point(0, 51);
            this.arcadeTabControl1.Name = "arcadeTabControl1";
            this.arcadeTabControl1.Padding = new System.Drawing.Point(0, 0);
            this.arcadeTabControl1.SelectedIndex = 0;
            this.arcadeTabControl1.Size = new System.Drawing.Size(1397, 846);
            this.arcadeTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.arcadeTabControl1.TabIndex = 0;

            // 
            // tabPage1 - Scan ROM Tab
            // 
            this.tabPage1.Controls.Add(this.scanMainPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1389, 803);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Scan ROM";
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));

            // 
            // scanMainPanel
            // 
            this.scanMainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.scanMainPanel.Controls.Add(this.titleLabel);
            this.scanMainPanel.Controls.Add(this.directoryGroup);
            this.scanMainPanel.Controls.Add(this.optionsGroup);
            this.scanMainPanel.Controls.Add(this.startScanButton);
            this.scanMainPanel.Controls.Add(this.scanProgress);
            this.scanMainPanel.Controls.Add(this.consoleGroup);
            this.scanMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scanMainPanel.GlowBorder = true;
            this.scanMainPanel.Name = "scanMainPanel";
            this.scanMainPanel.ScanLines = true;
            this.scanMainPanel.TabIndex = 0;

            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Glowing = false;
            this.titleLabel.Location = new System.Drawing.Point(60, 30);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(400, 50);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Scan & Index ROMs";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.titleLabel.TextStyle = RetroArcadeUI.ArcadeLabel.ArcadeTextStyle.Normal;

            // 
            // directoryGroup
            // 
            this.directoryGroup.Controls.Add(this.directoryLabel);
            this.directoryGroup.Controls.Add(this.directoryTextBox);
            this.directoryGroup.Controls.Add(this.browseButton);
            this.directoryGroup.HeaderText = "";
            this.directoryGroup.Location = new System.Drawing.Point(60, 100);
            this.directoryGroup.Name = "directoryGroup";
            this.directoryGroup.ScanLines = false;
            this.directoryGroup.Size = new System.Drawing.Size(1200, 80);
            this.directoryGroup.TabIndex = 1;
            this.directoryGroup.GlowBorder = false;

            // 
            // directoryLabel
            // 
            this.directoryLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.directoryLabel.ForeColor = System.Drawing.Color.White;
            this.directoryLabel.Location = new System.Drawing.Point(20, 30);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(130, 30);
            this.directoryLabel.TabIndex = 0;
            this.directoryLabel.Text = "ROM Directory:";
            this.directoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.directoryLabel.Glowing = false;

            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.directoryTextBox.Glowing = false;
            this.directoryTextBox.Location = new System.Drawing.Point(160, 30);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(800, 30);
            this.directoryTextBox.TabIndex = 1;
            this.directoryTextBox.Text = "Choose a folder...";

            // 
            // browseButton
            // 
            this.browseButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(144)))), ((int)(((byte)(220)))));
            this.browseButton.CrispText = true;
            this.browseButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            this.browseButton.Glowing = false;
            this.browseButton.Location = new System.Drawing.Point(980, 30);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(100, 30);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse";
            this.browseButton.TextColor = System.Drawing.Color.White;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);

            // 
            // optionsGroup
            // 
            this.optionsGroup.Controls.Add(this.recursiveCheckBox);
            this.optionsGroup.Controls.Add(this.metadataCheckBox);
            this.optionsGroup.HeaderText = "Scan Options";
            this.optionsGroup.Location = new System.Drawing.Point(60, 200);
            this.optionsGroup.Name = "optionsGroup";
            this.optionsGroup.ScanLines = false;
            this.optionsGroup.Size = new System.Drawing.Size(400, 120);
            this.optionsGroup.TabIndex = 2;
            this.optionsGroup.GlowBorder = false;

            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Checked = true;
            this.recursiveCheckBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.recursiveCheckBox.Glowing = false;
            this.recursiveCheckBox.Location = new System.Drawing.Point(20, 45);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(350, 25);
            this.recursiveCheckBox.TabIndex = 0;
            this.recursiveCheckBox.Text = "Scan subdirectories recursively";
            this.recursiveCheckBox.ForeColor = System.Drawing.Color.White;

            // 
            // metadataCheckBox
            // 
            this.metadataCheckBox.Checked = true;
            this.metadataCheckBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.metadataCheckBox.Glowing = false;
            this.metadataCheckBox.Location = new System.Drawing.Point(20, 80);
            this.metadataCheckBox.Name = "metadataCheckBox";
            this.metadataCheckBox.Size = new System.Drawing.Size(200, 25);
            this.metadataCheckBox.TabIndex = 1;
            this.metadataCheckBox.Text = "Extract metadata";
            this.metadataCheckBox.ForeColor = System.Drawing.Color.White;

            // 
            // startScanButton
            // 
            this.startScanButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(144)))), ((int)(((byte)(220)))));
            this.startScanButton.CrispText = true;
            this.startScanButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.startScanButton.Glowing = false;
            this.startScanButton.Location = new System.Drawing.Point(60, 340);
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
            this.scanProgress.Location = new System.Drawing.Point(230, 350);
            this.scanProgress.Name = "scanProgress";
            this.scanProgress.Size = new System.Drawing.Size(400, 20);
            this.scanProgress.TabIndex = 4;
            this.scanProgress.Value = 0;
            this.scanProgress.Visible = false;

            // 
            // consoleGroup
            // 
            this.consoleGroup.Controls.Add(this.consoleOutput);
            this.consoleGroup.HeaderText = "Scan Output";
            this.consoleGroup.Location = new System.Drawing.Point(60, 400);
            this.consoleGroup.Name = "consoleGroup";
            this.consoleGroup.ScanLines = false;
            this.consoleGroup.Size = new System.Drawing.Size(1200, 330);
            this.consoleGroup.TabIndex = 5;
            this.consoleGroup.GlowBorder = false;

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
            this.tabPage2.Location = new System.Drawing.Point(4, 39);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1386, 803);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sort ROMs";
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));

            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 39);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1386, 803);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "View ROMs";
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));

            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 39);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1386, 803);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Settings";
            this.tabPage4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));

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
            this.ResumeLayout(false);
        }

        #endregion

        private RetroArcadeUI.ArcadeFormSkin arcadeFormSkin1;
        private RetroArcadeUI.ArcadeTabControl arcadeTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;

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
    }
}