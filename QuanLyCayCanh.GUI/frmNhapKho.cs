using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;

namespace QuanLyCayCanh.GUI
{
    public class frmNhapKho : frmBase
    {
        private TextBox txtSanPham, txtSoLuong, txtDonGia, txtNhaCungCap;
        private PhieuNhapBUS phieuBUS = new PhieuNhapBUS();

        public frmNhapKho()
        {
            this.lblHeaderTitle.Text = "📥 PHIẾU NHẬP KHO";
            this.Size = new Size(1000, 600);
            SetupUI();
        }

        private void SetupUI()
        {
            Panel pnlMain = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(240, 243, 247), Padding = new Padding(20) };
            Panel pnlForm = new Panel { Location = new Point(20, 60), Size = new Size(400, 400), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(15) };

            int y = 10;
            txtSanPham = AddFormField(pnlForm, "Sản phẩm:", y); y += 60;
            txtSoLuong = AddFormField(pnlForm, "Số lượng:", y); y += 60;
            txtDonGia = AddFormField(pnlForm, "Đơn giá:", y); y += 60;
            txtNhaCungCap = AddFormField(pnlForm, "Nhà cung cấp:", y); y += 60;

            Button btnAdd = new Button { Text = "➕ Thêm", Location = new Point(15, y), Size = new Size(370, 40), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnAdd.Click += BtnAdd_Click;
            pnlForm.Controls.Add(btnAdd);
            pnlMain.Controls.Add(pnlForm);
            this.Controls.Add(pnlMain);
        }

        private TextBox AddFormField(Panel parent, string label, int y)
        {
            parent.Controls.Add(new Label { Text = label, Location = new Point(15, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            TextBox txt = new TextBox { Location = new Point(15, y + 25), Width = 370, Height = 30, Font = new Font("Segoe UI", 11) };
            parent.Controls.Add(txt);
            return txt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSanPham.Text) || string.IsNullOrWhiteSpace(txtSoLuong.Text)) { MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!int.TryParse(txtSoLuong.Text, out int sl) || sl <= 0) { MessageBox.Show("Số lượng không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!decimal.TryParse(txtDonGia.Text, out decimal dg)) dg = 0;

            try
            {
                bool ok = phieuBUS.ThemPhieuNhap(txtSanPham.Text.Trim(), sl, dg, txtNhaCungCap.Text.Trim());
                if (ok) { MessageBox.Show("Thêm phiếu nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information); txtSanPham.Clear(); txtSoLuong.Clear(); txtDonGia.Clear(); txtNhaCungCap.Clear(); }
                else MessageBox.Show("Không thể thêm phiếu nhập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
    }
}