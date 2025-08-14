
// RetroControls_Fixed.cs
// Retro WinForms controls with NO transparent backgrounds on custom controls.
// This fixes designer errors like: "Control does not support transparent background colors".
// Drop this file into your project and use the RetroForms namespace.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RetroForms
{
    internal enum MouseState { None, Over, Down }

    internal static class Retro
    {
        public static readonly Color Bg = Color.FromArgb(24, 24, 24);
        public static readonly Color Panel = Color.FromArgb(33, 33, 33);
        public static readonly Color Border = Color.FromArgb(96, 96, 96);
        public static readonly Color Light = Color.FromArgb(220, 220, 220);
        public static readonly Color Text = Color.FromArgb(255, 255, 255);
        public static readonly Color Accent = Color.FromArgb(255, 85, 85);
        public static readonly Color Accent2 = Color.FromArgb(255, 255, 85);

        public static StringFormat CenterSF = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        public static StringFormat NearSF = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };

        public static void UsePixel(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
        }

        public static Font PixelFont(float size, FontStyle style = FontStyle.Regular)
        {
            try { return new Font("Press Start 2P", size, style, GraphicsUnit.Point); } catch { }
            try { return new Font("Pixel Emulator", size, style, GraphicsUnit.Point); } catch { }
            return new Font("Consolas", size, style, GraphicsUnit.Point);
        }

        public static void Bezel(Graphics g, Rectangle r)
        {
            using (var p1 = new Pen(Border, 2))
            using (var p2 = new Pen(Accent2, 1))
            {
                g.DrawRectangle(p1, r.X, r.Y, r.Width - 1, r.Height - 1);
                g.DrawRectangle(p2, r.X + 2, r.Y + 2, r.Width - 5, r.Height - 5);
            }
        }
    }

    // Label supports transparent by default; keep safe solid background to avoid surprises.
    public class RetroLabel : Label
    {
        public RetroLabel()
        {
            ForeColor = Retro.Text;
            BackColor = Retro.Bg;
            Font = Retro.PixelFont(8f);
        }
    }

    public class RetroPanel : Panel
    {
        public RetroPanel()
        {
            DoubleBuffered = true;
            BackColor = Retro.Panel;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            Retro.UsePixel(g);
            g.Clear(BackColor);
            Retro.Bezel(g, new Rectangle(0, 0, Width, Height));
        }
    }

    public class RetroGroupBox : GroupBox
    {
        public RetroGroupBox()
        {
            DoubleBuffered = true;
            ForeColor = Retro.Accent2;
            BackColor = Retro.Bg; // solid, not transparent
            Font = Retro.PixelFont(8f, FontStyle.Bold);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            Retro.UsePixel(g);

            using (var bg = new SolidBrush(BackColor))
                g.FillRectangle(bg, ClientRectangle);

            var r = new Rectangle(4, 12, Width - 8, Height - 16);
            using (var b = new SolidBrush(Retro.Panel))
                g.FillRectangle(b, r);
            Retro.Bezel(g, r);

            using (var br = new SolidBrush(ForeColor))
                g.DrawString(Text?.ToUpperInvariant() ?? string.Empty, Font, br, new Rectangle(12, 0, Width - 24, 16), Retro.NearSF);
        }
    }

    [DefaultEvent("Click")]
    public class RetroButton : Control
    {
        private MouseState _state = MouseState.None;
        [Browsable(true)] public Color Accent { get; set; } = Retro.Accent;

        public RetroButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(120, 32);
            ForeColor = Retro.Text;
            BackColor = Retro.Panel; // solid
            Font = Retro.PixelFont(8f, FontStyle.Bold);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _state = MouseState.Over; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _state = MouseState.None; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _state = MouseState.Down; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _state = ClientRectangle.Contains(e.Location) ? MouseState.Over : MouseState.None; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; Retro.UsePixel(g);
            var r = new Rectangle(0, 0, Width, Height);

            using (var b = new SolidBrush(BackColor))
                g.FillRectangle(b, r);

            if (_state == MouseState.Over)
                using (var o = new SolidBrush(Color.FromArgb(32, Color.White))) g.FillRectangle(o, r);
            else if (_state == MouseState.Down)
                using (var o = new SolidBrush(Color.FromArgb(48, Color.Black))) g.FillRectangle(o, r);

            Retro.Bezel(g, r);

            using (var br = new SolidBrush(ForeColor))
                g.DrawString(Text, Font, br, r, Retro.CenterSF);
        }
    }

    [DefaultEvent("TextChanged")]
    public class RetroTextBox : Control
    {
        private readonly TextBox _tb = new TextBox();

        public RetroTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Height = 22;
            BackColor = Retro.Panel;    // solid
            ForeColor = Retro.Light;
            Font = Retro.PixelFont(8f);

            _tb.BorderStyle = BorderStyle.None;
            _tb.BackColor = Retro.Panel;
            _tb.ForeColor = Retro.Light;
            _tb.Location = new Point(4, 4);
            _tb.Width = Width - 8;
            _tb.TextChanged += (s, e) => OnTextChanged(e);
            Controls.Add(_tb);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Height = Math.Max(22, Height);
            _tb.Location = new Point(4, 4);
            _tb.Width = Width - 8;
            _tb.Height = Height - 8;
        }

        public override string Text
        {
            get => _tb.Text;
            set { _tb.Text = value; Invalidate(); }
        }

        [Browsable(true)]
        public bool UseSystemPasswordChar { get => _tb.UseSystemPasswordChar; set => _tb.UseSystemPasswordChar = value; }

        [Browsable(true)]
        public bool ReadOnly { get => _tb.ReadOnly; set => _tb.ReadOnly = value; }

        [Browsable(true)]
        public HorizontalAlignment TextAlign { get => _tb.TextAlign; set => _tb.TextAlign = value; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; Retro.UsePixel(g);
            var r = new Rectangle(0, 0, Width, Height);
            using (var b = new SolidBrush(BackColor)) g.FillRectangle(b, r);
            Retro.Bezel(g, r);
        }
    }

    [DefaultEvent("CheckedChanged")]
    public class RetroCheckBox : Control
    {
        private MouseState _state = MouseState.None;
        private bool _checked;
        public event EventHandler CheckedChanged;

        [Browsable(true)]
        public bool Checked
        {
            get => _checked;
            set { if (_checked == value) return; _checked = value; Invalidate(); CheckedChanged?.Invoke(this, EventArgs.Empty); }
        }

        public RetroCheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(140, 18);
            ForeColor = Retro.Text;
            BackColor = Retro.Bg;   // solid
            Font = Retro.PixelFont(8f);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _state = MouseState.Over; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _state = MouseState.None; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _state = MouseState.Down; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_state == MouseState.Down && ClientRectangle.Contains(e.Location)) Checked = !Checked;
            _state = ClientRectangle.Contains(e.Location) ? MouseState.Over : MouseState.None;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; Retro.UsePixel(g);

            var box = new Rectangle(0, 1, 16, 16);
            using (var b = new SolidBrush(Retro.Panel)) g.FillRectangle(b, box);
            using (var p = new Pen(Retro.Border, 2)) g.DrawRectangle(p, 0, 1, 15, 15);

            if (_state != MouseState.None)
                using (var o = new SolidBrush(Color.FromArgb(18, Color.White))) g.FillRectangle(o, box);

            if (Checked)
            {
                using (var br = new SolidBrush(Retro.Accent))
                {
                    g.FillRectangle(br, 4, 9, 2, 2);
                    g.FillRectangle(br, 6, 9, 2, 2);
                    g.FillRectangle(br, 8, 7, 2, 2);
                    g.FillRectangle(br, 10, 5, 2, 2);
                }
            }

            using (var br = new SolidBrush(ForeColor))
                g.DrawString(Text, Font, br, new Rectangle(20, 0, Width - 20, Height), Retro.NearSF);
        }
    }

    [DefaultEvent("CheckedChanged")]
    public class RetroRadioButton : Control
    {
        private MouseState _state = MouseState.None;
        private bool _checked;
        public event EventHandler CheckedChanged;

        [Browsable(true)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                if (_checked) UncheckSiblings();
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public RetroRadioButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(160, 18);
            ForeColor = Retro.Text;
            BackColor = Retro.Bg; // solid
            Font = Retro.PixelFont(8f);
            Cursor = Cursors.Hand;
        }

        private void UncheckSiblings()
        {
            if (Parent == null) return;
            foreach (Control c in Parent.Controls)
                if (c is RetroRadioButton rb && !ReferenceEquals(rb, this)) rb._checked = false;
            Parent.Invalidate(true);
        }

        protected override void OnMouseEnter(EventArgs e) { _state = MouseState.Over; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _state = MouseState.None; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _state = MouseState.Down; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_state == MouseState.Down && ClientRectangle.Contains(e.Location)) Checked = true;
            _state = ClientRectangle.Contains(e.Location) ? MouseState.Over : MouseState.None;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; Retro.UsePixel(g);

            var box = new Rectangle(0, 1, 16, 16);
            using (var b = new SolidBrush(Retro.Panel)) g.FillRectangle(b, box);
            using (var p = new Pen(Retro.Border, 2)) g.DrawRectangle(p, 0, 1, 15, 15);

            if (Checked)
                using (var br = new SolidBrush(Retro.Accent)) g.FillRectangle(br, 5, 6, 6, 6);

            using (var br = new SolidBrush(ForeColor))
                g.DrawString(Text, Font, br, new Rectangle(20, 0, Width - 20, Height), Retro.NearSF);
        }
    }

    public class RetroProgressBar : Control
    {
        private int _maximum = 100;
        private int _value = 0;

        [DefaultValue(100)]
        public int Maximum { get => _maximum; set { _maximum = Math.Max(1, value); if (_value > _maximum) _value = _maximum; Invalidate(); } }

        [DefaultValue(0)]
        public int Value { get => _value; set { _value = Math.Max(0, Math.Min(_maximum, value)); Invalidate(); } }

        [DefaultValue(true)]
        public bool ShowPercent { get; set; } = true;

        public RetroProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Height = 18;
            ForeColor = Retro.Text;
            BackColor = Retro.Panel; // solid
            Font = Retro.PixelFont(8f, FontStyle.Bold);
        }

        protected override void OnResize(EventArgs e) { base.OnResize(e); Height = 18; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; Retro.UsePixel(g);
            var r = new Rectangle(0, 0, Width, Height);
            using (var b = new SolidBrush(BackColor)) g.FillRectangle(b, r);
            Retro.Bezel(g, r);

            float pct = _value / (float)_maximum;
            int w = Math.Max(0, (int)((Width - 4) * pct));
            var fill = new Rectangle(2, 2, w, Height - 4);
            using (var b2 = new SolidBrush(Retro.Accent)) g.FillRectangle(b2, fill);

            if (ShowPercent)
                using (var br = new SolidBrush(ForeColor)) g.DrawString($"{(int)(pct * 100)}%", Font, br, r, Retro.CenterSF);
        }
    }

    public class RetroListBox : ListBox
    {
        public Color ItemBackColor { get; set; } = Retro.Panel;
        public Color ItemSelectedColor { get; set; } = Retro.Accent;
        public Color ItemForeColor { get; set; } = Retro.Text;

        public RetroListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = 16;
            IntegralHeight = false;
            BorderStyle = BorderStyle.None;
            BackColor = Retro.Panel;
            ForeColor = Retro.Text;
            Font = Retro.PixelFont(8f);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0) return;
            var g = e.Graphics; Retro.UsePixel(g);
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using (var b = new SolidBrush(selected ? ItemSelectedColor : ItemBackColor))
                g.FillRectangle(b, e.Bounds);

            using (var br = new SolidBrush(ItemForeColor))
                g.DrawString(" " + GetItemText(Items[e.Index]), Font, br, e.Bounds, new StringFormat { LineAlignment = StringAlignment.Center });

            e.DrawFocusRectangle();

            using (var p = new Pen(Retro.Border, 2))
                g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
        }
    }

    public class RetroComboBox : ComboBox
    {
        public Color ItemBackColor { get; set; } = Retro.Panel;
        public Color ItemSelectedColor { get; set; } = Retro.Accent;
        public Color ItemForeColor { get; set; } = Retro.Text;

        public RetroComboBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            ItemHeight = 18;
            FlatStyle = FlatStyle.Flat;
            BackColor = Retro.Panel;
            ForeColor = Retro.Text;
            Font = Retro.PixelFont(8f);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0) return;
            var g = e.Graphics; Retro.UsePixel(g);
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using (var b = new SolidBrush(selected ? ItemSelectedColor : ItemBackColor))
                g.FillRectangle(b, e.Bounds);

            using (var br = new SolidBrush(ItemForeColor))
                g.DrawString(GetItemText(Items[e.Index]), Font, br, e.Bounds, new StringFormat { LineAlignment = StringAlignment.Center });

            e.DrawFocusRectangle();
        }
    }

    public class RetroTabControl : TabControl
    {
        public Color TabBackColor { get; set; } = Retro.Panel;
        public Color TabActiveColor { get; set; } = Retro.Accent;
        public Color TabsAreaBackColor { get; set; } = Retro.Bg;
        public Color TabTextColor { get; set; } = Retro.Text;

        public RetroTabControl()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(120, 24);
            Font = Retro.PixelFont(8f, FontStyle.Bold);
            BackColor = TabsAreaBackColor;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var g = e.Graphics; Retro.UsePixel(g);
            var rect = GetTabRect(e.Index);

            using (var b = new SolidBrush(e.Index == SelectedIndex ? TabActiveColor : TabBackColor))
                g.FillRectangle(b, rect);

            Retro.Bezel(g, rect);

            using (var br = new SolidBrush(TabTextColor))
            {
                var text = TabPages[e.Index].Text;
                g.DrawString(text.ToUpperInvariant(), Font, br, rect, Retro.CenterSF);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (SelectedTab != null) SelectedTab.BackColor = TabsAreaBackColor;
        }
    }

    public class RetroColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Retro.Panel;
        public override Color MenuBorder => Retro.Border;
        public override Color MenuItemBorder => Retro.Border;
        public override Color MenuItemSelected => Retro.Accent;
        public override Color ImageMarginGradientBegin => Retro.Panel;
        public override Color ImageMarginGradientMiddle => Retro.Panel;
        public override Color ImageMarginGradientEnd => Retro.Panel;
        public override Color SeparatorDark => Retro.Border;
        public override Color ToolStripBorder => Retro.Border;
        public override Color ToolStripGradientBegin => Retro.Panel;
        public override Color ToolStripGradientMiddle => Retro.Panel;
        public override Color ToolStripGradientEnd => Retro.Panel;
        public override Color StatusStripGradientBegin => Retro.Panel;
        public override Color StatusStripGradientEnd => Retro.Panel;
    }

    public class RetroRenderer : ToolStripProfessionalRenderer
    {
        public RetroRenderer() : base(new RetroColorTable()) { }
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Retro.Text;
            e.TextFont = Retro.PixelFont(8f);
            base.OnRenderItemText(e);
        }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var g = e.Graphics; Retro.UsePixel(g);
            var r = new Rectangle(Point.Empty, e.Item.Bounds.Size);
            using (var b = new SolidBrush(e.Item.Selected ? Retro.Accent : Retro.Panel))
                g.FillRectangle(b, r);
            using (var p = new Pen(Retro.Border, 2))
                g.DrawRectangle(p, 0, 0, r.Width - 1, r.Height - 1);
        }
    }

    public class RetroContextMenuStrip : ContextMenuStrip
    {
        public RetroContextMenuStrip()
        {
            Renderer = new RetroRenderer();
            ShowImageMargin = false;
            Font = Retro.PixelFont(8f);
            ForeColor = Retro.Text;
            BackColor = Retro.Panel; // solid
        }
    }

    public class RetroToolStrip : ToolStrip
    {
        public RetroToolStrip()
        {
            Renderer = new RetroRenderer();
            Font = Retro.PixelFont(8f);
            ForeColor = Retro.Text;
            BackColor = Retro.Panel; // solid
        }
    }

    public class RetroStatusStrip : StatusStrip
    {
        public RetroStatusStrip()
        {
            Renderer = new RetroRenderer();
            Font = Retro.PixelFont(8f);
            ForeColor = Retro.Text;
            BackColor = Retro.Panel; // solid
            SizingGrip = false;
        }
    }
}
