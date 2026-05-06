using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCayCanh.GUI
{
    public class frmMain : Form
    {
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Form currentForm;
        private Label lblCurrentUser;

        public frmMain()
        {
            this.Size = new Size(1450, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "🌿 GARDEN STORE - Hệ thống quản lý cửa hàng cây cảnh";
            this.Icon = SystemIcons.Application;
            this.BackColor = Color.FromArgb(240, 243, 247);
            this.FormClosing += (s, e) => Application.Exit();

            SetupUI();
        }

        private void SetupUI()
        {
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(33, 47, 61)
            };

            Label lblTitle = new Label
            {
                Text = "🌿 GARDEN STORE",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            pnlHeader.Controls.Add(lblTitle);

            lblCurrentUser = new Label
            {
                Text = "👤 Đã đăng nhập",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(189, 195, 199),
                Location = new Point(1250, 25),
                Size = new Size(140, 20),
                TextAlign = ContentAlignment.MiddleRight
            };
            pnlHeader.Controls.Add(lblCurrentUser);

            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(44, 62, 80),
                AutoScroll = true
            };

            CreateMenuItems();

            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 243, 247)
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlHeader);

            LoadForm(new frmDashboard());
        }

        private void CreateMenuItems()
        {
            string[] menus = {
                "📊 Bảng Điều Khiển",
                "🛒 Bán Hàng",
                "🌿 Cây Cảnh",
                "📁 Danh Mục",
                "👥 Khách Hàng",
                "👨‍💼 Nhân Viên",
                "📦 Đơn Hàng",
                "🎁 Khuyến Mãi",
                "📊 Báo Cáo",
                "📜 Lịch Sử GD",
                "👤 Tài Khoản",
                "📥 Nhập Kho",
                "ℹ️ Về Ứng Dụng"
            };

            int y = 20;
            foreach (string menu in menus)
            {
                Button btn = CreateMenuButton(menu, y);
                btn.Click += (s, e) => MenuClick(btn);
                pnlSidebar.Controls.Add(btn);
                y += 65;
            }

            Button btnLogout = new Button
            {
                Text = "🚪 Đăng Xuất",
                Location = new Point(0, y + 50),
                Size = new Size(250, 60),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BackColor = Color.FromArgb(231, 76, 60),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                this.Close();
                Application.Restart();
            };
            pnlSidebar.Controls.Add(btnLogout);
        }

        private Button CreateMenuButton(string text, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(0, y),
                Size = new Size(250, 60),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(189, 195, 199),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BackColor = Color.FromArgb(44, 62, 80),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(52, 73, 94);
                btn.ForeColor = Color.White;
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(44, 62, 80);
                btn.ForeColor = Color.FromArgb(189, 195, 199);
            };

            return btn;
        }

        private void MenuClick(Button btn)
        {
            Form f = null;
            string menuText = btn.Text;

            if (menuText.Contains("Bảng Điều Khiển")) f = new frmDashboard();
            else if (menuText.Contains("Bán Hàng")) f = new frmBanHang();
            else if (menuText.Contains("Cây Cảnh")) f = new frmCayCanh();
            else if (menuText.Contains("Danh Mục")) f = new frmDanhMuc();
            else if (menuText.Contains("Khách Hàng")) f = new frmKhachHang();
            else if (menuText.Contains("Nhân Viên")) f = new frmNhanVien();
            else if (menuText.Contains("Đơn Hàng")) f = new frmDonHang();
            else if (menuText.Contains("Khuyến Mãi")) f = new frmKhuyenMai();
            else if (menuText.Contains("Báo Cáo")) f = new frmBaoCao();
            else if (menuText.Contains("Lịch Sử")) f = new frmLichSuDH();
            else if (menuText.Contains("Tài Khoản")) f = new frmNguoiDung();
            else if (menuText.Contains("Nhập Kho")) f = new frmNhapKho();
            else if (menuText.Contains("Về")) f = new frmAbout();

            if (f != null) LoadForm(f);
        }

        private void LoadForm(Form f)
        {
            if (currentForm != null && !currentForm.IsDisposed)
            {
                currentForm.Close();
                currentForm.Dispose();
            }

            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(f);
            f.Show();
            currentForm = f;
        }
    }
}