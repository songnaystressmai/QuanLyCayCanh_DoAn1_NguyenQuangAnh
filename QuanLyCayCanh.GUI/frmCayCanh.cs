using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;
using QuanLyCayCanh.DTO;

namespace QuanLyCayCanh.GUI
{
    public class frmCayCanh : frmBase
    {
        private DataGridView dgv;
        private TextBox txtId, txtTen, txtGia, txtSoLuong, txtTimKiem;
        private ComboBox cboLoai;
        private PictureBox pbCay;
        private Button btnThem, btnSua, btnXoa, btnChonAnh, btnReset, btnTimKiem;

        private CayCanhBUS bus = new CayCanhBUS();
        private DanhMucBUS busDanhMuc = new DanhMucBUS();

        // Biến lưu đường dẫn file ảnh tạm khi chọn ảnh
        private string duongDanAnh = "";

        public frmCayCanh()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1200, 700);

            if (this.pnlHeader != null)
            {
                this.pnlHeader.Visible = true;
                this.lblHeaderTitle.Text = "🌿 QUẢN LÝ CÂY CẢNH";
            }

            this.Padding = new Padding(0, 10, 0, 0);

            SetupModernUI();
            SetupEvents();
            LoadDanhMuc();
            LoadDataToGrid();
        }

        private void SetupModernUI()
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
            mainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350f));
            mainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // --- Left input panel ---
            Panel pnlInput = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15),
                AutoScroll = true
            };
            pnlInput.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.FromArgb(230, 230, 230), ButtonBorderStyle.Solid);
            };

            txtId = new TextBox { Visible = false };
            pbCay = new PictureBox
            {
                Size = new Size(180, 180),
                Location = new Point(85, 15),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            btnChonAnh = new Button
            {
                Text = "📷 Chọn ảnh",
                Location = new Point(115, 205),
                Size = new Size(120, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(230, 230, 230),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnChonAnh.FlatAppearance.BorderSize = 0;

            Font fLabel = new Font("Segoe UI", 9, FontStyle.Bold);
            Font fInput = new Font("Segoe UI", 11);

            int labelY = 250; int inputY = 275; int spacing = 65;
            AddLabelTextBox(pnlInput, "Tên cây:", labelY, inputY, ref txtTen, fLabel, fInput);
            AddLabelTextBox(pnlInput, "Giá bán (VNĐ):", labelY + spacing, inputY + spacing, ref txtGia, fLabel, fInput);
            AddLabelTextBox(pnlInput, "Số lượng tồn:", labelY + spacing * 2, inputY + spacing * 2, ref txtSoLuong, fLabel, fInput);

            pnlInput.Controls.Add(new Label { Text = "Danh mục:", Location = new Point(15, labelY + spacing * 3), Font = fLabel, AutoSize = true });
            cboLoai = new ComboBox { Location = new Point(15, inputY + spacing * 3), Width = 300, Font = fInput, DropDownStyle = ComboBoxStyle.DropDownList };

            int btnY = 530;
            btnThem = CreateStyledButton("✅ Thêm mới", Color.FromArgb(46, 204, 113), new Point(15, btnY), 145, 40);
            btnSua = CreateStyledButton("✏️ Cập nhật", Color.FromArgb(52, 152, 219), new Point(170, btnY), 145, 40);
            btnXoa = CreateStyledButton("❌ Xóa bỏ", Color.FromArgb(231, 76, 60), new Point(15, btnY + 50), 145, 40);
            btnReset = CreateStyledButton("🔄 Làm mới", Color.FromArgb(149, 165, 166), new Point(170, btnY + 50), 145, 40);

            pnlInput.Controls.AddRange(new Control[] { txtId, pbCay, btnChonAnh, cboLoai, btnThem, btnSua, btnXoa, btnReset });

            // --- Right grid/search panel ---
            Panel pnlGridContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 0, 0, 0) };

            Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Margin = new Padding(0, 0, 0, 10) };
            Label lblSearch = new Label { Text = "Tìm kiếm tên cây:", Location = new Point(15, 22), AutoSize = true, Font = fLabel };
            txtTimKiem = new TextBox { Location = new Point(130, 18), Width = 210, Font = fInput };
            btnTimKiem = CreateStyledButton("🔍 Tìm", Color.FromArgb(52, 152, 219), new Point(350, 17), 80, 32);
            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtTimKiem, btnTimKiem });

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
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 47, 61);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 249);

            pnlGridContainer.Controls.Add(dgv);
            pnlGridContainer.Controls.Add(pnlSearch);

            mainTlp.Controls.Add(pnlInput, 0, 0);
            mainTlp.Controls.Add(pnlGridContainer, 1, 0);

            pnlMainWrapper.Controls.Add(mainTlp);
            this.Controls.Add(pnlMainWrapper);

            pnlMainWrapper.SendToBack();
        }

        private void SetupEvents()
        {
            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnReset.Click += (s, e) => ResetForm();
            btnTimKiem.Click += (s, e) => TimKiem();
            btnChonAnh.Click += BtnChonAnh_Click;
            dgv.CellClick += Dgv_CellClick;
        }

        private void LoadDanhMuc()
        {
            try
            {
                cboLoai.DataSource = busDanhMuc.LayTatCaDanhMuc();
                cboLoai.DisplayMember = "TenDanhMuc";
                cboLoai.ValueMember = "Id";
            }
            catch { }
        }

        private void LoadDataToGrid()
        {
            try
            {
                dgv.DataSource = bus.LayTatCaCay();
                if (dgv.Columns["Id"] != null) dgv.Columns["Id"].HeaderText = "Mã số";
                if (dgv.Columns["TenCay"] != null) dgv.Columns["TenCay"].HeaderText = "Tên cây";
                if (dgv.Columns["Gia"] != null) dgv.Columns["Gia"].HeaderText = "Giá bán";
                if (dgv.Columns["SoLuong"] != null) dgv.Columns["SoLuong"].HeaderText = "Tồn kho";
            }
            catch { }
        }

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
            // frmCayCanh
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.ClientSize = new System.Drawing.Size(1125, 934);
            this.Name = "frmCayCanh";
            this.Load += new System.EventHandler(this.frmCayCanh_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        private void frmCayCanh_Load(object sender, EventArgs e)
        {

        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgv.Rows[e.RowIndex];
                txtId.Text = row.Cells["Id"].Value.ToString();
                txtTen.Text = row.Cells["TenCay"].Value.ToString();
                txtGia.Text = row.Cells["Gia"].Value.ToString();
                txtSoLuong.Text = row.Cells["SoLuong"].Value.ToString();
                if (int.TryParse(row.Cells["DanhMucId"]?.Value?.ToString(), out int dm)) cboLoai.SelectedValue = dm;

                // load first image if any
                try
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    var dtImgs = bus.LayHinhAnhTheoCay(id);
                    if (dtImgs != null && dtImgs.Rows.Count > 0)
                    {
                        string rel = dtImgs.Rows[0]["DuongDan"]?.ToString();
                        string full = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rel ?? "");
                        if (File.Exists(full))
                        {
                            using (var fs = File.OpenRead(full))
                                pbCay.Image = Image.FromStream(fs);
                        }
                        else pbCay.Image = null;
                    }
                    else pbCay.Image = null;
                }
                catch { pbCay.Image = null; }
            }
        }

        private void BtnChonAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.png;*.jpeg" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var fs = File.OpenRead(ofd.FileName))
                        {
                            pbCay.Image = Image.FromStream(fs);
                        }
                        duongDanAnh = ofd.FileName;
                    }
                    catch (Exception ex) { MessageBox.Show("Không thể load ảnh: " + ex.Message); }
                }
            }
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // validate
                string ten = txtTen.Text.Trim();
                if (string.IsNullOrEmpty(ten)) { MessageBox.Show("Vui lòng nhập tên cây."); return; }

                if (!decimal.TryParse(txtGia.Text.Trim(), out decimal gia)) { MessageBox.Show("Giá không hợp lệ."); return; }
                if (!int.TryParse(txtSoLuong.Text.Trim(), out int sl)) { MessageBox.Show("Số lượng không hợp lệ."); return; }
                int danhMucId = cboLoai.SelectedValue != null ? Convert.ToInt32(cboLoai.SelectedValue) : 0;

                // create DTO
                DTO.CayCanh cay = new DTO.CayCanh
                {
                    TenCay = ten,
                    Gia = gia,
                    SoLuong = sl,
                    DanhMucId = danhMucId
                };

                // call BUS to insert and get new Id
                int newId = bus.ThemCayCanh_ReturnId(cay); // method must return new Id (SCOPE_IDENTITY)
                if (newId > 0)
                {
                    // if user has chosen an image, copy into app's img folder and save relative path into HinhAnhCay table
                    if (!string.IsNullOrEmpty(duongDanAnh) && File.Exists(duongDanAnh))
                    {
                        string imgFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
                        Directory.CreateDirectory(imgFolder);
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(duongDanAnh);
                        string dest = Path.Combine(imgFolder, filename);
                        File.Copy(duongDanAnh, dest, true);
                        string relative = Path.Combine("img", filename).Replace('\\', '/'); // store with forward slashes
                        bus.ThemHinhAnhCay(newId, relative);
                    }

                    MessageBox.Show("Thêm cây thành công.");
                    LoadDataToGrid();
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Thêm cây thất bại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm cây: " + ex.Message);
            }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out int id) || id <= 0) { MessageBox.Show("Chọn cây cần sửa."); return; }

                string ten = txtTen.Text.Trim();
                if (string.IsNullOrEmpty(ten)) { MessageBox.Show("Vui lòng nhập tên cây."); return; }
                if (!decimal.TryParse(txtGia.Text.Trim(), out decimal gia)) { MessageBox.Show("Giá không hợp lệ."); return; }
                if (!int.TryParse(txtSoLuong.Text.Trim(), out int sl)) { MessageBox.Show("Số lượng không hợp lệ."); return; }
                int danhMucId = cboLoai.SelectedValue != null ? Convert.ToInt32(cboLoai.SelectedValue) : 0;

                DTO.CayCanh cay = new DTO.CayCanh
                {
                    Id = id,
                    TenCay = ten,
                    Gia = gia,
                    SoLuong = sl,
                    DanhMucId = danhMucId
                };

                if (bus.CapNhatCayCanh(cay))
                {
                    if (!string.IsNullOrEmpty(duongDanAnh) && File.Exists(duongDanAnh))
                    {
                        string imgFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
                        Directory.CreateDirectory(imgFolder);
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(duongDanAnh);
                        string dest = Path.Combine(imgFolder, filename);
                        File.Copy(duongDanAnh, dest, true);
                        string relative = Path.Combine("img", filename).Replace('\\', '/');
                        bus.ThemHinhAnhCay(id, relative);
                    }

                    MessageBox.Show("Cập nhật thành công.");
                    LoadDataToGrid();
                }
                else MessageBox.Show("Cập nhật thất bại.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out int id) || id <= 0) { MessageBox.Show("Chọn cây cần xóa."); return; }
                if (MessageBox.Show("Xác nhận xóa?", "Hỏi", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (bus.XoaCay(id)) { MessageBox.Show("Xóa thành công."); LoadDataToGrid(); ResetForm(); }
                    else MessageBox.Show("Xóa thất bại.");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
        }

        private void TimKiem()
        {
            try { dgv.DataSource = bus.LayTheoTen(txtTimKiem.Text); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void ResetForm()
        {
            txtId.Clear(); txtTen.Clear(); txtGia.Text = "0"; txtSoLuong.Text = "0";
            pbCay.Image = null; duongDanAnh = "";
        }

        private void AddLabelTextBox(Panel panel, string labelText, int labelY, int inputY, ref TextBox txtBox, Font fLabel, Font fInput)
        {
            panel.Controls.Add(new Label { Text = labelText, Location = new Point(15, labelY), Font = fLabel, AutoSize = true });
            txtBox = new TextBox { Location = new Point(15, inputY), Width = 300, Font = fInput };
            panel.Controls.Add(txtBox);
        }

        private Button CreateStyledButton(string text, Color backColor, Point loc, int w, int h)
        {
            Button btn = new Button
            {
                Text = text,
                Location = loc,
                Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}