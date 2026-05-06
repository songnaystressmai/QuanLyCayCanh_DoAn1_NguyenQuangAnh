using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmDonHang : frmBase
    {
        private DataGridView dgv;
        private DonHangBUS bus = new DonHangBUS();
        private Button btnXuatHD;
        private Label lblTotalCount;

        public frmDonHang()
        {
            this.lblHeaderTitle.Text = "📦 QUẢN LÝ HÓA ĐƠN HỆ THỐNG";
            this.Size = new Size(1100, 700);
            this.Padding = new Padding(0, 10, 0, 0);

            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            Panel pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0), BackColor = Color.FromArgb(240, 243, 247) };

            Panel pnlActions = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Margin = new Padding(0, 0, 0, 15) };

            btnXuatHD = new Button
            {
                Text = "📄 Xuất hóa đơn (.txt)",
                Size = new Size(180, 40),
                Location = new Point(15, 10),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnXuatHD.FlatAppearance.BorderSize = 0;
            btnXuatHD.Click += BtnXuatHD_Click;

            lblTotalCount = new Label
            {
                Text = "Tổng số hóa đơn: 0",
                AutoSize = true,
                Location = new Point(850, 20),
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.DimGray,
                Anchor = AnchorStyles.Right
            };

            pnlActions.Controls.AddRange(new Control[] { btnXuatHD, lblTotalCount });

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 45 }
            };

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 50;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 244, 244);

            pnlMain.Controls.Add(dgv);
            pnlMain.Controls.Add(pnlActions);
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();
        }

        public void LoadData()
        {
            try
            {
                DataTable dt = bus.LayTatCaDonHang();
                dgv.DataSource = dt;
                lblTotalCount.Text = $"Tổng số hóa đơn: {dt.Rows.Count}";
                // Format columns if present
                if (dgv.Columns.Contains("TongTien"))
                {
                    dgv.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void BtnXuatHD_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow row = dgv.SelectedRows[0];
                string maHD = row.Cells[0].Value.ToString();
                string khachHang = row.Cells[1].Value.ToString();
                string nhanVien = row.Cells[2].Value.ToString();
                string tongTien = row.Cells[3].Value.ToString();
                string ngayLap = row.Cells[4].Value.ToString();

                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Text File|*.txt", FileName = $"HoaDon_{maHD}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName))
                        {
                            sw.WriteLine("==========================================");
                            sw.WriteLine("            GARDEN STORE INVOICE          ");
                            sw.WriteLine("==========================================");
                            sw.WriteLine($"Mã hóa đơn: {maHD}");
                            sw.WriteLine($"Ngày lập  : {ngayLap}");
                            sw.WriteLine($"Khách hàng: {khachHang}");
                            sw.WriteLine($"Nhân viên : {nhanVien}");
                            sw.WriteLine("------------------------------------------");
                            sw.WriteLine($"TỔNG CỘNG : {tongTien} VNĐ");
                            sw.WriteLine("==========================================");
                            sw.WriteLine("      Cảm ơn quý khách đã ủng hộ cửa hàng! ");
                        }
                        MessageBox.Show("Đã xuất hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất file: " + ex.Message);
            }
        }

        // wrapper for public refresh
        public void RefreshData()
        {
            LoadData();
        }
    }
}