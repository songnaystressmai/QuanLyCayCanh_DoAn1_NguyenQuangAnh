using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCayCanh.GUI
{
    public class SmoothPanel : Panel
    {
        public SmoothPanel() { this.DoubleBuffered = true; this.ResizeRedraw = true; }
    }

    public class frmDashboard : frmBase
    {
        public frmDashboard()
        {
            if (this.pnlHeader != null) { this.pnlHeader.Visible = true; this.lblHeaderTitle.Text = "📊 BẢNG ĐIỀU KHIỂN HỆ THỐNG"; }
            this.Padding = new Padding(0, 10, 0, 0);
            this.DoubleBuffered = true;
            SetupDashboard();
        }

        private void SetupDashboard()
        {
            SmoothPanel pnlMain = new SmoothPanel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(242, 245, 250), Padding = new Padding(0, 10, 0, 0) };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            FlowLayoutPanel flowStats = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 160, BackColor = Color.Transparent };
            pnlMain.Controls.Add(flowStats);

            CreateStatCard(flowStats, "Sản Phẩm", "350", Color.FromArgb(52, 152, 219), "📦");
            CreateStatCard(flowStats, "Đơn Hàng", "1,250", Color.FromArgb(46, 204, 113), "📋");
            CreateStatCard(flowStats, "Khách Hàng", "520", Color.FromArgb(155, 89, 182), "👥");
            CreateStatCard(flowStats, "Doanh Thu", "150M", Color.FromArgb(230, 126, 34), "💰");

            Panel spacer = new Panel { Dock = DockStyle.Top, Height = 20 };
            pnlMain.Controls.Add(spacer);
            spacer.BringToFront();

            SmoothPanel pnlInfo = new SmoothPanel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(0, 10, 0, 0) };
            pnlInfo.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInfo.ClientRectangle, Color.FromArgb(230, 230, 230), ButtonBorderStyle.Solid); };
            pnlMain.Controls.Add(pnlInfo);
            pnlInfo.BringToFront();

            Label lblInfo = new Label { Text = "📈 Hoạt Động Gần Đây", Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Dock = DockStyle.Top, Height = 50, TextAlign = ContentAlignment.MiddleLeft };
            pnlInfo.Controls.Add(lblInfo);

            ListBox lstActivities = new ListBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11f), BorderStyle = BorderStyle.None, ItemHeight = 35, ForeColor = Color.FromArgb(71, 84, 102), BackColor = Color.White };
            lstActivities.Items.Add(" ✅  Đơn hàng #1001 - Hoàn thành (12:30 - 15/04/2026)");
            lstActivities.Items.Add(" ✅  Thêm 50 cây Kim Tiền vào kho (10:15 - 15/04/2026)");
            lstActivities.Items.Add(" ✅  Tạo khuyến mãi 'Sale Hè 2026' (09:00 - 14/04/2026)");
            lstActivities.Items.Add(" ✅  Nhân viên Nguyễn Quang Anh đăng nhập (08:45 - 14/04/2026)");
            lstActivities.Items.Add(" ✅  Cập nhật giá sản phẩm Xương Rồng (17:00 - 13/04/2026)");
            pnlInfo.Controls.Add(lstActivities);
            lstActivities.BringToFront();
        }

        private void CreateStatCard(FlowLayoutPanel parent, string title, string value, Color bgColor, string icon)
        {
            Panel card = new Panel { Size = new Size(265, 135), BackColor = bgColor, Margin = new Padding(0, 0, 20, 10) };
            Label lblIcon = new Label { Text = icon, Font = new Font("Segoe UI", 35), ForeColor = Color.FromArgb(180, Color.White), Location = new Point(15, 15), AutoSize = true };
            Label lblTitle = new Label { Text = title.ToUpper(), Font = new Font("Segoe UI Semibold", 9), ForeColor = Color.White, Location = new Point(15, 100), AutoSize = true };
            Label lblValue = new Label { Text = value, Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.White, Location = new Point(120, 45), AutoSize = true };
            card.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblValue });
            parent.Controls.Add(card);
        }
    }
}