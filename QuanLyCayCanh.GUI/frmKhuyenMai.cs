using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmKhuyenMai : frmBase
    {
        private DataGridView dgv;
        private TextBox txtTen, txtPhanTram;
        private DateTimePicker dtpBatDau, dtpKetThuc;
        private Button btnThem, btnSua, btnXoa, btnReset;
        private KhuyenMaiBUS bus = new KhuyenMaiBUS();

        public frmKhuyenMai()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(1100, 700);
            if (this.pnlHeader != null) { this.pnlHeader.Visible = true; this.lblHeaderTitle.Text = "🎁 QUẢN LÝ KHUYẾN MÃI"; }
            this.Padding = new Padding(0, 10, 0, 0);
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            Panel pnlMainWrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(240, 243, 247), Padding = new Padding(0, 45, 0, 0) };
            TableLayoutPanel mainTlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.Transparent };
            mainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380f)); mainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            Panel pnlInput = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(25), Margin = new Padding(0, 0, 15, 0), AutoScroll = true };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.FromArgb(220, 226, 230), ButtonBorderStyle.Solid); };

            int y = 20; int spacing = 80;
            AddLabelAndTextBox(pnlInput, "Tên chương trình:", y, ref txtTen); y += spacing;
            AddLabelAndTextBox(pnlInput, "% Giảm giá:", y, ref txtPhanTram); y += spacing;
            AddLabelAndDatePicker(pnlInput, "Ngày bắt đầu:", y, ref dtpBatDau); y += spacing;
            AddLabelAndDatePicker(pnlInput, "Ngày kết thúc:", y, ref dtpKetThuc); y += 40;

            y += 50;
            btnThem = CreateBtn("➕ Thêm mới", Color.FromArgb(46, 204, 113), 25, y); y += 55;
            btnSua = CreateBtn("✏️ Sửa thông tin", Color.FromArgb(52, 152, 219), 25, y); y += 55;
            btnXoa = CreateBtn("❌ Xóa khuyến mãi", Color.FromArgb(231, 76, 60), 25, y); y += 55;
            btnReset = CreateBtn("🔄 Làm mới form", Color.FromArgb(149, 165, 166), 25, y);

            pnlInput.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnReset });

            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowTemplate = { Height = 45 } };
            dgv.EnableHeadersVisualStyles = false; dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 47, 61); dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); dgv.ColumnHeadersHeight = 45;
            dgv.CellClick += Dgv_CellClick;

            mainTlp.Controls.Add(pnlInput, 0, 0);
            mainTlp.Controls.Add(dgv, 1, 0);
            pnlMainWrapper.Controls.Add(mainTlp);
            this.Controls.Add(pnlMainWrapper);
            pnlMainWrapper.SendToBack();

            btnThem.Click += (s, e) => { if (bus.ThemKhuyenMai(txtTen.Text.Trim(), int.TryParse(txtPhanTram.Text.Replace("%", ""), out int p) ? p : 0, dtpBatDau.Value, dtpKetThuc.Value)) { MessageBox.Show("Thêm thành công"); LoadData(); } };
            btnSua.Click += (s, e) => MessageBox.Show("Chức năng sửa có thể được thêm tương tự");
            btnXoa.Click += (s, e) => { if (dgv.SelectedRows.Count > 0) { int id = Convert.ToInt32(dgv.SelectedRows[0].Cells[0].Value); if (MessageBox.Show("Xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes) { /* call BUS */ } } };
            btnReset.Click += (s, e) => ResetForm();
        }

        private void AddLabelAndTextBox(Panel p, string label, int y, ref TextBox txt) { p.Controls.Add(new Label { Text = label, Location = new Point(25, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true }); txt = new TextBox { Location = new Point(25, y + 30), Width = 320, Font = new Font("Segoe UI", 11) }; p.Controls.Add(txt); }

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
            // frmKhuyenMai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.ClientSize = new System.Drawing.Size(1125, 934);
            this.Name = "frmKhuyenMai";
            this.Load += new System.EventHandler(this.frmKhuyenMai_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        private void frmKhuyenMai_Load(object sender, EventArgs e)
        {

        }

        private void AddLabelAndDatePicker(Panel p, string label, int y, ref DateTimePicker dtp) { p.Controls.Add(new Label { Text = label, Location = new Point(25, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true }); dtp = new DateTimePicker { Location = new Point(25, y + 30), Width = 320, Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Short }; p.Controls.Add(dtp); }
        private Button CreateBtn(string text, Color bg, int x, int y) { Button btn = new Button { Text = text, Location = new Point(x, y), Size = new Size(320, 45), BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand }; btn.FlatAppearance.BorderSize = 0; return btn; }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgv.Rows[e.RowIndex];

                // Dùng tên cột thực từ BUS: TenKhuyenMai, PhanTramGiam, NgayBatDau, NgayKetThuc
                txtTen.Text = row.Cells["TenKhuyenMai"]?.Value?.ToString() ?? "";
                txtPhanTram.Text = row.Cells["PhanTramGiam"]?.Value?.ToString() ?? "";

                DateTime bd, kt;
                if (DateTime.TryParse(row.Cells["NgayBatDau"]?.Value?.ToString(), out bd))
                    dtpBatDau.Value = bd;
                if (DateTime.TryParse(row.Cells["NgayKetThuc"]?.Value?.ToString(), out kt))
                    dtpKetThuc.Value = kt;
            }
        }

        private void ResetForm() { txtTen.Clear(); txtPhanTram.Clear(); dtpBatDau.Value = DateTime.Now; dtpKetThuc.Value = DateTime.Now; }

        private void LoadData()
        {
            var dt = bus.LayTatCaKhuyenMai(); // trả về TenKhuyenMai, PhanTramGiam, NgayBatDau, NgayKetThuc
            dgv.DataSource = dt;
            if (dgv.Columns["TenKhuyenMai"] != null) dgv.Columns["TenKhuyenMai"].HeaderText = "Tên chương trình";
            if (dgv.Columns["PhanTramGiam"] != null) dgv.Columns["PhanTramGiam"].HeaderText = "% Giảm";
            try
            {
                dgv.DataSource = bus.LayTatCaKhuyenMai();
            }
            catch { }
        }
    }
}