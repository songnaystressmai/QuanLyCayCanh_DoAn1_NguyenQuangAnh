using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyCayCanh.BUS;
using QuanLyCayCanh.DTO;

namespace QuanLyCayCanh.GUI
{
    public class frmBanHang : frmBase
    {
        private FlowLayoutPanel flpProducts;
        private ListBox lstCart;
        private Label lblTotal;
        private List<CartItem> cartItems = new List<CartItem>();
        private decimal currentTotal = 0m;

        // BUS
        private readonly CayCanhBUS cayBus = new CayCanhBUS();
        private readonly DonHangBUS donBus = new DonHangBUS();
        private readonly ChiTietDonHangBUS ctBus = new ChiTietDonHangBUS();

        public frmBanHang()
        {
            if (this.pnlHeader != null)
            {
                this.pnlHeader.Visible = true;
                this.lblHeaderTitle.Text = "🛒 HỆ THỐNG BÁN HÀNG";
            }

            this.Padding = new Padding(0, 10, 0, 0);
            this.BackColor = Color.FromArgb(242, 245, 248);

            SetupUI();

            // Load products from DB
            LoadProductsFromDb();
        }

        private void SetupUI()
        {
            TableLayoutPanel tlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(0, 15, 0, 0),
                BackColor = Color.Transparent
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70f));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400f));
            this.Controls.Add(tlp);
            tlp.BringToFront();

            // Products panel
            flpProducts = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };
            tlp.Controls.Add(flpProducts, 0, 0);

            // Right panel - cart
            Panel pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                Margin = new Padding(10, 0, 0, 0)
            };

            Label lblTitle = new Label
            {
                Text = "CHI TIẾT ĐƠN HÀNG",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lstCart = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11),
                SelectionMode = SelectionMode.None
            };

            Panel pnlSummary = new Panel { Dock = DockStyle.Bottom, Height = 180, BackColor = Color.FromArgb(250, 250, 250) };
            lblTotal = new Label
            {
                Text = "TỔNG: 0 VNĐ",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 10, 0)
            };

            Button btnPay = new Button
            {
                Text = "XÁC NHẬN THANH TOÁN",
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(45, 90, 39),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPay.FlatAppearance.BorderSize = 0;
            btnPay.Click += BtnPay_Click;

            Button btnCancel = new Button
            {
                Text = "HỦY ĐƠN HÀNG",
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => ResetCart();

            pnlSummary.Controls.Add(btnCancel);
            pnlSummary.Controls.Add(btnPay);
            pnlSummary.Controls.Add(lblTotal);

            pnlRight.Controls.Add(lstCart);
            pnlRight.Controls.Add(lblTitle);
            pnlRight.Controls.Add(pnlSummary);

            tlp.Controls.Add(pnlRight, 1, 0);
        }

        private void LoadProductsFromDb()
        {
            try
            {
                flpProducts.Controls.Clear();
                var dt = cayBus.LayTatCaCay();
                foreach (System.Data.DataRow r in dt.Rows)
                {
                    int id = Convert.ToInt32(r["Id"]);
                    string ten = r["TenCay"]?.ToString() ?? "Không tên";
                    decimal gia = r["Gia"] != DBNull.Value ? Convert.ToDecimal(r["Gia"]) : 0m;

                    // try load first image if any
                    string imgFullPath = null;
                    try
                    {
                        var dtImgs = cayBus.LayHinhAnhTheoCay(id);
                        if (dtImgs != null && dtImgs.Rows.Count > 0)
                        {
                            string rel = dtImgs.Rows[0]["DuongDan"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(rel))
                            {
                                string full = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rel);
                                if (File.Exists(full)) imgFullPath = full;
                            }
                        }
                    }
                    catch { /* ignore image errors */ }

                    flpProducts.Controls.Add(CreateProductCard(ten, gia, id, imgFullPath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải sản phẩm: " + ex.Message);
            }
        }

        private Panel CreateProductCard(string name, decimal price, int id, string imgFullPath)
        {
            Panel card = new Panel { Size = new Size(200, 290), BackColor = Color.White, Margin = new Padding(12) };
            card.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, Color.FromArgb(230, 230, 230), ButtonBorderStyle.Solid); };

            PictureBox pic = new PictureBox { Size = new Size(220, 160), BackColor = Color.FromArgb(232, 245, 233), Dock = DockStyle.Top, SizeMode = PictureBoxSizeMode.Zoom };
            if (!string.IsNullOrEmpty(imgFullPath) && File.Exists(imgFullPath))
            {
                try
                {
                    using (var fs = File.OpenRead(imgFullPath))
                    {
                        pic.Image = Image.FromStream(fs);
                    }
                }
                catch { /* ignore image load error */ }
            }

            Label lblName = new Label { Text = name, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.BottomCenter, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            Label lblPrice = new Label { Text = $"{price:N0}đ", Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter, ForeColor = Color.FromArgb(211, 84, 0), Font = new Font("Segoe UI", 12, FontStyle.Bold) };

            Button btnAdd = new Button { Text = "THÊM VÀO GIỎ", Dock = DockStyle.Bottom, Height = 45, BackColor = Color.FromArgb(240, 240, 240), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                var existing = cartItems.Find(x => x.CayId == id);
                if (existing != null)
                {
                    existing.SoLuong += 1;
                }
                else
                {
                    cartItems.Add(new CartItem { CayId = id, Ten = name, Gia = price, SoLuong = 1 });
                }
                RefreshCartList();
            };

            card.Controls.Add(lblPrice);
            card.Controls.Add(lblName);
            card.Controls.Add(btnAdd);
            card.Controls.Add(pic);
            return card;
        }

        private void RefreshCartList()
        {
            lstCart.Items.Clear();
            currentTotal = 0m;
            foreach (var it in cartItems)
            {
                lstCart.Items.Add($"{it.Ten} - {it.Gia:N0}đ x{it.SoLuong}");
                currentTotal += it.Gia * it.SoLuong;
            }
            lblTotal.Text = $"TỔNG: {currentTotal:N0} VNĐ";
        }

        private void ResetCart()
        {
            if (MessageBox.Show("Xác nhận hủy giỏ hàng?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                cartItems.Clear();
                RefreshCartList();
            }
        }

        private void BtnPay_Click(object sender, EventArgs e)
{
    if (cartItems.Count == 0)
    {
        MessageBox.Show("Giỏ hàng đang trống!", "Thông báo");
        return;
    }
    if (MessageBox.Show($"Xác nhận thanh toán {currentTotal:N0} VNĐ?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes)
        return;

    try
    {
        // TODO: Lấy id khách hàng và id người dùng (từ session/login)
        int khachHangId = 1; // tạm thời
        int nguoiDungId = 1; // tạm thời

        // 1) Tạo DonHang (trả về id)
        int donHangId = donBus.TaoDonHang(khachHangId, nguoiDungId);
        if (donHangId <= 0) throw new Exception("Không tạo được đơn hàng.");

        // 2) Chèn từng ChiTietDonHang + trừ tồn kho
        foreach (var it in cartItems)
        {
            bool ok = ctBus.ThemChiTietDonHang(donHangId, it.CayId, it.SoLuong, it.Gia);
            if (!ok) throw new Exception($"Thêm chi tiết cho {it.Ten} thất bại.");

            // Trừ tồn kho (nếu DB không có trigger)
            bool okTru = cayBus.TruTonKho(it.CayId, it.SoLuong);
            if (!okTru) { /* log hoặc ignore */ }
        }

        // 3) Cập nhật tổng tiền DonHang
        bool okUpdate = donBus.UpdateTongTien(donHangId);
        if (!okUpdate)
        {
            // Nếu update bằng hàm DB thất bại, ta tính và cập nhật thủ công
            decimal tong = 0m;
            foreach (var it in cartItems) tong += it.Gia * it.SoLuong;
            donBus.UpdateTongTienManually(donHangId, tong); // (nếu bạn thêm method này)
        }

        // 4) Thêm lịch sử đơn hàng (trạng thái: 1 = "Chờ xử lý")
        donBus.ThemLichSuDonHang(donHangId, 1);

        MessageBox.Show("Thanh toán thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // 5) Làm mới UI: xóa giỏ, reload danh sách đơn hàng, báo cáo
        cartItems.Clear();
        RefreshCartList();

        // reload các views
        LoadProductsFromDb();
        // nếu có form DonHang đang hiển thị (frmDonHang), gọi LoadData() để refresh:
        // (nếu bạn dùng event/mediator hoặc public method, gọi tương ứng)
        // ví dụ: DonHangForm?.LoadData();
    }
    catch (Exception ex)
    {
        MessageBox.Show("Lỗi khi thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
    }
}