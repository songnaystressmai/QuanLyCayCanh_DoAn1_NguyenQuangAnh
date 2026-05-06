using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCayCanh.GUI
{
    public class frmAbout : frmBase
    {
        public frmAbout()
        {
            this.lblHeaderTitle.Text = "ℹ️ GIỚI THIỆU ỨNG DỤNG";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new Padding(0, 10, 0, 0);
            SetupUI();
        }

        private void SetupUI()
        {
            Panel pnlMain = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(40) };
            Label lblLogo = new Label { Text = "🌿", Font = new Font("Segoe UI", 48), Location = new Point(40, 40), AutoSize = true };
            Label lblBrand = new Label { Text = "GARDEN STORE", Font = new Font("Segoe UI", 26, FontStyle.Bold), ForeColor = Color.FromArgb(33, 47, 61), Location = new Point(130, 55), AutoSize = true };
            Label lblSlogan = new Label { Text = "Giải pháp quản lý cửa hàng cây cảnh chuyên nghiệp", Font = new Font("Segoe UI", 11, FontStyle.Italic), ForeColor = Color.FromArgb(46, 204, 113), Location = new Point(135, 100), AutoSize = true };
            Panel line = new Panel { Height = 2, Width = 580, BackColor = Color.FromArgb(235, 237, 239), Location = new Point(40, 160) };
            TableLayoutPanel tlpInfo = new TableLayoutPanel { Location = new Point(40, 180), Size = new Size(580, 200), ColumnCount = 2, RowCount = 5 };
            tlpInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f)); tlpInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70f));
            AddInfoRow(tlpInfo, "Phiên bản:", "1.1.0 (Enterprise Edition)");
            AddInfoRow(tlpInfo, "Ngày phát hành:", "15/04/2026");
            AddInfoRow(tlpInfo, "Tác giả:", "Nguyễn Quang Anh");
            AddInfoRow(tlpInfo, "Công nghệ:", "C# WinForms, SQL Server");
            AddInfoRow(tlpInfo, "Hỗ trợ:", "support@gardenstore.com");
            Label lblCopyright = new Label { Text = "© 2026 Garden Store Team. Bảo lưu mọi quyền.\nPhần mềm được phát triển cho mục đích quản lý nội bộ.", Font = new Font("Segoe UI", 9), ForeColor = Color.DarkGray, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Bottom, Height = 60 };
            pnlMain.Controls.AddRange(new Control[] { lblLogo, lblBrand, lblSlogan, line, tlpInfo, lblCopyright });
            this.Controls.Add(pnlMain); pnlMain.BringToFront();
        }

        private void AddInfoRow(TableLayoutPanel tlp, string label, string value)
        {
            Label lblTitle = new Label { Text = label, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            Label lblValue = new Label { Text = value, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(44, 62, 80), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            tlp.Controls.Add(lblTitle); tlp.Controls.Add(lblValue);
        }
    }
}