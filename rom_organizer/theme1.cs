using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RetroArcadeUI
{
    public class ArcadeButton : Control
    {
        private int W;
        private int H;
        private bool _Glowing = true;
        private bool _CrispText = true; // New property for crisp text rendering
        private MouseState State = MouseState.None;
        private Timer glowTimer;
        private int glowIntensity = 0;
        private int glowDirection = 1;

        [Category("Colors")]
        public Color BaseColor
        {
            get { return _BaseColor; }
            set { _BaseColor = value; }
        }

        public enum MouseState : byte
        {
            None = 0,
            Over = 1,
            Down = 2,
            Block = 3
        }

        [Category("Colors")]
        public Color TextColor
        {
            get { return _TextColor; }
            set { _TextColor = value; }
        }

        [Category("Colors")]
        public Color GlowColor
        {
            get { return _GlowColor; }
            set { _GlowColor = value; }
        }

        [Category("Options")]
        public bool Glowing
        {
            get { return _Glowing; }
            set
            {
                _Glowing = value;
                if (value && glowTimer == null)
                {
                    glowTimer = new Timer();
                    glowTimer.Interval = 50;
                    glowTimer.Tick += GlowTimer_Tick;
                    glowTimer.Start();
                }
                else if (!value && glowTimer != null)
                {
                    glowTimer.Stop();
                    glowTimer.Dispose();
                    glowTimer = null;
                }
            }
        }

        [Category("Options")]
        public bool CrispText
        {
            get { return _CrispText; }
            set { _CrispText = value; Invalidate(); }
        }

        private void GlowTimer_Tick(object sender, EventArgs e)
        {
            glowIntensity += glowDirection * 5;
            if (glowIntensity >= 50) glowDirection = -1;
            if (glowIntensity <= 0) glowDirection = 1;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            State = MouseState.Down;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            State = MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            State = MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            State = MouseState.None;
            Invalidate();
        }

        private Color _BaseColor = ArcadeColors.NeonCyan;
        private Color _TextColor = Color.Black;
        private Color _GlowColor = ArcadeColors.NeonCyan;

        public ArcadeButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            Size = new Size(120, 40);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 10, FontStyle.Bold); // Changed to Segoe UI for better readability
            Cursor = Cursors.Hand;
            Glowing = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            W = Width - 1;
            H = Height - 1;

            Rectangle Base = new Rectangle(2, 2, W - 4, H - 4);

            G.SmoothingMode = SmoothingMode.AntiAlias;
            G.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Use different text rendering based on CrispText setting
            if (_CrispText)
            {
                G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            }
            else
            {
                G.TextRenderingHint = TextRenderingHint.AntiAlias;
            }

            G.Clear(BackColor);

            // Outer glow effect
            if (_Glowing)
            {
                using (GraphicsPath path = ArcadeHelpers.RoundRec(new Rectangle(0, 0, W + 1, H + 1), 8))
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = Color.FromArgb(glowIntensity, _GlowColor);
                        brush.SurroundColors = new Color[] { Color.Transparent };
                        G.FillPath(brush, path);
                    }
                }
            }

            // Main button body
            using (GraphicsPath buttonPath = ArcadeHelpers.RoundRec(Base, 6))
            {
                // Background gradient
                using (LinearGradientBrush bgBrush = new LinearGradientBrush(Base,
                    Color.FromArgb(40, ArcadeColors.DarkPurple),
                    Color.FromArgb(80, ArcadeColors.DarkPurple),
                    LinearGradientMode.Vertical))
                {
                    G.FillPath(bgBrush, buttonPath);
                }

                // Border
                Color borderColor = _BaseColor;
                int borderWidth = 2;

                switch (State)
                {
                    case MouseState.Over:
                        borderColor = ArcadeHelpers.BrightenColor(_BaseColor, 40);
                        borderWidth = 3;
                        break;
                    case MouseState.Down:
                        borderColor = ArcadeHelpers.BrightenColor(_BaseColor, 60);
                        borderWidth = 4;
                        // Slight inner shadow effect
                        using (LinearGradientBrush shadowBrush = new LinearGradientBrush(Base,
                            Color.FromArgb(50, Color.Black),
                            Color.Transparent,
                            LinearGradientMode.Vertical))
                        {
                            Rectangle shadowRect = new Rectangle(Base.X, Base.Y, Base.Width, Base.Height / 3);
                            using (GraphicsPath shadowPath = ArcadeHelpers.RoundRec(shadowRect, 6))
                            {
                                G.FillPath(shadowBrush, shadowPath);
                            }
                        }
                        break;
                }

                using (Pen borderPen = new Pen(borderColor, borderWidth))
                {
                    G.DrawPath(borderPen, buttonPath);
                }
            }

            // Text rendering - simplified for clarity
            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            if (_CrispText)
            {
                // Crisp text - no glow effects, just clean text
                using (SolidBrush textBrush = new SolidBrush(_TextColor))
                {
                    G.DrawString(Text, Font, textBrush, Base, sf);
                }
            }
            else
            {
                // Text with subtle glow (reduced for better readability)
                using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(60, _BaseColor)))
                {
                    Rectangle glowRect = new Rectangle(Base.X + 1, Base.Y + 1, Base.Width, Base.Height);
                    G.DrawString(Text, Font, glowBrush, glowRect, sf);
                }

                // Main text
                using (SolidBrush textBrush = new SolidBrush(_TextColor))
                {
                    G.DrawString(Text, Font, textBrush, Base, sf);
                }
            }

            e.Graphics.DrawImageUnscaled(B, 0, 0);
            G.Dispose();
            B.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && glowTimer != null)
            {
                glowTimer.Stop();
                glowTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class ArcadeProgressBar : Control
    {
        private int _Value = 0;
        private int _Maximum = 100;
        private bool _Animated = true;
        private Timer animTimer;
        private int animOffset = 0;

        [Category("Control")]
        public int Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value < _Value) _Value = value;
                _Maximum = value;
                Invalidate();
            }
        }

        [Category("Control")]
        public int Value
        {
            get { return _Value; }
            set
            {
                if (value > _Maximum) value = _Maximum;
                _Value = value;
                Invalidate();
            }
        }

        [Category("Options")]
        public bool Animated
        {
            get { return _Animated; }
            set
            {
                _Animated = value;
                if (value && animTimer == null)
                {
                    animTimer = new Timer();
                    animTimer.Interval = 100;
                    animTimer.Tick += AnimTimer_Tick;
                    animTimer.Start();
                }
                else if (!value && animTimer != null)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    animTimer = null;
                }
            }
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            animOffset += 2;
            if (animOffset >= 20) animOffset = 0;
            Invalidate();
        }

        public ArcadeProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            BackColor = Color.FromArgb(20, 20, 30);
            Height = 30;
            Font = new Font("Orbitron", 10, FontStyle.Bold);
            Animated = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            G.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle baseRect = new Rectangle(0, 0, Width - 1, Height - 1);
            Rectangle progressRect = new Rectangle(2, 2,
                (int)((float)_Value / _Maximum * (Width - 4)), Height - 4);

            // Background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(baseRect,
                ArcadeColors.DarkPurple,
                Color.FromArgb(40, ArcadeColors.DarkPurple),
                LinearGradientMode.Vertical))
            {
                G.FillRectangle(bgBrush, baseRect);
            }

            // Border
            using (Pen borderPen = new Pen(ArcadeColors.NeonCyan, 2))
            {
                G.DrawRectangle(borderPen, baseRect);
            }

            // Progress fill
            if (progressRect.Width > 0)
            {
                // Gradient progress bar
                using (LinearGradientBrush progressBrush = new LinearGradientBrush(progressRect,
                    ArcadeColors.NeonPink,
                    ArcadeColors.NeonCyan,
                    LinearGradientMode.Horizontal))
                {
                    G.FillRectangle(progressBrush, progressRect);
                }

                // Animated overlay pattern
                if (_Animated)
                {
                    using (HatchBrush hatchBrush = new HatchBrush(HatchStyle.ForwardDiagonal,
                        Color.FromArgb(80, Color.White),
                        Color.Transparent))
                    {
                        Rectangle animRect = new Rectangle(progressRect.X - animOffset, progressRect.Y,
                            progressRect.Width + 20, progressRect.Height);
                        G.FillRectangle(hatchBrush, animRect);
                    }
                }
            }

            // Progress text
            string progressText = $"{_Value}%";
            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                G.DrawString(progressText, Font, textBrush, baseRect, sf);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && animTimer != null)
            {
                animTimer.Stop();
                animTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class ArcadePanel : ContainerControl
    {
        private bool _ScanLines = true;
        private bool _GlowBorder = true;

        [Category("Options")]
        public bool ScanLines
        {
            get { return _ScanLines; }
            set { _ScanLines = value; Invalidate(); }
        }

        [Category("Options")]
        public bool GlowBorder
        {
            get { return _GlowBorder; }
            set { _GlowBorder = value; Invalidate(); }
        }

        public ArcadePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.FromArgb(15, 15, 25);
            Size = new Size(300, 200);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            G.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle baseRect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Background gradient
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(baseRect,
                Color.FromArgb(20, 20, 35),
                Color.FromArgb(10, 10, 20),
                LinearGradientMode.Vertical))
            {
                G.FillRectangle(bgBrush, baseRect);
            }

            // Scan lines effect
            if (_ScanLines)
            {
                using (Pen scanPen = new Pen(Color.FromArgb(10, ArcadeColors.NeonCyan), 1))
                {
                    for (int y = 0; y < Height; y += 4)
                    {
                        G.DrawLine(scanPen, 0, y, Width, y);
                    }
                }
            }

            // Glowing border
            if (_GlowBorder)
            {
                using (Pen glowPen = new Pen(ArcadeColors.NeonCyan, 2))
                {
                    G.DrawRectangle(glowPen, 1, 1, Width - 3, Height - 3);
                }

                using (Pen outerGlowPen = new Pen(Color.FromArgb(100, ArcadeColors.NeonCyan), 1))
                {
                    G.DrawRectangle(outerGlowPen, 0, 0, Width - 1, Height - 1);
                }
            }

            base.OnPaint(e);
        }
    }

    public class ArcadeLabel : Label
    {
        private bool _Glowing = false;
        private ArcadeTextStyle _TextStyle = ArcadeTextStyle.Normal;

        public enum ArcadeTextStyle
        {
            Normal,
            Neon,
            Pixelated,
            Retro
        }

        [Category("Options")]
        public bool Glowing
        {
            get { return _Glowing; }
            set { _Glowing = value; Invalidate(); }
        }

        [Category("Options")]
        public ArcadeTextStyle TextStyle
        {
            get { return _TextStyle; }
            set { _TextStyle = value; Invalidate(); }
        }

        public ArcadeLabel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Font = new Font("Orbitron", 12, FontStyle.Bold);
            ForeColor = ArcadeColors.NeonCyan;
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle textRect = new Rectangle(0, 0, Width, Height);
            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            switch (_TextStyle)
            {
                case ArcadeTextStyle.Neon:
                    DrawNeonText(G, textRect, sf);
                    break;
                case ArcadeTextStyle.Pixelated:
                    G.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                    DrawGlowingText(G, textRect, sf);
                    break;
                case ArcadeTextStyle.Retro:
                    DrawRetroText(G, textRect, sf);
                    break;
                default:
                    if (_Glowing)
                        DrawGlowingText(G, textRect, sf);
                    else
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), textRect, sf);
                    break;
            }
        }

        private void DrawNeonText(Graphics g, Rectangle rect, StringFormat sf)
        {
            // Multiple glow layers for neon effect
            Color[] glowColors = {
                Color.FromArgb(30, ForeColor),
                Color.FromArgb(60, ForeColor),
                Color.FromArgb(90, ForeColor)
            };

            for (int i = 0; i < glowColors.Length; i++)
            {
                using (SolidBrush glowBrush = new SolidBrush(glowColors[i]))
                {
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            if (x == 0 && y == 0) continue;
                            Rectangle glowRect = new Rectangle(rect.X + x * (i + 1), rect.Y + y * (i + 1), rect.Width, rect.Height);
                            g.DrawString(Text, Font, glowBrush, glowRect, sf);
                        }
                    }
                }
            }

            // Main text
            using (SolidBrush mainBrush = new SolidBrush(Color.White))
            {
                g.DrawString(Text, Font, mainBrush, rect, sf);
            }
        }

        private void DrawGlowingText(Graphics g, Rectangle rect, StringFormat sf)
        {
            // Simple glow effect
            using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(100, ForeColor)))
            {
                for (int i = 1; i <= 2; i++)
                {
                    Rectangle glowRect = new Rectangle(rect.X - i, rect.Y - i, rect.Width + i * 2, rect.Height + i * 2);
                    g.DrawString(Text, Font, glowBrush, glowRect, sf);
                }
            }

            using (SolidBrush mainBrush = new SolidBrush(ForeColor))
            {
                g.DrawString(Text, Font, mainBrush, rect, sf);
            }
        }

        private void DrawRetroText(Graphics g, Rectangle rect, StringFormat sf)
        {
            // Retro 3D effect
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(80, Color.Black)))
            {
                Rectangle shadowRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                g.DrawString(Text, Font, shadowBrush, shadowRect, sf);
            }

            using (SolidBrush mainBrush = new SolidBrush(ForeColor))
            {
                g.DrawString(Text, Font, mainBrush, rect, sf);
            }
        }
    }

    // Color constants for the retro arcade theme
    public static class ArcadeColors
    {
        public static readonly Color NeonCyan = Color.FromArgb(0, 255, 255);
        public static readonly Color NeonPink = Color.FromArgb(255, 20, 147);
        public static readonly Color NeonGreen = Color.FromArgb(57, 255, 20);
        public static readonly Color NeonOrange = Color.FromArgb(255, 165, 0);
        public static readonly Color NeonPurple = Color.FromArgb(138, 43, 226);
        public static readonly Color NeonYellow = Color.FromArgb(255, 255, 0);

        public static readonly Color DarkPurple = Color.FromArgb(25, 25, 40);
        public static readonly Color DeepBlue = Color.FromArgb(10, 10, 30);
        public static readonly Color CharcoalGray = Color.FromArgb(35, 35, 45);

        // Classic arcade game inspired colors
        public static readonly Color PacManYellow = Color.FromArgb(255, 255, 0);
        public static readonly Color GhostBlue = Color.FromArgb(0, 191, 255);
        public static readonly Color SpaceInvaderGreen = Color.FromArgb(0, 255, 0);
        public static readonly Color AsteroidRed = Color.FromArgb(255, 69, 0);
    }

    // Helper methods for the arcade theme
    public static class ArcadeHelpers
    {
        public static GraphicsPath RoundRec(Rectangle rectangle, int curve)
        {
            GraphicsPath path = new GraphicsPath();
            int arcRectangleWidth = curve * 2;

            path.AddArc(new Rectangle(rectangle.X, rectangle.Y, arcRectangleWidth, arcRectangleWidth), -180, 90);
            path.AddArc(new Rectangle(rectangle.Width - arcRectangleWidth + rectangle.X, rectangle.Y, arcRectangleWidth, arcRectangleWidth), -90, 90);
            path.AddArc(new Rectangle(rectangle.Width - arcRectangleWidth + rectangle.X, rectangle.Height - arcRectangleWidth + rectangle.Y, arcRectangleWidth, arcRectangleWidth), 0, 90);
            path.AddArc(new Rectangle(rectangle.X, rectangle.Height - arcRectangleWidth + rectangle.Y, arcRectangleWidth, arcRectangleWidth), 90, 90);
            path.AddLine(new Point(rectangle.X, rectangle.Height - arcRectangleWidth + rectangle.Y), new Point(rectangle.X, curve + rectangle.Y));

            return path;
        }

        public static Color BrightenColor(Color color, int amount)
        {
            int r = Math.Min(255, color.R + amount);
            int g = Math.Min(255, color.G + amount);
            int b = Math.Min(255, color.B + amount);
            return Color.FromArgb(color.A, r, g, b);
        }

        public static Color DarkenColor(Color color, int amount)
        {
            int r = Math.Max(0, color.R - amount);
            int g = Math.Max(0, color.G - amount);
            int b = Math.Max(0, color.B - amount);
            return Color.FromArgb(color.A, r, g, b);
        }

        public static readonly StringFormat CenterSF = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public static readonly StringFormat NearSF = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near
        };
    }

    // Custom Form Theme
    public class ArcadeForm : Form
    {
        private Panel titleBar;
        private ArcadeButton closeButton, minimizeButton, maximizeButton;
        private ArcadeLabel titleLabel;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private Timer scanLineTimer;
        private int scanLineOffset = 0;

        public string FormTitle
        {
            get { return titleLabel?.Text ?? ""; }
            set { if (titleLabel != null) titleLabel.Text = value; }
        }

        public ArcadeForm()
        {
            InitializeArcadeForm();
        }

        private void InitializeArcadeForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            BackColor = ArcadeColors.DeepBlue;
            StartPosition = FormStartPosition.CenterScreen;

            // Create title bar
            titleBar = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = ArcadeColors.DarkPurple
            };

            // Title label
            titleLabel = new ArcadeLabel
            {
                Text = "RETRO ARCADE",
                ForeColor = ArcadeColors.NeonCyan,
                Font = new Font("Orbitron", 14, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Glowing = true
            };

            // Window controls
            closeButton = new ArcadeButton
            {
                Text = "×",
                Size = new Size(40, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BaseColor = ArcadeColors.AsteroidRed,
                TextColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold)
            };
            closeButton.Location = new Point(titleBar.Width - 45, 5);
            closeButton.Click += (s, e) => Close();

            maximizeButton = new ArcadeButton
            {
                Text = "□",
                Size = new Size(40, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BaseColor = ArcadeColors.NeonGreen,
                TextColor = Color.Black,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            maximizeButton.Location = new Point(titleBar.Width - 90, 5);
            maximizeButton.Click += (s, e) => WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;

            minimizeButton = new ArcadeButton
            {
                Text = "−",
                Size = new Size(40, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BaseColor = ArcadeColors.NeonYellow,
                TextColor = Color.Black,
                Font = new Font("Arial", 16, FontStyle.Bold)
            };
            minimizeButton.Location = new Point(titleBar.Width - 135, 5);
            minimizeButton.Click += (s, e) => WindowState = FormWindowState.Minimized;

            titleBar.Controls.Add(titleLabel);
            titleBar.Controls.Add(closeButton);
            titleBar.Controls.Add(maximizeButton);
            titleBar.Controls.Add(minimizeButton);

            // Title bar drag functionality
            titleBar.MouseDown += TitleBar_MouseDown;
            titleBar.MouseMove += TitleBar_MouseMove;
            titleBar.MouseUp += TitleBar_MouseUp;
            titleLabel.MouseDown += TitleBar_MouseDown;
            titleLabel.MouseMove += TitleBar_MouseMove;
            titleLabel.MouseUp += TitleBar_MouseUp;

            Controls.Add(titleBar);

            // Scan line animation
            scanLineTimer = new Timer { Interval = 100 };
            scanLineTimer.Tick += (s, e) => { scanLineOffset += 2; if (scanLineOffset > Height) scanLineOffset = 0; Invalidate(); };
            scanLineTimer.Start();
        }

        // Also fix the ArcadeForm dragging logic
        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position; // Use screen coordinates
            dragFormPoint = Location;
            ((Control)sender).Capture = true;
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point currentScreenPos = Cursor.Position;
                Point delta = new Point(
                    currentScreenPos.X - dragCursorPoint.X,
                    currentScreenPos.Y - dragCursorPoint.Y
                );

                Point newLocation = new Point(
                    dragFormPoint.X + delta.X,
                    dragFormPoint.Y + delta.Y
                );

                if (Location != newLocation)
                {
                    Location = newLocation;
                }
            }
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                dragging = false;
                ((Control)sender).Capture = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // Gradient background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(ClientRectangle,
                ArcadeColors.DeepBlue, Color.FromArgb(5, 5, 15), LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            // Scan lines
            using (Pen scanPen = new Pen(Color.FromArgb(15, ArcadeColors.NeonCyan)))
            {
                for (int y = scanLineOffset; y < Height; y += 8)
                {
                    g.DrawLine(scanPen, 0, y, Width, y);
                }
            }

            // Border glow
            using (Pen borderPen = new Pen(ArcadeColors.NeonCyan, 2))
            {
                g.DrawRectangle(borderPen, 1, 1, Width - 3, Height - 3);
            }
        }
    }

    // TextBox
    public class ArcadeTextBox : Control
    {
        private TextBox innerTextBox;
        private bool _Glowing = true;

        [Category("Options")]
        public bool Glowing
        {
            get { return _Glowing; }
            set { _Glowing = value; Invalidate(); }
        }

        public override string Text
        {
            get { return innerTextBox?.Text ?? ""; }
            set { if (innerTextBox != null) innerTextBox.Text = value; }
        }

        public bool ReadOnly
        {
            get { return innerTextBox?.ReadOnly ?? false; }
            set { if (innerTextBox != null) innerTextBox.ReadOnly = value; }
        }

        public ArcadeTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            innerTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = ArcadeColors.DarkPurple,
                ForeColor = ArcadeColors.NeonCyan,
                Font = new Font("Consolas", 10),
                Location = new Point(5, 5)
            };

            Controls.Add(innerTextBox);
            Size = new Size(200, 30);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (innerTextBox != null)
            {
                innerTextBox.Size = new Size(Width - 10, Height - 10);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(rect,
                ArcadeColors.DarkPurple, Color.FromArgb(15, 15, 25), LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, rect);
            }

            // Border with glow
            Color borderColor = _Glowing ? ArcadeColors.NeonCyan : ArcadeColors.CharcoalGray;
            using (Pen borderPen = new Pen(borderColor, 2))
            {
                g.DrawRectangle(borderPen, rect);
            }

            if (_Glowing)
            {
                using (Pen glowPen = new Pen(Color.FromArgb(50, ArcadeColors.NeonCyan), 1))
                {
                    g.DrawRectangle(glowPen, new Rectangle(-1, -1, Width + 1, Height + 1));
                }
            }
        }
    }

    // ComboBox
    public class ArcadeComboBox : ComboBox
    {
        public ArcadeComboBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            BackColor = ArcadeColors.DarkPurple;
            ForeColor = ArcadeColors.NeonCyan;
            Font = new Font("Orbitron", 9);
            ItemHeight = 25;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;

            // Background
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                using (LinearGradientBrush selBrush = new LinearGradientBrush(rect,
                    ArcadeColors.NeonCyan, ArcadeColors.NeonPink, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(selBrush, rect);
                }
                g.DrawString(Items[e.Index].ToString(), Font, new SolidBrush(Color.Black), rect);
            }
            else
            {
                g.FillRectangle(new SolidBrush(ArcadeColors.DarkPurple), rect);
                g.DrawString(Items[e.Index].ToString(), Font, new SolidBrush(ArcadeColors.NeonCyan), rect);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            g.FillRectangle(new SolidBrush(ArcadeColors.DarkPurple), rect);
            g.DrawRectangle(new Pen(ArcadeColors.NeonCyan, 2), rect);

            // Draw dropdown arrow
            Point[] arrow = {
                new Point(Width - 20, Height / 2 - 3),
                new Point(Width - 10, Height / 2 - 3),
                new Point(Width - 15, Height / 2 + 3)
            };
            g.FillPolygon(new SolidBrush(ArcadeColors.NeonCyan), arrow);

            // Draw text
            if (!string.IsNullOrEmpty(Text))
            {
                Rectangle textRect = new Rectangle(5, 0, Width - 30, Height);
                g.DrawString(Text, Font, new SolidBrush(ArcadeColors.NeonCyan), textRect,
                    new StringFormat { LineAlignment = StringAlignment.Center });
            }
        }
    }

    // ListBox
    public class ArcadeListBox : ListBox
    {
        public ArcadeListBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            DrawMode = DrawMode.OwnerDrawFixed;
            BackColor = ArcadeColors.DarkPurple;
            ForeColor = ArcadeColors.NeonCyan;
            Font = new Font("Orbitron", 9);
            ItemHeight = 30;
            BorderStyle = BorderStyle.None;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;

            // Background
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                using (LinearGradientBrush selBrush = new LinearGradientBrush(rect,
                    ArcadeColors.NeonPink, ArcadeColors.NeonCyan, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(selBrush, rect);
                }

                // Selection border
                g.DrawRectangle(new Pen(Color.White, 1), rect);
                g.DrawString(Items[e.Index].ToString(), Font, new SolidBrush(Color.Black),
                    new Rectangle(rect.X + 5, rect.Y, rect.Width, rect.Height),
                    new StringFormat { LineAlignment = StringAlignment.Center });
            }
            else
            {
                g.FillRectangle(new SolidBrush(ArcadeColors.DarkPurple), rect);
                g.DrawString(Items[e.Index].ToString(), Font, new SolidBrush(ArcadeColors.NeonCyan),
                    new Rectangle(rect.X + 5, rect.Y, rect.Width, rect.Height),
                    new StringFormat { LineAlignment = StringAlignment.Center });
            }

            // Item separator line
            g.DrawLine(new Pen(Color.FromArgb(50, ArcadeColors.NeonCyan)),
                rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Border
            g.DrawRectangle(new Pen(ArcadeColors.NeonCyan, 2), rect);
        }
    }

    // CheckBox
    public class ArcadeCheckBox : Control
    {
        private bool _Checked = false;
        private bool _Glowing = true;

        [Category("Options")]
        public bool Checked
        {
            get { return _Checked; }
            set { _Checked = value; Invalidate(); CheckedChanged?.Invoke(this, EventArgs.Empty); }
        }

        [Category("Options")]
        public bool Glowing
        {
            get { return _Glowing; }
            set { _Glowing = value; Invalidate(); }
        }

        public event EventHandler CheckedChanged;

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Checked = !Checked;
        }

        public ArcadeCheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(150, 25);
            Font = new Font("Orbitron", 10);
            ForeColor = ArcadeColors.NeonCyan;
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle checkRect = new Rectangle(2, 2, 20, 20);
            Rectangle textRect = new Rectangle(28, 0, Width - 30, Height);

            // Check box background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(checkRect,
                ArcadeColors.DarkPurple, Color.FromArgb(40, 40, 60), LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, checkRect);
            }

            // Check box border
            Color borderColor = _Glowing ? ArcadeColors.NeonCyan : ArcadeColors.CharcoalGray;
            g.DrawRectangle(new Pen(borderColor, 2), checkRect);

            // Check mark
            if (_Checked)
            {
                using (Pen checkPen = new Pen(ArcadeColors.NeonGreen, 3))
                {
                    g.DrawLines(checkPen, new Point[] {
                        new Point(6, 12),
                        new Point(10, 16),
                        new Point(18, 8)
                    });
                }
            }

            // Text with glow
            if (_Glowing)
            {
                using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(100, ForeColor)))
                {
                    g.DrawString(Text, Font, glowBrush,
                        new Rectangle(textRect.X - 1, textRect.Y - 1, textRect.Width, textRect.Height),
                        new StringFormat { LineAlignment = StringAlignment.Center });
                }
            }

            g.DrawString(Text, Font, new SolidBrush(ForeColor), textRect,
                new StringFormat { LineAlignment = StringAlignment.Center });
        }
    }

    // RadioButton
    public class ArcadeRadioButton : Control
    {
        private bool _Checked = false;
        private bool _Glowing = true;

        [Category("Options")]
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                if (value) UncheckOthers();
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Category("Options")]
        public bool Glowing
        {
            get { return _Glowing; }
            set { _Glowing = value; Invalidate(); }
        }

        public event EventHandler CheckedChanged;

        private void UncheckOthers()
        {
            if (Parent != null)
            {
                foreach (Control control in Parent.Controls)
                {
                    if (control is ArcadeRadioButton radio && radio != this)
                    {
                        radio._Checked = false;
                        radio.Invalidate();
                    }
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (!_Checked) Checked = true;
        }

        public ArcadeRadioButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(150, 25);
            Font = new Font("Orbitron", 10);
            ForeColor = ArcadeColors.NeonCyan;
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle radioRect = new Rectangle(2, 2, 20, 20);
            Rectangle textRect = new Rectangle(28, 0, Width - 30, Height);

            // Radio button background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(radioRect,
                ArcadeColors.DarkPurple, Color.FromArgb(40, 40, 60), LinearGradientMode.Vertical))
            {
                g.FillEllipse(bgBrush, radioRect);
            }

            // Radio button border
            Color borderColor = _Glowing ? ArcadeColors.NeonCyan : ArcadeColors.CharcoalGray;
            g.DrawEllipse(new Pen(borderColor, 2), radioRect);

            // Radio dot
            if (_Checked)
            {
                Rectangle dotRect = new Rectangle(6, 6, 12, 12);
                using (LinearGradientBrush dotBrush = new LinearGradientBrush(dotRect,
                    ArcadeColors.NeonGreen, ArcadeColors.NeonCyan, LinearGradientMode.Vertical))
                {
                    g.FillEllipse(dotBrush, dotRect);
                }
            }

            // Text with glow
            if (_Glowing)
            {
                using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(100, ForeColor)))
                {
                    g.DrawString(Text, Font, glowBrush,
                        new Rectangle(textRect.X - 1, textRect.Y - 1, textRect.Width, textRect.Height),
                        new StringFormat { LineAlignment = StringAlignment.Center });
                }
            }

            g.DrawString(Text, Font, new SolidBrush(ForeColor), textRect,
                new StringFormat { LineAlignment = StringAlignment.Center });
        }
    }

    // GroupBox
    public class ArcadeGroupBox : Control
    {
        private bool _ScanLines = true;
        private bool _GlowBorder = true;
        private string _HeaderText = "GROUP";

        [Category("Options")]
        public bool ScanLines
        {
            get { return _ScanLines; }
            set { _ScanLines = value; Invalidate(); }
        }

        [Category("Options")]
        public bool GlowBorder
        {
            get { return _GlowBorder; }
            set { _GlowBorder = value; Invalidate(); }
        }

        [Category("Options")]
        public string HeaderText
        {
            get { return _HeaderText; }
            set { _HeaderText = value; Invalidate(); }
        }

        public ArcadeGroupBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ContainerControl, true);
            DoubleBuffered = true;
            Size = new Size(250, 150);
            Font = new Font("Segoe UI", 12, FontStyle.Bold);
            Padding = new Padding(10, 30, 10, 10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            if (!string.IsNullOrEmpty(_HeaderText))
            {
                // Background with header
                Rectangle headerRect = new Rectangle(0, 0, Width, 35);
                Rectangle contentRect = new Rectangle(0, 35, Width, Height - 35);

                // Header background
                using (LinearGradientBrush headerBrush = new LinearGradientBrush(headerRect,
                    Color.FromArgb(45, 45, 55), Color.FromArgb(35, 35, 45), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(headerBrush, headerRect);
                }

                // Content background
                using (SolidBrush contentBrush = new SolidBrush(Color.FromArgb(25, 25, 35)))
                {
                    g.FillRectangle(contentBrush, contentRect);
                }

                // Header text
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    Rectangle textRect = new Rectangle(15, 0, Width - 30, 35);
                    g.DrawString(_HeaderText, Font, textBrush, textRect, sf);
                }

                // Header separator line
                if (_GlowBorder)
                {
                    using (Pen separatorPen = new Pen(ArcadeColors.NeonCyan, 1))
                    {
                        g.DrawLine(separatorPen, 0, 35, Width, 35);
                    }
                }
            }
            else
            {
                // No header - just content background
                using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(25, 25, 35)))
                {
                    g.FillRectangle(bgBrush, rect);
                }
            }

            // Border
            if (_GlowBorder)
            {
                g.DrawRectangle(new Pen(ArcadeColors.NeonCyan, 1), rect);
            }

            // Scan lines
            if (_ScanLines)
            {
                using (Pen scanPen = new Pen(Color.FromArgb(15, ArcadeColors.NeonCyan)))
                {
                    int startY = string.IsNullOrEmpty(_HeaderText) ? 5 : 40;
                    for (int y = startY; y < Height; y += 6)
                    {
                        g.DrawLine(scanPen, 2, y, Width - 2, y);
                    }
                }
            }
        }
    }

    // PictureBox
    public class ArcadePictureBox : Control
    {
        private Image _Image;
        private PictureBoxSizeMode _SizeMode = PictureBoxSizeMode.Normal;
        private bool _GlowBorder = true;

        [Category("Appearance")]
        public Image Image
        {
            get { return _Image; }
            set { _Image = value; Invalidate(); }
        }

        [Category("Behavior")]
        public PictureBoxSizeMode SizeMode
        {
            get { return _SizeMode; }
            set { _SizeMode = value; Invalidate(); }
        }

        [Category("Options")]
        public bool GlowBorder
        {
            get { return _GlowBorder; }
            set { _GlowBorder = value; Invalidate(); }
        }

        public ArcadePictureBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(100, 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(rect,
                Color.FromArgb(10, 10, 20), Color.FromArgb(20, 20, 35), LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, rect);
            }

            // Draw image if present
            if (_Image != null)
            {
                Rectangle imageRect = GetImageRect();
                g.DrawImage(_Image, imageRect);
            }

            // Border
            if (_GlowBorder)
            {
                g.DrawRectangle(new Pen(ArcadeColors.NeonCyan, 2), rect);

                // Outer glow
                using (Pen glowPen = new Pen(Color.FromArgb(100, ArcadeColors.NeonCyan), 1))
                {
                    g.DrawRectangle(glowPen, new Rectangle(-1, -1, Width + 1, Height + 1));
                }
            }
        }

        private Rectangle GetImageRect()
        {
            if (_Image == null) return Rectangle.Empty;

            switch (_SizeMode)
            {
                case PictureBoxSizeMode.Normal:
                    return new Rectangle(2, 2, _Image.Width, _Image.Height);
                case PictureBoxSizeMode.StretchImage:
                    return new Rectangle(2, 2, Width - 4, Height - 4);
                case PictureBoxSizeMode.CenterImage:
                    int x = (Width - _Image.Width) / 2;
                    int y = (Height - _Image.Height) / 2;
                    return new Rectangle(x, y, _Image.Width, _Image.Height);
                case PictureBoxSizeMode.Zoom:
                    return GetZoomedRect();
                default:
                    return new Rectangle(2, 2, Width - 4, Height - 4);
            }
        }

        private Rectangle GetZoomedRect()
        {
            if (_Image == null) return Rectangle.Empty;

            float ratioX = (float)(Width - 4) / _Image.Width;
            float ratioY = (float)(Height - 4) / _Image.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(_Image.Width * ratio);
            int newHeight = (int)(_Image.Height * ratio);
            int x = (Width - newWidth) / 2;
            int y = (Height - newHeight) / 2;

            return new Rectangle(x, y, newWidth, newHeight);
        }
    }

    // TabControl
    public class ArcadeTabControl : TabControl
    {
        public ArcadeTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            Font = new Font("Orbitron", 10, FontStyle.Bold);
            ItemSize = new Size(120, 35);
            SizeMode = TabSizeMode.Fixed;
            Appearance = TabAppearance.Normal;

            // Remove default padding that causes white space
            Padding = new Point(0, 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate areas
            Rectangle tabAreaRect = new Rectangle(0, 0, Width, ItemSize.Height);
            Rectangle pageRect = new Rectangle(0, ItemSize.Height, Width, Height - ItemSize.Height);

            // Fill entire tab area with dark background first to eliminate white spaces
            using (LinearGradientBrush tabAreaBrush = new LinearGradientBrush(tabAreaRect,
                ArcadeColors.DarkPurple, Color.FromArgb(30, 30, 45), LinearGradientMode.Vertical))
            {
                g.FillRectangle(tabAreaBrush, tabAreaRect);
            }

            // Draw tab pages background
            using (LinearGradientBrush pageBrush = new LinearGradientBrush(pageRect,
                ArcadeColors.DarkPurple, Color.FromArgb(15, 15, 25), LinearGradientMode.Vertical))
            {
                g.FillRectangle(pageBrush, pageRect);
            }

            // Draw tabs with original styling
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle tabRect = GetTabRect(i);
                bool isSelected = (i == SelectedIndex);

                // Tab background - original design
                if (isSelected)
                {
                    using (LinearGradientBrush selectedBrush = new LinearGradientBrush(tabRect,
                        ArcadeColors.NeonCyan, ArcadeColors.NeonPink, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(selectedBrush, tabRect);
                    }
                }
                else
                {
                    using (LinearGradientBrush tabBrush = new LinearGradientBrush(tabRect,
                        Color.FromArgb(40, 40, 60), ArcadeColors.DarkPurple, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(tabBrush, tabRect);
                    }
                }

                // Tab border - original design
                Color borderColor = isSelected ? Color.White : ArcadeColors.NeonCyan;
                g.DrawRectangle(new Pen(borderColor, 1), tabRect);

                // Tab text - original design
                Color textColor = isSelected ? Color.Black : ArcadeColors.NeonCyan;
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(TabPages[i].Text, Font, new SolidBrush(textColor), tabRect, sf);
            }

            // Page border
            g.DrawRectangle(new Pen(ArcadeColors.NeonCyan, 2), pageRect);
        }
    }

    // MenuStrip
    public class ArcadeMenuStrip : MenuStrip
    {
        public ArcadeMenuStrip()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            BackColor = ArcadeColors.DarkPurple;
            ForeColor = ArcadeColors.NeonCyan;
            Font = new Font("Orbitron", 10, FontStyle.Bold);
            Renderer = new ArcadeMenuRenderer();
        }
    }

    public class ArcadeMenuRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(e.Item.Bounds,
                    ArcadeColors.NeonCyan, ArcadeColors.NeonPink, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, e.Item.Bounds);
                }
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(ArcadeColors.DarkPurple), e.Item.Bounds);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(e.AffectedBounds,
                ArcadeColors.DarkPurple, Color.FromArgb(40, 40, 60), LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }
    }

    // StatusStrip
    public class ArcadeStatusStrip : StatusStrip
    {
        private Timer pulseTimer;
        private int pulseValue = 0;
        private int pulseDirection = 1;

        public ArcadeStatusStrip()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            BackColor = ArcadeColors.DarkPurple;
            ForeColor = ArcadeColors.NeonCyan;
            Font = new Font("Orbitron", 9);

            pulseTimer = new Timer { Interval = 50 };
            pulseTimer.Tick += (s, e) => {
                pulseValue += pulseDirection * 3;
                if (pulseValue >= 50) pulseDirection = -1;
                if (pulseValue <= 0) pulseDirection = 1;
                Invalidate();
            };
            pulseTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            // Background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(rect,
                ArcadeColors.DarkPurple, Color.FromArgb(40, 40, 60), LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, rect);
            }

            // Pulsing top border
            Color pulseColor = Color.FromArgb(pulseValue + 50, ArcadeColors.NeonCyan);
            g.DrawLine(new Pen(pulseColor, 2), 0, 0, Width, 0);

            base.OnPaint(e);
        }
    }

    // DataGridView
    public class ArcadeDataGridView : DataGridView
    {
        public ArcadeDataGridView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            BackgroundColor = ArcadeColors.DarkPurple;
            GridColor = ArcadeColors.NeonCyan;
            DefaultCellStyle.BackColor = ArcadeColors.DarkPurple;
            DefaultCellStyle.ForeColor = ArcadeColors.NeonCyan;
            DefaultCellStyle.SelectionBackColor = ArcadeColors.NeonPink;
            DefaultCellStyle.SelectionForeColor = Color.Black;

            ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 60);
            ColumnHeadersDefaultCellStyle.ForeColor = ArcadeColors.NeonCyan;
            ColumnHeadersDefaultCellStyle.Font = new Font("Orbitron", 10, FontStyle.Bold);

            RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 60);
            RowHeadersDefaultCellStyle.ForeColor = ArcadeColors.NeonCyan;

            EnableHeadersVisualStyles = false;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.Single;
            Font = new Font("Consolas", 9);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw outer border
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            e.Graphics.DrawRectangle(new Pen(ArcadeColors.NeonCyan, 2), rect);
        }
    }

    // TrackBar/Slider
    public class ArcadeTrackBar : Control
    {
        private int _Value = 0;
        private int _Minimum = 0;
        private int _Maximum = 100;
        private bool isDragging = false;
        private bool _Glowing = true;

        [Category("Behavior")]
        public int Value
        {
            get { return _Value; }
            set
            {
                int newValue = Math.Max(_Minimum, Math.Min(_Maximum, value));
                if (_Value != newValue)
                {
                    _Value = newValue;
                    Invalidate();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Category("Behavior")]
        public int Minimum
        {
            get { return _Minimum; }
            set { _Minimum = value; if (_Value < value) Value = value; }
        }

        [Category("Behavior")]
        public int Maximum
        {
            get { return _Maximum; }
            set { _Maximum = value; if (_Value > value) Value = value; }
        }

        [Category("Options")]
        public bool Glowing
        {
            get { return _Glowing; }
            set { _Glowing = value; Invalidate(); }
        }

        public event EventHandler ValueChanged;

        public ArcadeTrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(200, 30);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            int trackWidth = Width - 20;
            float percent = Math.Max(0, Math.Min(1, (float)(mouseX - 10) / trackWidth));
            Value = (int)(_Minimum + percent * (_Maximum - _Minimum));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Track background
            Rectangle trackRect = new Rectangle(10, Height / 2 - 3, Width - 20, 6);
            using (LinearGradientBrush trackBrush = new LinearGradientBrush(trackRect,
                ArcadeColors.DarkPurple, Color.FromArgb(40, 40, 60), LinearGradientMode.Vertical))
            {
                g.FillRectangle(trackBrush, trackRect);
            }
            g.DrawRectangle(new Pen(ArcadeColors.NeonCyan, 1), trackRect);

            // Progress fill
            float percent = (float)(_Value - _Minimum) / (_Maximum - _Minimum);
            int fillWidth = (int)(trackRect.Width * percent);
            if (fillWidth > 0)
            {
                Rectangle fillRect = new Rectangle(trackRect.X, trackRect.Y, fillWidth, trackRect.Height);
                using (LinearGradientBrush fillBrush = new LinearGradientBrush(fillRect,
                    ArcadeColors.NeonCyan, ArcadeColors.NeonPink, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(fillBrush, fillRect);
                }
            }

            // Thumb
            int thumbX = (int)(10 + (Width - 30) * percent);
            Rectangle thumbRect = new Rectangle(thumbX, 5, 10, Height - 10);

            using (LinearGradientBrush thumbBrush = new LinearGradientBrush(thumbRect,
                Color.White, ArcadeColors.NeonCyan, LinearGradientMode.Vertical))
            {
                g.FillEllipse(thumbBrush, thumbRect);
            }
            g.DrawEllipse(new Pen(ArcadeColors.NeonCyan, 2), thumbRect);

            // Glow effect
            if (_Glowing)
            {
                using (Pen glowPen = new Pen(Color.FromArgb(100, ArcadeColors.NeonCyan), 1))
                {
                    g.DrawEllipse(glowPen, new Rectangle(thumbRect.X - 2, thumbRect.Y - 2, thumbRect.Width + 4, thumbRect.Height + 4));
                }
            }
        }
    }

    // Timer (Visual Component)
    public class ArcadeTimer : Component
    {
        private System.Windows.Forms.Timer internalTimer;

        public event EventHandler Tick
        {
            add { internalTimer.Tick += value; }
            remove { internalTimer.Tick -= value; }
        }

        public int Interval
        {
            get { return internalTimer.Interval; }
            set { internalTimer.Interval = value; }
        }

        public bool Enabled
        {
            get { return internalTimer.Enabled; }
            set { internalTimer.Enabled = value; }
        }

        public ArcadeTimer()
        {
            internalTimer = new System.Windows.Forms.Timer();
        }

        public void Start() => internalTimer.Start();
        public void Stop() => internalTimer.Stop();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                internalTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // ToolTip
    public class ArcadeToolTip : ToolTip
    {
        public ArcadeToolTip()
        {
            OwnerDraw = true;
            BackColor = ArcadeColors.DarkPurple;
            ForeColor = ArcadeColors.NeonCyan;

            Draw += OnDraw;
            Popup += OnPopup;
        }

        private void OnPopup(object sender, PopupEventArgs e)
        {
            // Calculate size with padding
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF size = g.MeasureString(GetToolTip(e.AssociatedControl), new Font("Orbitron", 9));
                e.ToolTipSize = new Size((int)size.Width + 20, (int)size.Height + 15);
            }
        }

        private void OnDraw(object sender, DrawToolTipEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(e.Bounds,
                ArcadeColors.DarkPurple, Color.FromArgb(40, 40, 60), LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, e.Bounds);
            }

            // Border with glow
            g.DrawRectangle(new Pen(ArcadeColors.NeonCyan, 2), e.Bounds);
            using (Pen glowPen = new Pen(Color.FromArgb(100, ArcadeColors.NeonCyan), 1))
            {
                Rectangle glowRect = new Rectangle(e.Bounds.X - 1, e.Bounds.Y - 1, e.Bounds.Width + 1, e.Bounds.Height + 1);
                g.DrawRectangle(glowPen, glowRect);
            }

            // Text with glow
            Font font = new Font("Orbitron", 9);
            Rectangle textRect = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 5, e.Bounds.Width - 10, e.Bounds.Height - 10);

            // Text glow
            using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(100, ArcadeColors.NeonCyan)))
            {
                for (int i = 1; i <= 2; i++)
                {
                    Rectangle glowTextRect = new Rectangle(textRect.X - i, textRect.Y - i, textRect.Width, textRect.Height);
                    g.DrawString(e.ToolTipText, font, glowBrush, glowTextRect);
                }
            }

            // Main text
            g.DrawString(e.ToolTipText, font, new SolidBrush(ArcadeColors.NeonCyan), textRect);
        }
    }

    // SplitContainer
    public class ArcadeSplitContainer : SplitContainer
    {
        public ArcadeSplitContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            BackColor = ArcadeColors.DarkPurple;
            Panel1.BackColor = Color.FromArgb(20, 20, 35);
            Panel2.BackColor = Color.FromArgb(20, 20, 35);
            SplitterWidth = 8;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // Draw splitter with neon effect
            Rectangle splitterRect = SplitterRectangle;
            using (LinearGradientBrush splitterBrush = new LinearGradientBrush(splitterRect,
                ArcadeColors.NeonCyan, ArcadeColors.NeonPink,
                Orientation == Orientation.Horizontal ? LinearGradientMode.Horizontal : LinearGradientMode.Vertical))
            {
                g.FillRectangle(splitterBrush, splitterRect);
            }

            // Draw grip dots
            int centerX = splitterRect.X + splitterRect.Width / 2;
            int centerY = splitterRect.Y + splitterRect.Height / 2;

            using (SolidBrush dotBrush = new SolidBrush(Color.White))
            {
                if (Orientation == Orientation.Vertical)
                {
                    for (int i = -6; i <= 6; i += 6)
                    {
                        g.FillEllipse(dotBrush, centerX - 1, centerY + i - 1, 3, 3);
                    }
                }
                else
                {
                    for (int i = -6; i <= 6; i += 6)
                    {
                        g.FillEllipse(dotBrush, centerX + i - 1, centerY - 1, 3, 3);
                    }
                }
            }
        }
    }

    // FormSkin - Drop this on any form to give it the arcade theme
    public class ArcadeFormSkin : ContainerControl
    {
        private int W, H;
        private bool isDragging = false;
        private Point dragCursorPoint, dragFormPoint;
        private Timer scanLineTimer, titleGlowTimer;
        private int scanLineOffset = 0;
        private int titleGlowIntensity = 0;
        private int titleGlowDirection = 1;
        private int moveHeight = 50;

        private bool _ShowWindowControls = true;
        private bool _AnimatedScanLines = true;
        private bool _GlowingTitle = true;
        private bool _ShowLogo = true;
        private string _FormTitle = "RETRO ARCADE";

        [Category("Appearance")]
        public bool ShowWindowControls
        {
            get { return _ShowWindowControls; }
            set { _ShowWindowControls = value; Invalidate(); }
        }

        [Category("Appearance")]
        public bool AnimatedScanLines
        {
            get { return _AnimatedScanLines; }
            set
            {
                _AnimatedScanLines = value;
                if (scanLineTimer != null)
                    scanLineTimer.Enabled = value;
            }
        }

        [Category("Appearance")]
        public bool GlowingTitle
        {
            get { return _GlowingTitle; }
            set
            {
                _GlowingTitle = value;
                if (titleGlowTimer != null)
                    titleGlowTimer.Enabled = value;
            }
        }

        [Category("Appearance")]
        public bool ShowLogo
        {
            get { return _ShowLogo; }
            set { _ShowLogo = value; Invalidate(); }
        }

        [Category("Appearance")]
        public string FormTitle
        {
            get { return _FormTitle; }
            set { _FormTitle = value; Invalidate(); }
        }

        [Category("Colors")]
        public Color HeaderColor { get; set; } = ArcadeColors.DarkPurple;

        [Category("Colors")]
        public Color BaseColor { get; set; } = ArcadeColors.DeepBlue;

        [Category("Colors")]
        public Color BorderColor { get; set; } = ArcadeColors.NeonCyan;

        [Category("Colors")]
        public Color AccentColor { get; set; } = ArcadeColors.NeonPink;

        public ArcadeFormSkin()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            BackColor = Color.White;
            Font = new Font("Orbitron", 12, FontStyle.Bold);

            // Scan line animation timer
            scanLineTimer = new Timer { Interval = 100 };
            scanLineTimer.Tick += (s, e) => {
                scanLineOffset += 2;
                if (scanLineOffset > Height) scanLineOffset = 0;
                Invalidate();
            };

            // Title glow animation timer
            titleGlowTimer = new Timer { Interval = 50 };
            titleGlowTimer.Tick += (s, e) => {
                titleGlowIntensity += titleGlowDirection * 5;
                if (titleGlowIntensity >= 50) titleGlowDirection = -1;
                if (titleGlowIntensity <= 0) titleGlowDirection = 1;
                Invalidate();
            };

            if (_AnimatedScanLines) scanLineTimer.Start();
            if (_GlowingTitle) titleGlowTimer.Start();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (ParentForm != null)
            {
                ParentForm.FormBorderStyle = FormBorderStyle.None;
                ParentForm.StartPosition = FormStartPosition.CenterScreen;
                ParentForm.BackColor = BaseColor;
            }
            Dock = DockStyle.Fill;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && new Rectangle(0, 0, Width, moveHeight).Contains(e.Location))
            {
                // Handle window control clicks
                if (_ShowWindowControls && HandleWindowControlClick(e.Location))
                    return;

                // Start dragging - use screen coordinates to avoid calculation issues
                isDragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = ParentForm.Location;
                Capture = true; // Capture mouse to prevent losing events
            }
        }

        private bool HandleWindowControlClick(Point location)
        {
            if (!_ShowWindowControls) return false;

            int buttonWidth = 30;
            int buttonHeight = 25;
            int buttonY = 12;
            int rightMargin = 10;

            // Close button
            Rectangle closeRect = new Rectangle(Width - rightMargin - buttonWidth, buttonY, buttonWidth, buttonHeight);
            if (closeRect.Contains(location))
            {
                ParentForm?.Close();
                return true;
            }

            // Maximize button
            Rectangle maxRect = new Rectangle(Width - rightMargin - (buttonWidth * 2) - 5, buttonY, buttonWidth, buttonHeight);
            if (maxRect.Contains(location))
            {
                if (ParentForm != null)
                {
                    ParentForm.WindowState = ParentForm.WindowState == FormWindowState.Maximized
                        ? FormWindowState.Normal
                        : FormWindowState.Maximized;
                }
                return true;
            }

            // Minimize button
            Rectangle minRect = new Rectangle(Width - rightMargin - (buttonWidth * 3) - 10, buttonY, buttonWidth, buttonHeight);
            if (minRect.Contains(location))
            {
                if (ParentForm != null)
                    ParentForm.WindowState = FormWindowState.Minimized;
                return true;
            }

            return false;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isDragging)
            {
                isDragging = false;
                Capture = false; // Release mouse capture
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging && ParentForm != null)
            {
                // Use screen coordinates for smooth dragging
                Point currentScreenPos = Cursor.Position;
                Point delta = new Point(
                    currentScreenPos.X - dragCursorPoint.X,
                    currentScreenPos.Y - dragCursorPoint.Y
                );

                Point newLocation = new Point(
                    dragFormPoint.X + delta.X,
                    dragFormPoint.Y + delta.Y
                );

                // Only update if the position actually changed to prevent unnecessary updates
                if (ParentForm.Location != newLocation)
                {
                    ParentForm.Location = newLocation;
                }
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (ParentForm != null && _ShowWindowControls)
            {
                ParentForm.WindowState = ParentForm.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            G.SmoothingMode = SmoothingMode.AntiAlias;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            G.PixelOffsetMode = PixelOffsetMode.HighQuality;

            W = Width;
            H = Height;

            Rectangle baseRect = new Rectangle(0, 0, W, H);
            Rectangle headerRect = new Rectangle(0, 0, W, 50);

            // Main background gradient
            using (LinearGradientBrush baseBrush = new LinearGradientBrush(baseRect,
                BaseColor, Color.FromArgb(5, 5, 15), LinearGradientMode.Vertical))
            {
                G.FillRectangle(baseBrush, baseRect);
            }

            // Header background
            using (LinearGradientBrush headerBrush = new LinearGradientBrush(headerRect,
                HeaderColor, Color.FromArgb(40, 40, 65), LinearGradientMode.Vertical))
            {
                G.FillRectangle(headerBrush, headerRect);
            }

            // Animated scan lines
            if (_AnimatedScanLines)
            {
                using (Pen scanPen = new Pen(Color.FromArgb(15, BorderColor)))
                {
                    for (int y = scanLineOffset; y < H; y += 8)
                    {
                        G.DrawLine(scanPen, 0, y, W, y);
                    }
                }
            }

            // Logo
            if (_ShowLogo)
            {
                DrawLogo(G);
            }

            // Title with glow effect
            DrawTitle(G);

            // Window controls
            if (_ShowWindowControls)
            {
                DrawWindowControls(G);
            }

            // Outer border with glow
            using (Pen borderPen = new Pen(BorderColor, 3))
            {
                G.DrawRectangle(borderPen, 1, 1, W - 3, H - 3);
            }

            // Additional glow layers
            using (Pen glowPen1 = new Pen(Color.FromArgb(100, BorderColor), 1))
            {
                G.DrawRectangle(glowPen1, 0, 0, W - 1, H - 1);
            }
            using (Pen glowPen2 = new Pen(Color.FromArgb(50, BorderColor), 1))
            {
                G.DrawRectangle(glowPen2, -1, -1, W + 1, H + 1);
            }

            // Header separator line
            using (Pen sepPen = new Pen(BorderColor, 2))
            {
                G.DrawLine(sepPen, 0, 50, W, 50);
            }
        }

        private void DrawLogo(Graphics G)
        {
            // Retro-style logo bars
            Rectangle logo1 = new Rectangle(15, 16, 6, 18);
            Rectangle logo2 = new Rectangle(25, 16, 6, 18);
            Rectangle logo3 = new Rectangle(35, 16, 6, 18);

            // Animated color cycling
            int offset = (int)(DateTime.Now.Millisecond / 100) % 3;
            Color[] colors = { BorderColor, AccentColor, ArcadeColors.NeonGreen };

            using (LinearGradientBrush brush1 = new LinearGradientBrush(logo1, colors[offset], colors[(offset + 1) % 3], LinearGradientMode.Vertical))
            using (LinearGradientBrush brush2 = new LinearGradientBrush(logo2, colors[(offset + 1) % 3], colors[(offset + 2) % 3], LinearGradientMode.Vertical))
            using (LinearGradientBrush brush3 = new LinearGradientBrush(logo3, colors[(offset + 2) % 3], colors[offset], LinearGradientMode.Vertical))
            {
                G.FillRectangle(brush1, logo1);
                G.FillRectangle(brush2, logo2);
                G.FillRectangle(brush3, logo3);
            }

            // Logo glow
            using (Pen glowPen = new Pen(Color.FromArgb(80, Color.White), 1))
            {
                G.DrawRectangle(glowPen, new Rectangle(logo1.X - 1, logo1.Y - 1, logo1.Width + 1, logo1.Height + 1));
                G.DrawRectangle(glowPen, new Rectangle(logo2.X - 1, logo2.Y - 1, logo2.Width + 1, logo2.Height + 1));
                G.DrawRectangle(glowPen, new Rectangle(logo3.X - 1, logo3.Y - 1, logo3.Width + 1, logo3.Height + 1));
            }
        }

        private void DrawTitle(Graphics G)
        {
            Rectangle titleRect = new Rectangle(50, 15, W - 200, 20);

            if (_GlowingTitle)
            {
                // Multi-layer glow effect
                Color glowColor = Color.FromArgb(titleGlowIntensity + 30, BorderColor);
                using (SolidBrush glowBrush = new SolidBrush(glowColor))
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        Rectangle glowRect = new Rectangle(titleRect.X - i, titleRect.Y - i, titleRect.Width + (i * 2), titleRect.Height + (i * 2));
                        G.DrawString(_FormTitle, Font, glowBrush, glowRect, ArcadeHelpers.NearSF);
                    }
                }
            }

            // Main title text
            using (LinearGradientBrush titleBrush = new LinearGradientBrush(titleRect,
                Color.White, BorderColor, LinearGradientMode.Horizontal))
            {
                G.DrawString(_FormTitle, Font, titleBrush, titleRect, ArcadeHelpers.NearSF);
            }
        }

        private void DrawWindowControls(Graphics G)
        {
            int buttonWidth = 30;
            int buttonHeight = 25;
            int buttonY = 12;
            int rightMargin = 10;
            Font buttonFont = new Font("Arial", 12, FontStyle.Bold);

            // Close button
            Rectangle closeRect = new Rectangle(W - rightMargin - buttonWidth, buttonY, buttonWidth, buttonHeight);
            DrawWindowButton(G, closeRect, "×", ArcadeColors.AsteroidRed, buttonFont);

            // Maximize button
            Rectangle maxRect = new Rectangle(W - rightMargin - (buttonWidth * 2) - 5, buttonY, buttonWidth, buttonHeight);
            string maxText = ParentForm?.WindowState == FormWindowState.Maximized ? "❐" : "□";
            DrawWindowButton(G, maxRect, maxText, ArcadeColors.NeonGreen, buttonFont);

            // Minimize button
            Rectangle minRect = new Rectangle(W - rightMargin - (buttonWidth * 3) - 10, buttonY, buttonWidth, buttonHeight);
            DrawWindowButton(G, minRect, "−", ArcadeColors.NeonYellow, buttonFont);
        }

        private void DrawWindowButton(Graphics G, Rectangle rect, string text, Color color, Font font)
        {
            // Button background
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(rect,
                Color.FromArgb(40, 40, 60), Color.FromArgb(25, 25, 40), LinearGradientMode.Vertical))
            {
                G.FillRectangle(bgBrush, rect);
            }

            // Button border
            using (Pen borderPen = new Pen(color, 1))
            {
                G.DrawRectangle(borderPen, rect);
            }

            // Button text
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using (SolidBrush textBrush = new SolidBrush(color))
            {
                G.DrawString(text, font, textBrush, rect, sf);
            }

            // Button glow
            using (Pen glowPen = new Pen(Color.FromArgb(100, color), 1))
            {
                G.DrawRectangle(glowPen, new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 1, rect.Height + 1));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                scanLineTimer?.Stop();
                scanLineTimer?.Dispose();
                titleGlowTimer?.Stop();
                titleGlowTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Custom Scrollable TextBox with Dark Theme Scrollbar
    public class ArcadeScrollableTextBox : Control
    {
        private List<ColoredText> textLines = new List<ColoredText>();
        private VScrollBar vScrollBar;
        private int lineHeight = 16;
        private int maxVisibleLines;
        private Font textFont;

        public struct ColoredText
        {
            public string Text;
            public Color Color;
            public ColoredText(string text, Color color)
            {
                Text = text;
                Color = color;
            }
        }

        public ArcadeScrollableTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            textFont = new Font("Consolas", 10F);
            BackColor = Color.FromArgb(30, 30, 30);

            // Create custom scrollbar
            vScrollBar = new VScrollBar();
            vScrollBar.Dock = DockStyle.Right;
            vScrollBar.Width = 18;
            vScrollBar.Scroll += VScrollBar_Scroll;
            Controls.Add(vScrollBar);

            // Style the scrollbar
            StyleScrollBar();

            // Add initial text
            AddText("🖥️ ROM Scanner Console", Color.FromArgb(100, 200, 255));
            AddText("", Color.White);
            AddText("Ready to scan ROM collection...", Color.FromArgb(220, 220, 220));
            AddText("Select a directory and configure options to begin.", Color.FromArgb(220, 220, 220));
        }

        private void StyleScrollBar()
        {
            // Unfortunately, we can't directly style VScrollBar colors in WinForms
            // But we can create a custom one. For now, let's use a darker background
            vScrollBar.BackColor = Color.FromArgb(40, 40, 40);
        }

        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        public void AddText(string text, Color color)
        {
            textLines.Add(new ColoredText(text, color));
            UpdateScrollBar();
            ScrollToBottom();
            Invalidate();
        }

        public void ClearText()
        {
            textLines.Clear();
            UpdateScrollBar();
            Invalidate();
        }

        private void UpdateScrollBar()
        {
            maxVisibleLines = Math.Max(1, (Height - 10) / lineHeight);

            if (textLines.Count > maxVisibleLines)
            {
                vScrollBar.Visible = true;
                vScrollBar.Minimum = 0;
                vScrollBar.Maximum = Math.Max(maxVisibleLines, textLines.Count - 1);
                vScrollBar.LargeChange = maxVisibleLines;
                vScrollBar.SmallChange = 1;
            }
            else
            {
                vScrollBar.Visible = false;
                vScrollBar.Minimum = 0;
                vScrollBar.Maximum = Math.Max(1, textLines.Count);
                vScrollBar.Value = 0;
            }
        }

        private void ScrollToBottom()
        {
            if (vScrollBar.Visible && textLines.Count > maxVisibleLines)
            {
                int newValue = Math.Max(0, textLines.Count - maxVisibleLines);
                newValue = Math.Min(newValue, vScrollBar.Maximum - vScrollBar.LargeChange + 1);
                vScrollBar.Value = Math.Max(vScrollBar.Minimum, Math.Min(vScrollBar.Maximum, newValue));
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollBar();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);

            if (textLines.Count == 0) return;

            int startLine = vScrollBar.Visible ? vScrollBar.Value : 0;
            int endLine = Math.Min(startLine + maxVisibleLines, textLines.Count);

            int y = 5;
            for (int i = startLine; i < endLine; i++)
            {
                if (i < textLines.Count)
                {
                    using (SolidBrush brush = new SolidBrush(textLines[i].Color))
                    {
                        g.DrawString(textLines[i].Text, textFont, brush, 5, y);
                    }
                }
                y += lineHeight;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (vScrollBar.Visible)
            {
                int newValue = vScrollBar.Value - (e.Delta / 120) * 3;
                newValue = Math.Max(vScrollBar.Minimum, Math.Min(vScrollBar.Maximum - maxVisibleLines + 1, newValue));
                vScrollBar.Value = newValue;
                Invalidate();
            }
            base.OnMouseWheel(e);
        }
    }

    // Custom Dark Scrollbar Control
    public class ArcadeScrollBar : Control
    {
        private bool isDragging = false;
        private int thumbPosition = 0;
        private int thumbHeight = 30;
        private int mouseOffset = 0;

        private int _Minimum = 0;
        private int _Maximum = 100;
        private int _Value = 0;
        private int _LargeChange = 10;

        public event ScrollEventHandler Scroll;

        public int Minimum
        {
            get { return _Minimum; }
            set { _Minimum = value; UpdateThumb(); }
        }

        public int Maximum
        {
            get { return _Maximum; }
            set { _Maximum = value; UpdateThumb(); }
        }

        public int Value
        {
            get { return _Value; }
            set
            {
                _Value = Math.Max(_Minimum, Math.Min(_Maximum, value));
                UpdateThumb();
                Invalidate();
            }
        }

        public int LargeChange
        {
            get { return _LargeChange; }
            set { _LargeChange = value; UpdateThumb(); }
        }

        public ArcadeScrollBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Width = 18;
            BackColor = Color.FromArgb(25, 25, 25);
            UpdateThumb();
        }

        private void UpdateThumb()
        {
            if (_Maximum <= _Minimum) return;

            int trackHeight = Height - 4;
            thumbHeight = Math.Max(20, (int)((float)_LargeChange / (_Maximum - _Minimum) * trackHeight));

            float ratio = (float)(_Value - _Minimum) / (_Maximum - _Minimum);
            thumbPosition = 2 + (int)(ratio * (trackHeight - thumbHeight));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Rectangle thumbRect = new Rectangle(2, thumbPosition, Width - 4, thumbHeight);
            if (thumbRect.Contains(e.Location))
            {
                isDragging = true;
                mouseOffset = e.Y - thumbPosition;
                Capture = true;
            }
            else
            {
                // Click on track
                if (e.Y < thumbPosition)
                    Value -= _LargeChange;
                else
                    Value += _LargeChange;

                Scroll?.Invoke(this, new ScrollEventArgs(ScrollEventType.LargeDecrement, Value));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isDragging)
            {
                int newThumbPos = e.Y - mouseOffset;
                int trackHeight = Height - thumbHeight - 4;

                newThumbPos = Math.Max(2, Math.Min(trackHeight + 2, newThumbPos));

                float ratio = (float)(newThumbPos - 2) / trackHeight;
                int newValue = _Minimum + (int)(ratio * (_Maximum - _Minimum));

                if (newValue != _Value)
                {
                    _Value = newValue;
                    thumbPosition = newThumbPos;
                    Invalidate();
                    Scroll?.Invoke(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, _Value));
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
            Capture = false;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateThumb();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Track background
            using (SolidBrush trackBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
            {
                g.FillRectangle(trackBrush, 1, 1, Width - 2, Height - 2);
            }

            // Track border
            using (Pen trackPen = new Pen(Color.FromArgb(60, 60, 60)))
            {
                g.DrawRectangle(trackPen, 0, 0, Width - 1, Height - 1);
            }

            // Thumb
            Rectangle thumbRect = new Rectangle(2, thumbPosition, Width - 4, thumbHeight);

            using (LinearGradientBrush thumbBrush = new LinearGradientBrush(thumbRect,
                Color.FromArgb(80, 80, 80), Color.FromArgb(60, 60, 60), LinearGradientMode.Vertical))
            {
                g.FillRectangle(thumbBrush, thumbRect);
            }

            // Thumb border
            using (Pen thumbPen = new Pen(Color.FromArgb(100, 100, 100)))
            {
                g.DrawRectangle(thumbPen, thumbRect);
            }

            // Thumb grip lines
            using (Pen gripPen = new Pen(Color.FromArgb(120, 120, 120)))
            {
                int centerY = thumbPosition + thumbHeight / 2;
                for (int i = -2; i <= 2; i += 2)
                {
                    g.DrawLine(gripPen, 4, centerY + i, Width - 5, centerY + i);
                }
            }
        }
    }
}