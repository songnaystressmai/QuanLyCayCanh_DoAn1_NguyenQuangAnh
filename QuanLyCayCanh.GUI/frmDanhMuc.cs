using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmDanhMuc : frmBase
    {
        private DataGridView dgv;
        private TextBox txtTenDanhMuc;
        private Button btnThem, btnXoa, btnReset;
        private DanhMucBUS bus = new DanhMucBUS();

        public frmDanhMuc()
        {
            if (this.lblHeaderTitle != null) { this.lblHeaderTitle.Text = "📁 QUẢN LÝ DANH MỤC CÂY"; this.lblHeaderTitle.Visible = true; }
            this.Size = new Size(1100, 700); this.BackColor = Color.FromArgb(242, 245, 250);
            this.Padding = new Padding(0, 10, 0, 0);
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            Panel pnlWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0), BackColor = Color.Transparent };
            TableLayoutPanel mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.Transparent };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350f)); mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            Panel pnlInputCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(25), Margin = new Padding(0, 0, 20, 0) };
            pnlInputCard.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInputCard.ClientRectangle, Color.FromArgb(230, 230, 230), ButtonBorderStyle.Solid); };

            Label lblHeading = new Label { Text = "Thông Tin", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), AutoSize = true, Location = new Point(25, 25) };
            Label lblHint = new Label { Text = "Tên danh mục:", Location = new Point(25, 75), Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, AutoSize = true };

            txtTenDanhMuc = new TextBox { Location = new Point(25, 100), Width = 290, Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            btnThem = CreateStyledButton("Thêm mới", Color.FromArgb(46, 204, 113), 25, 160);
            btnXoa = CreateStyledButton("Xóa chọn", Color.FromArgb(231, 76, 60), 25, 215);
            btnReset = CreateStyledButton("Làm mới", Color.FromArgb(52, 152, 219), 25, 270);

            btnThem.Click += (s, e) => ThemDanhMuc();
            btnXoa.Click += (s, e) => XoaDanhMuc();
            btnReset.Click += (s, e) => { txtTenDanhMuc.Clear(); txtTenDanhMuc.Focus(); };

            pnlInputCard.Controls.AddRange(new Control[] { lblHeading, lblHint, txtTenDanhMuc, btnThem, btnXoa, btnReset });

            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, ReadOnly = true, RowTemplate = { Height = 45 }, GridColor = Color.FromArgb(240, 240, 240), EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            dgv.ColumnHeadersHeight = 50;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 244, 253);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(26, 115, 232);

            dgv.CellClick += (s, e) => { if (e.RowIndex >= 0 && dgv.Rows[e.RowIndex].Cells["TenDanhMuc"].Value != null) txtTenDanhMuc.Text = dgv.Rows[e.RowIndex].Cells["TenDanhMuc"].Value.ToString(); };

            mainLayout.Controls.Add(pnlInputCard, 0, 0);
            mainLayout.Controls.Add(dgv, 1, 0);
            pnlWrapper.Controls.Add(mainLayout);
            this.Controls.Add(pnlWrapper);
            pnlWrapper.BringToFront();
        }

        private Button CreateStyledButton(string text, Color bg, int x, int y)
        {
            Button btn = new Button { Text = text, Location = new Point(x, y), Size = new Size(290, 45), BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10), Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0; return btn;
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = bus.LayTatCaDanhMuc();
                if (dgv.Columns["TenDanhMuc"] != null) dgv.Columns["TenDanhMuc"].HeaderText = "Tên Loại Cây";
                if (dgv.Columns["Id"] != null) dgv.Columns["Id"].HeaderText = "Mã";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void ThemDanhMuc()
        {
            if (string.IsNullOrWhiteSpace(txtTenDanhMuc.Text)) return;
            if (bus.ThemDanhMuc(txtTenDanhMuc.Text)) { txtTenDanhMuc.Clear(); LoadData(); }
        }

        private void XoaDanhMuc()
        {
            if (dgv.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show("Xác nhận xóa?", "Hỏi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (bus.XoaDanhMuc(id)) LoadData();
            }
        }
    }
}