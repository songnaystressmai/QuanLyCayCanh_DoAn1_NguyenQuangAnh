using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmKhachHang : frmBase
    {
        private DataGridView dgv;
        private TextBox txtTen, txtSDT;
        private Button btnThem, btnReset;
        private KhachHangBUS bus = new KhachHangBUS();

        public frmKhachHang()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(1200, 650);

            if (this.pnlHeader != null) { this.pnlHeader.Visible = true; this.lblHeaderTitle.Text = "👥 QUẢN LÝ DANH SÁCH KHÁCH HÀNG"; }
            this.Padding = new Padding(0, 10, 0, 0);

            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            Panel pnlMainWrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(240, 243, 247), Padding = new Padding(0, 45, 0, 0) };
            TableLayoutPanel mainTlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.Transparent };
            mainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350f));
            mainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            Panel pnlInput = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20), Margin = new Padding(0, 0, 10, 0) };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.FromArgb(220, 226, 230), ButtonBorderStyle.Solid); };

            Font fLabel = new Font("Segoe UI", 10, FontStyle.Bold);
            Font fInput = new Font("Segoe UI", 11);

            pnlInput.Controls.Add(new Label { Text = "Tên khách hàng:", Location = new Point(20, 30), Font = fLabel, AutoSize = true });
            txtTen = new TextBox { Location = new Point(20, 55), Width = 300, Font = fInput };
            pnlInput.Controls.Add(txtTen);

            pnlInput.Controls.Add(new Label { Text = "Số điện thoại:", Location = new Point(20, 110), Font = fLabel, AutoSize = true });
            txtSDT = new TextBox { Location = new Point(20, 135), Width = 300, Font = fInput };
            pnlInput.Controls.Add(txtSDT);

            btnThem = CreateStyledButton("➕ Thêm khách hàng", Color.FromArgb(46, 204, 113), new Point(20, 200), 300, 45);
            btnThem.Click += (s, e) => ThemKhachHang();
            pnlInput.Controls.Add(btnThem);

            btnReset = CreateStyledButton("🔄 Làm mới form", Color.FromArgb(149, 165, 166), new Point(20, 260), 300, 45);
            btnReset.Click += (s, e) => { txtTen.Clear(); txtSDT.Clear(); };
            pnlInput.Controls.Add(btnReset);

            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowTemplate = { Height = 45 } };
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 47, 61);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            mainTlp.Controls.Add(pnlInput, 0, 0);
            mainTlp.Controls.Add(dgv, 1, 0);
            pnlMainWrapper.Controls.Add(mainTlp);
            this.Controls.Add(pnlMainWrapper);
            pnlMainWrapper.SendToBack();
        }

        private Button CreateStyledButton(string text, Color backColor, Point loc, int w, int h)
        {
            Button btn = new Button { Text = text, Location = loc, Size = new Size(w, h), BackColor = backColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = bus.LayTatCaKhachHang();
                if (dgv.Columns.Count >= 2)
                {
                    dgv.Columns[0].HeaderText = "Mã số";
                    dgv.Columns[1].HeaderText = "Họ và Tên";
                    dgv.Columns[2].HeaderText = "Số điện thoại";
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void ThemKhachHang()
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text) || string.IsNullOrWhiteSpace(txtSDT.Text)) { MessageBox.Show("Vui lòng nhập đầy đủ tên và số điện thoại!", "Thông báo"); return; }
            if (bus.ThemKhachHang(txtTen.Text, txtSDT.Text)) { MessageBox.Show("✅ Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); txtTen.Clear(); txtSDT.Clear(); LoadData(); }
        }
    }
}