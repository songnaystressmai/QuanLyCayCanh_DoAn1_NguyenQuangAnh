using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmLogin : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private ModernTextBox txtUser;
        private ModernTextBox txtPass;
        private ModernButton btnLogin;
        private ModernButton btnExit;
        private Label lblWarning;

        private NguoiDungBUS bus = new NguoiDungBUS();

        public frmLogin()
        {
            InitializeForm();
            InitializeLoginUI();
        }

        private void InitializeForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(560, 760);
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            this.Paint += FrmLogin_Paint;
            this.FormClosing += FrmLogin_FormClosing;
        }

        private void FrmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK) this.DialogResult = DialogResult.Cancel;
        }

        private void FrmLogin_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = this.ClientRectangle;
            using (var brush = new LinearGradientBrush(r,
                                                       Color.FromArgb(30, 64, 60),
                                                       Color.FromArgb(46, 204, 113),
                                                       LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, r);
            }

            using (var pen = new Pen(Color.FromArgb(25, Color.White)))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(pen, -120, -120, 300, 300);
                e.Graphics.DrawEllipse(pen, 420, 560, 260, 260);
            }
        }

        private void InitializeLoginUI()
        {
            Panel pnlCard = new Panel { Size = new Size(500, 620), Location = new Point((this.ClientSize.Width - 500) / 2, 70), BackColor = Color.White };
            pnlCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                int radius = 18;
                using (var path = RoundedRect(new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1), radius)) pnlCard.Region = new Region(path);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0))) e.Graphics.FillRectangle(shadowBrush, 6, 6, pnlCard.Width - 6, pnlCard.Height - 6);
            };
            this.Controls.Add(pnlCard);

            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 160, BackColor = Color.FromArgb(245, 247, 248) };
            pnlCard.Controls.Add(pnlHeader);

            Label lblLogo = new Label { Text = "🌿", Font = new Font("Segoe UI", 64), ForeColor = Color.FromArgb(46, 204, 113), AutoSize = false, Size = new Size(100, 100), TextAlign = ContentAlignment.MiddleCenter, Location = new Point((pnlCard.Width - 100) / 2, 10), BackColor = Color.Transparent };
            pnlHeader.Controls.Add(lblLogo);

            Label lblTitle = new Label { Text = "GARDEN STORE", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.FromArgb(33, 47, 61), AutoSize = false, TextAlign = ContentAlignment.TopCenter, Dock = DockStyle.Bottom, Height = 48, BackColor = pnlHeader.BackColor };
            pnlHeader.Controls.Add(lblTitle);

            Panel pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = pnlCard.BackColor, Padding = new Padding(36, 12, 36, 24) };
            pnlCard.Controls.Add(pnlContent);
            pnlContent.BringToFront();

            Label lblSub = new Label { Text = "Hệ thống quản lý cửa hàng cây cảnh", Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Height = 24, Dock = DockStyle.Top };
            pnlContent.Controls.Add(lblSub);
            pnlContent.Controls.Add(new Label { Height = 18, Dock = DockStyle.Top });

            Label lblUser = new Label { Text = "Tên đăng nhập", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(33, 47, 61), Location = new Point(10, 86), AutoSize = true };
            pnlContent.Controls.Add(lblUser);

            txtUser = new ModernTextBox { Size = new Size(pnlContent.Width - 72, 44), Location = new Point((pnlContent.Width - (pnlContent.Width - 72)) / 2, 110), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Placeholder = "Nhập tên đăng nhập", TextValue = "admin" };
            pnlContent.Controls.Add(txtUser);

            Label lblPass = new Label { Text = "Mật khẩu", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(33, 47, 61), Location = new Point(10, 168), AutoSize = true };
            pnlContent.Controls.Add(lblPass);

            txtPass = new ModernTextBox { Size = new Size(pnlContent.Width - 72, 44), Location = new Point((pnlContent.Width - (pnlContent.Width - 72)) / 2, 192), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Placeholder = "Nhập mật khẩu", IsPassword = true, TextValue = "123" };
            pnlContent.Controls.Add(txtPass);

            lblWarning = new Label { Text = "", ForeColor = Color.FromArgb(231, 76, 60), AutoSize = false, Size = new Size(pnlContent.Width - 20, 36), Location = new Point(10, 248), BackColor = Color.Transparent, Visible = false };
            pnlContent.Controls.Add(lblWarning);

            btnLogin = new ModernButton { Text = "✅ ĐĂNG NHẬP", Location = new Point(10, 296), Size = new Size(pnlContent.Width - 20, 52), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            btnLogin.Click += BtnLogin_Click;
            pnlContent.Controls.Add(btnLogin);

            btnExit = new ModernButton { Text = "❌ Thoát", Location = new Point(10, 362), Size = new Size(pnlContent.Width - 20, 44), BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            btnExit.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlContent.Controls.Add(btnExit);

            this.Resize += (s, e) => { pnlCard.Location = new Point((this.ClientSize.Width - pnlCard.Width) / 2, pnlCard.Location.Y); };

            pnlHeader.MouseDown += Drag_MouseDown;
            lblLogo.MouseDown += Drag_MouseDown;
            lblTitle.MouseDown += Drag_MouseDown;
            pnlCard.MouseDown += Drag_MouseDown;
        }

        private void Drag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e) => TryLogin();

        private void TryLogin()
        {
            lblWarning.Visible = false;
            string user = txtUser.TextValue?.Trim();
            string pass = txtPass.TextValue;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) { ShowWarning("Vui lòng nhập tài khoản và mật khẩu!"); return; }

            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "⏳ Đang xử lý...";

                var dt = bus.DangNhap(user, pass);

                if (dt != null && dt.Rows.Count > 0)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowWarning("Tài khoản hoặc mật khẩu không đúng!");
                    txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowWarning("Lỗi: " + ex.Message);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "✅ ĐĂNG NHẬP";
            }
        }

        private void ShowWarning(string message) { lblWarning.Text = "⚠️ " + message; lblWarning.Visible = true; }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter; path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter; path.AddArc(arc, 0, 90);
            arc.X = bounds.Left; path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // ModernTextBox control
    public class ModernTextBox : UserControl
    {
        private readonly TextBox inner;
        private string placeholder = "";
        private bool isPassword = false;
        private Color underlineColor = Color.FromArgb(200, 207, 214);
        private Color underlineFocusColor = Color.FromArgb(46, 204, 113);

        public ModernTextBox()
        {
            this.Height = 44;
            this.BackColor = Color.Transparent;
            inner = new TextBox { BorderStyle = BorderStyle.None, Location = new Point(6, 8), Width = this.Width - 12, Font = new Font("Segoe UI", 11), BackColor = Color.FromArgb(245, 246, 247) };
            inner.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inner.TextChanged += (s, e) => this.Invalidate();
            inner.Enter += (s, e) => this.Invalidate();
            inner.Leave += (s, e) => this.Invalidate();

            this.Controls.Add(inner);
            this.Padding = new Padding(0);
            this.Resize += (s, e) => inner.Width = this.Width - 12;
        }

        public string Placeholder { get => placeholder; set { placeholder = value; Invalidate(); } }
        public bool IsPassword { get => isPassword; set { isPassword = value; inner.UseSystemPasswordChar = value; } }
        public string TextValue { get => inner.Text; set => inner.Text = value; }
        public char PasswordChar { get => inner.PasswordChar; set { inner.PasswordChar = value; inner.UseSystemPasswordChar = true; } }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (SolidBrush b = new SolidBrush(Color.FromArgb(245, 246, 247))) e.Graphics.FillRectangle(b, 0, 0, this.Width, this.Height);
            Color c = inner.Focused ? underlineFocusColor : underlineColor;
            using (Pen p = new Pen(c, 2)) e.Graphics.DrawLine(p, 6, this.Height - 6, this.Width - 6, this.Height - 6);
            if (string.IsNullOrEmpty(inner.Text) && !string.IsNullOrEmpty(placeholder) && !inner.Focused)
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(150, 150, 150))) e.Graphics.DrawString(placeholder, inner.Font, b, new PointF(inner.Left + 2, inner.Top - 1));
            }
        }
    }

    // ModernButton control
    public class ModernButton : Button
    {
        private Color originalBackColor;
        private Color hoverBackColor;

        public ModernButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Cursor = Cursors.Hand;
            this.originalBackColor = this.BackColor;
            this.hoverBackColor = ControlPaint.Light(this.BackColor, 0.12f);
            this.Padding = new Padding(6);
        }

        protected override void OnHandleCreated(EventArgs e) { base.OnHandleCreated(e); originalBackColor = this.BackColor; hoverBackColor = ControlPaint.Light(this.BackColor, 0.12f); }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.Parent?.BackColor ?? SystemColors.Control);
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            int radius = 8;
            using (GraphicsPath path = RoundedRect(rect, radius))
            {
                using (SolidBrush brush = new SolidBrush(this.BackColor)) e.Graphics.FillPath(brush, path);
            }
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, rect, this.ForeColor, flags);
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); this.BackColor = hoverBackColor; this.Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); this.BackColor = originalBackColor; this.Invalidate(); }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter; path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter; path.AddArc(arc, 0, 90);
            arc.X = bounds.Left; path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}