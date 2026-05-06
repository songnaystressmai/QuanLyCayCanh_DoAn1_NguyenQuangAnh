using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmNguoiDung : frmBase
    {
        private DataGridView dgv;
        private TextBox txtUsername;
        private ComboBox cboRole;
        private DateTimePicker dtpDate;
        private DataTable dtNguoiDung;
        private NguoiDungBUS bus = new NguoiDungBUS();

        public frmNguoiDung()
        {
            this.lblHeaderTitle.Text = "👤 QUẢN LÝ TÀI KHOẢN";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new Padding(0, 10, 0, 0);

            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            Panel wrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 60, 0, 0), BackColor = Color.FromArgb(245, 247, 250) };
            this.Controls.Add(wrapper);

            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 700 };
            wrapper.Controls.Add(split);

            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            split.Panel1.Controls.Add(dgv);

            TableLayoutPanel rightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220)); rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            split.Panel2.Controls.Add(rightLayout);

            GroupBox gb = new GroupBox { Text = "Thông tin tài khoản", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold), Padding = new Padding(15) };
            TableLayoutPanel form = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 2, RowCount = 3, AutoSize = true };
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            txtUsername = new TextBox { Dock = DockStyle.Fill };
            cboRole = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            dtpDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };

            form.Controls.Add(new Label { Text = "Tên:", Anchor = AnchorStyles.Left }, 0, 0);
            form.Controls.Add(txtUsername, 1, 0);
            form.Controls.Add(new Label { Text = "Vai trò:", Anchor = AnchorStyles.Left }, 0, 1);
            form.Controls.Add(cboRole, 1, 1);
            form.Controls.Add(new Label { Text = "Ngày:", Anchor = AnchorStyles.Left }, 0, 2);
            form.Controls.Add(dtpDate, 1, 2);

            gb.Controls.Add(form);
            rightLayout.Controls.Add(gb, 0, 0);

            FlowLayoutPanel pnlBtn = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(10) };
            Button btnNew = CreateButton("Thêm mới", Color.FromArgb(52, 152, 219));
            Button btnSave = CreateButton("Lưu", Color.FromArgb(46, 204, 113));
            Button btnDelete = CreateButton("Xóa", Color.FromArgb(231, 76, 60));
            Button btnClear = CreateButton("Làm mới", Color.FromArgb(149, 165, 166));
            pnlBtn.Controls.AddRange(new Control[] { btnNew, btnSave, btnDelete, btnClear });

            pnlBtn.Resize += (s, e) => { foreach (Control c in pnlBtn.Controls) c.Width = pnlBtn.Width - 25; };
            rightLayout.Controls.Add(pnlBtn, 0, 1);

            dgv.SelectionChanged += Dgv_SelectionChanged;
            btnNew.Click += (s, e) => { dgv.ClearSelection(); ClearInputs(); };
            btnClear.Click += (s, e) => ClearInputs();
            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;

            // load roles
            try { var dtRoles = bus.LayVaiTro(); cboRole.DataSource = dtRoles; cboRole.DisplayMember = "TenVaiTro"; cboRole.ValueMember = "Id"; } catch { }
        }

        private Button CreateButton(string text, Color color)
        {
            return new Button { Text = text, BackColor = color, ForeColor = Color.White, Height = 50, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, FlatAppearance = { BorderSize = 0 }, Margin = new Padding(5) };
        }

        private void LoadData()
        {
            try { dtNguoiDung = bus.LayTatCaNguoiDung(); dgv.DataSource = dtNguoiDung; } catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách người dùng: " + ex.Message); }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text)) { MessageBox.Show("Nhập tên đăng nhập!"); return; }
            try
            {
                if (dgv.SelectedRows.Count > 0)
                {
                    DataRow row = ((DataRowView)dgv.SelectedRows[0].DataBoundItem).Row;
                    int id = Convert.ToInt32(row["Id"]);
                    string ten = txtUsername.Text.Trim();
                    int vaiTroId = cboRole.SelectedValue != null ? Convert.ToInt32(cboRole.SelectedValue) : 0;
                    if (bus.CapNhatNguoiDung(id, ten, vaiTroId)) { MessageBox.Show("Cập nhật thành công!"); LoadData(); }
                }
                else
                {
                    string pwd = Microsoft.VisualBasic.Interaction.InputBox("Nhập mật khẩu cho người dùng mới (Để trống sẽ dùng '123'):", "Mật khẩu", "123");
                    if (string.IsNullOrEmpty(pwd)) pwd = "123";
                    int vaiTroId = cboRole.SelectedValue != null ? Convert.ToInt32(cboRole.SelectedValue) : 0;
                    if (bus.ThemNguoiDung(txtUsername.Text.Trim(), pwd, vaiTroId)) { MessageBox.Show("Thêm người dùng thành công!"); LoadData(); ClearInputs(); }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                DataRow row = ((DataRowView)dgv.SelectedRows[0].DataBoundItem).Row;
                int id = Convert.ToInt32(row["Id"]);
                if (MessageBox.Show("Xóa người dùng này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try { if (bus.XoaNguoiDung(id)) { MessageBox.Show("Xóa thành công!"); LoadData(); ClearInputs(); } }
                    catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
                }
            }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                var row = ((DataRowView)dgv.SelectedRows[0].DataBoundItem).Row;
                txtUsername.Text = row["TenDangNhap"].ToString();
                if (int.TryParse(row["VaiTroId"]?.ToString(), out int vt)) { try { cboRole.SelectedValue = vt; } catch { } }
                if (DateTime.TryParse(row.Table.Columns.Contains("NgayTao") ? row["NgayTao"]?.ToString() : null, out DateTime date)) dtpDate.Value = date;
            }
        }

        private void ClearInputs() { txtUsername.Clear(); if (cboRole.Items.Count > 0) cboRole.SelectedIndex = 0; dtpDate.Value = DateTime.Now; txtUsername.Focus(); }
    }
}