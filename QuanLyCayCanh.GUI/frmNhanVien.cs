using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmNhanVien : frmBase
    {
        private DataGridView dgv;
        private TextBox txtTen, txtSDT, txtDiaChi;
        private Button btnThem, btnSua, btnXoa, btnReset;
        private NhanVienBUS bus = new NhanVienBUS();
        private NguoiDungBUS ndBus = new NguoiDungBUS();
        private ComboBox cboNguoiDung;

        public frmNhanVien()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(1100, 700);

            if (this.pnlHeader != null)
            {
                this.pnlHeader.Visible = true;
                this.lblHeaderTitle.Text = "👨‍💼 QUẢN LÝ THÔNG TIN NHÂN VIÊN";
            }

            this.Padding = new Padding(0, 10, 0, 0);

            SetupUI();
            LoadData();
            LoadNguoiDungOptions();
        }

        private void SetupUI()
        {
            Panel pnlMainWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 243, 247),
                Padding = new Padding(0, 45, 0, 0)
            };

            TableLayoutPanel mainTlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                BackColor = Color.Transparent
            };
            mainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380f));
            mainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            Panel pnlInput = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(25),
                Margin = new Padding(0, 0, 15, 0),
                AutoScroll = true
            };

            pnlInput.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.FromArgb(220, 226, 230), ButtonBorderStyle.Solid);
            };

            int y = 20;
            int spacing = 75;
            AddLabelAndTextBox(pnlInput, "Tên nhân viên:", y, ref txtTen); y += spacing;
            AddLabelAndTextBox(pnlInput, "Số điện thoại:", y, ref txtSDT); y += spacing;
            AddLabelAndTextBox(pnlInput, "Địa chỉ liên lạc:", y, ref txtDiaChi); y += 90;

            pnlInput.Controls.Add(new Label { Text = "Tài khoản liên kết:", Location = new Point(25, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            cboNguoiDung = new ComboBox { Location = new Point(25, y + 30), Width = 320, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlInput.Controls.Add(cboNguoiDung);

            int btnStartY = y + 90;
            btnThem = CreateBtn("➕ Thêm mới", Color.FromArgb(46, 204, 113), 25, btnStartY); btnStartY += 55;
            btnSua = CreateBtn("✏️ Sửa thông tin", Color.FromArgb(52, 152, 219), 25, btnStartY); btnStartY += 55;
            btnXoa = CreateBtn("❌ Xóa nhân viên", Color.FromArgb(231, 76, 60), 25, btnStartY); btnStartY += 55;
            btnReset = CreateBtn("🔄 Làm mới form", Color.FromArgb(149, 165, 166), 25, btnStartY);

            pnlInput.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnReset });

            Panel pnlGridContainer = new Panel { Dock = DockStyle.Fill };

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 45 }
            };

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 47, 61);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            pnlGridContainer.Controls.Add(dgv);

            mainTlp.Controls.Add(pnlInput, 0, 0);
            mainTlp.Controls.Add(pnlGridContainer, 1, 0);

            pnlMainWrapper.Controls.Add(mainTlp);
            this.Controls.Add(pnlMainWrapper);

            pnlMainWrapper.SendToBack();

            // events
            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnReset.Click += (s, e) => { txtTen.Clear(); txtSDT.Clear(); txtDiaChi.Clear(); };

            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = bus.LayTatCaNhanVien();
                if (dgv.Columns.Count > 0)
                {
                    if (dgv.Columns["Id"] != null) dgv.Columns["Id"].HeaderText = "Mã NV";
                    if (dgv.Columns["TenNhanVien"] != null) dgv.Columns["TenNhanVien"].HeaderText = "Tên nhân viên";
                    if (dgv.Columns["SoDienThoai"] != null) dgv.Columns["SoDienThoai"].HeaderText = "SĐT";
                    if (dgv.Columns["DiaChi"] != null) dgv.Columns["DiaChi"].HeaderText = "Địa chỉ";
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải nhân viên: " + ex.Message); }
        }

        private void LoadNguoiDungOptions()
        {
            try
            {
                var dt = ndBus.LayTatCaNguoiDung();
                cboNguoiDung.DataSource = dt;
                cboNguoiDung.DisplayMember = "TenDangNhap";
                cboNguoiDung.ValueMember = "Id";
            }
            catch { }
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTen.Text)) { MessageBox.Show("Tên nhân viên không được rỗng"); return; }
                int nguoiDungId = cboNguoiDung.SelectedValue != null ? Convert.ToInt32(cboNguoiDung.SelectedValue) : 0;
                if (bus.ThemNhanVien(txtTen.Text.Trim(), txtSDT.Text.Trim(), txtDiaChi.Text.Trim(), nguoiDungId))
                {
                    MessageBox.Show("Thêm thành công");
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;
            try
            {
                int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
                int nguoiDungId = cboNguoiDung.SelectedValue != null ? Convert.ToInt32(cboNguoiDung.SelectedValue) : 0;
                if (bus.CapNhatNhanVien(id, txtTen.Text.Trim(), txtSDT.Text.Trim(), txtDiaChi.Text.Trim(), nguoiDungId))
                {
                    MessageBox.Show("Cập nhật thành công");
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show("Xác nhận xóa?", "Hỏi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (bus.XoaNhanVien(id)) { LoadData(); }
            }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];
                txtTen.Text = row.Cells["TenNhanVien"].Value?.ToString();
                txtSDT.Text = row.Cells["SoDienThoai"].Value?.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();
                if (row.Cells["NguoiDungId"].Value != null && int.TryParse(row.Cells["NguoiDungId"].Value.ToString(), out int ndId))
                {
                    try { cboNguoiDung.SelectedValue = ndId; } catch { }
                }
            }
        }

        private void AddLabelAndTextBox(Panel p, string label, int y, ref TextBox txt)
        {
            Label lbl = new Label
            {
                Text = label,
                Location = new Point(25, y),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };
            p.Controls.Add(lbl);

            txt = new TextBox
            {
                Location = new Point(25, y + 30),
                Width = 320,
                Font = new Font("Segoe UI", 11)
            };
            p.Controls.Add(txt);
        }

        private Button CreateBtn(string text, Color bg, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(320, 45),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Name = "frmNhanVien";
            this.ResumeLayout(false);
        }

        private void frmNhanVien_Load(object sender, EventArgs e) { }
    }
}