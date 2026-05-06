using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmBaoCao : frmBase
    {
        private DataGridView dgvReport;
        private DataTable dtReport;
        private Label lblDoanhThu, lblTopProduct, lblKhachMoi;
        private DonHangBUS donBus = new DonHangBUS();
        private CayCanhBUS cayBus = new CayCanhBUS();
        private KhachHangBUS khBus = new KhachHangBUS();
        private ChiTietDonHangBUS ctBus = new ChiTietDonHangBUS();

        public frmBaoCao()
        {
            this.lblHeaderTitle.Text = "📊 BÁO CÁO DOANH THU";
            this.Size = new Size(1000, 700);
            this.Padding = new Padding(0, 10, 0, 0);
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            Panel pnlMain = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(240, 243, 247), Padding = new Padding(0, 20, 0, 0) };

            TableLayoutPanel pnlStats = new TableLayoutPanel { Dock = DockStyle.Top, Height = 150, ColumnCount = 3, RowCount = 1, Padding = new Padding(0, 25, 0, 0) };
            pnlStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            pnlStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            pnlStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));

            // Card 1 - doanh thu
            Panel card1 = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(52, 152, 219), Margin = new Padding(5) };
            lblDoanhThu = new Label { Text = "0 VNĐ", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, Location = new Point(10, 40), AutoSize = true };
            card1.Controls.Add(new Label { Text = "Doanh Thu Tháng", Font = new Font("Segoe UI", 9), ForeColor = Color.White, Location = new Point(10, 10), AutoSize = true });
            card1.Controls.Add(lblDoanhThu);
            pnlStats.Controls.Add(card1);

            // Card 2 - top product
            Panel card2 = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(46, 204, 113), Margin = new Padding(5) };
            lblTopProduct = new Label { Text = "-", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.White, Location = new Point(10, 40), AutoSize = true };
            card2.Controls.Add(new Label { Text = "Sản Phẩm Bán Chạy", Font = new Font("Segoe UI", 9), ForeColor = Color.White, Location = new Point(10, 10), AutoSize = true });
            card2.Controls.Add(lblTopProduct);
            pnlStats.Controls.Add(card2);

            // Card 3 - khách mới
            Panel card3 = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(155, 89, 182), Margin = new Padding(5) };
            lblKhachMoi = new Label { Text = "0", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.White, Location = new Point(10, 40), AutoSize = true };
            card3.Controls.Add(new Label { Text = "Khách Hàng Mới", Font = new Font("Segoe UI", 9), ForeColor = Color.White, Location = new Point(10, 10), AutoSize = true });
            card3.Controls.Add(lblKhachMoi);
            pnlStats.Controls.Add(card3);

            Panel pnlAction = new Panel { Dock = DockStyle.Top, Height = 50 };
            Button btnExport = new Button { Text = "📤 Xuất Báo Cáo (.txt)", Size = new Size(160, 40), Location = new Point(0, 5), BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnExport.Click += (s, e) => ExportToTxt();
            pnlAction.Controls.Add(btnExport);

            dgvReport = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AllowUserToAddRows = false, RowTemplate = { Height = 40 }, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 47, 61);
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.EnableHeadersVisualStyles = false;

            pnlMain.Controls.Add(dgvReport);
            pnlMain.Controls.Add(pnlAction);
            pnlMain.Controls.Add(pnlStats);
            this.Controls.Add(pnlMain);
        }

        public void LoadData()
        {
            try
            {
                // doanh thu
                decimal revenue = donBus.GetTotalRevenue();
                lblDoanhThu.Text = revenue.ToString("N0") + " VNĐ";

                // top product
                var dtTop = cayBus.GetTopSellingProducts(1);
                if (dtTop != null && dtTop.Rows.Count > 0)
                {
                    lblTopProduct.Text = $"{dtTop.Rows[0]["TenCay"]} ({dtTop.Rows[0]["TongBan"]})";
                }
                else lblTopProduct.Text = "-";

                // số khách hàng tổng
                int khCount = khBus.GetKhachHangCount();
                lblKhachMoi.Text = khCount.ToString();

                // chi tiết bán hàng
                dtReport = ctBus.GetRecentSales();
                dgvReport.DataSource = dtReport;

                // format
                if (dgvReport.Columns.Contains("ThanhTien"))
                    dgvReport.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải báo cáo: " + ex.Message);
            }
        }

        private void ExportToTxt()
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Text File|*.txt", FileName = "BaoCaoDoanhThu.txt" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    sw.WriteLine("--- BÁO CÁO DOANH THU GARDEN STORE ---");
                    sw.WriteLine($"Ngày xuất: {DateTime.Now}");
                    sw.WriteLine("---------------------------------------");
                    foreach (DataColumn col in dtReport.Columns) sw.Write(col.ColumnName + "\t");
                    sw.WriteLine();
                    foreach (DataRow row in dtReport.Rows)
                    {
                        foreach (var item in row.ItemArray) sw.Write(item.ToString() + "\t");
                        sw.WriteLine();
                    }
                }
                MessageBox.Show("Xuất báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}