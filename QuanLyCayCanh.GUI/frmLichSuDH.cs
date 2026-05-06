using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmLichSuDH : frmBase
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private BindingSource bindingSource = new BindingSource();
        private LichSuDonHangBUS bus = new LichSuDonHangBUS();
        private string placeholderText = "Tìm kiếm theo mã GD, khách hàng...";

        public frmLichSuDH()
        {
            this.lblHeaderTitle.Text = "📜 LỊCH SỬ GIAO DỊCH";
            this.Size = new Size(1000, 650);
            this.Padding = new Padding(0, 10, 0, 0);
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            int gap = 20;

            Panel pnlMain = new Panel
            {
                BackColor = Color.FromArgb(240, 243, 247),
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 45, 0, 0)
            };

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Margin = new Padding(0, 0, 0, 20) };

            txtSearch = new TextBox
            {
                Text = placeholderText,
                ForeColor = Color.Gray,
                Size = new Size(300, 30),
                Location = new Point(15, 15),
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            txtSearch.KeyDown += TxtSearch_KeyDown;

            Button btnSearch = new Button
            {
                Text = "🔍 Tìm",
                Location = new Point(325, 14),
                Size = new Size(80, 32),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            pnlTop.Controls.Add(txtSearch);
            pnlTop.Controls.Add(btnSearch);
            pnlMain.Controls.Add(pnlTop);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowTemplate = { Height = 40 },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false
            };

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 47, 61);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);
            dgv.CellFormatting += Dgv_CellFormatting;

            pnlMain.Controls.Add(dgv);
            this.Controls.Add(pnlMain);
        }

        public void LoadData()
        {
            try
            {
                DataTable dt = bus.LayTatCaLichSuDonHang();
                bindingSource.DataSource = dt;
                dgv.DataSource = bindingSource;

                // format columns
                if (dgv.Columns.Contains("ThoiGian"))
                {
                    dgv.Columns["ThoiGian"].HeaderText = "Thời Gian";
                }
                if (dgv.Columns.Contains("TenTrangThai"))
                {
                    dgv.Columns["TenTrangThai"].HeaderText = "Trạng Thái";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch sử: " + ex.Message);
            }
        }

        // Search behavior
        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword == placeholderText || string.IsNullOrWhiteSpace(keyword))
            {
                bindingSource.Filter = string.Empty;
            }
            else
            {
                string escaped = keyword.Replace("'", "''");
                DataTable dt = (DataTable)bindingSource.DataSource;
                var filters = dt.Columns.Cast<DataColumn>()
                    .Where(c => c.DataType == typeof(string))
                    .Select(c => $"[{c.ColumnName}] LIKE '%{escaped}%'");
                bindingSource.Filter = string.Join(" OR ", filters);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e) => PerformSearch();

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { PerformSearch(); e.SuppressKeyPress = true; }
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == placeholderText) { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = placeholderText; txtSearch.ForeColor = Color.Gray; }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgv.Columns[e.ColumnIndex].Name == "TenTrangThai" && e.Value != null)
            {
                if (e.Value.ToString().Contains("Hoàn")) { e.CellStyle.ForeColor = Color.FromArgb(39, 174, 96); e.CellStyle.Font = new Font(dgv.Font, FontStyle.Bold); }
                else e.CellStyle.ForeColor = Color.FromArgb(230, 126, 34);
            }
        }

        public void RefreshData() => LoadData();

        private void InitializeComponent()
        {
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            // 
            // frmLichSuDH
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.ClientSize = new System.Drawing.Size(1125, 934);
            this.Name = "frmLichSuDH";
            this.Load += new System.EventHandler(this.frmLichSuDH_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        private void frmLichSuDH_Load(object sender, EventArgs e)
        {

        }
    }
}