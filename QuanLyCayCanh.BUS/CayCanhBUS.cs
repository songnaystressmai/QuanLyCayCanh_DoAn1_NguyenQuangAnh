using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCayCanh.DAL;
using QuanLyCayCanh.DTO;

namespace QuanLyCayCanh.BUS
{
    public class CayCanhBUS
    {
        private readonly DataService ds = new DataService();
        private readonly DataService _ds = new DataService();

        public DataTable LayTatCaCay()
        {
            try
            {
                return ds.GetTable("SELECT Id, TenCay, Gia, SoLuong, DanhMucId FROM CayCanh ORDER BY Id DESC");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách cây: " + ex.Message);
            }
        }

        public DataTable LayTheoTen(string tenCay)
        {
            try
            {
                string sql = "SELECT Id, TenCay, Gia, SoLuong, DanhMucId FROM CayCanh WHERE TenCay LIKE @TenCay ORDER BY Id DESC";
                return _ds.GetTableWithParams(sql, new Dictionary<string, object> { { "@TenCay", "%" + tenCay + "%" } });
            }
            catch (Exception ex) { throw new Exception("Lỗi tìm kiếm cây: " + ex.Message); }
        }

        public bool ThemCayCanh(CayCanh cay)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cay.TenCay)) throw new ArgumentException("Tên cây không được rỗng");
                string sql = "INSERT INTO CayCanh (TenCay, Gia, SoLuong, DanhMucId) VALUES (@TenCay, @Gia, @SoLuong, @DanhMucId)";
                var pars = new Dictionary<string, object>
                {
                    { "@TenCay", cay.TenCay.Trim() },
                    { "@Gia", cay.Gia },
                    { "@SoLuong", cay.SoLuong },
                    { "@DanhMucId", cay.DanhMucId }
                };
                return _ds.ExecuteQueryWithParams(sql, pars);
            }
            catch (Exception ex) { throw new Exception("Lỗi thêm cây: " + ex.Message); }
        }

        // Thêm và trả về Id mới
        public int ThemCayCanh_ReturnId(CayCanh cay)
        {
            try
            {
                string sql = "INSERT INTO CayCanh (TenCay, Gia, SoLuong, DanhMucId) VALUES (@TenCay, @Gia, @SoLuong, @DanhMucId); SELECT SCOPE_IDENTITY();";
                var pars = new Dictionary<string, object>
                {
                    { "@TenCay", cay.TenCay.Trim() },
                    { "@Gia", cay.Gia },
                    { "@SoLuong", cay.SoLuong },
                    { "@DanhMucId", cay.DanhMucId }
                };
                object res = _ds.ExecuteScalar(sql, pars);
                return res != null ? Convert.ToInt32(Convert.ToDecimal(res)) : 0;
            }
            catch (Exception ex) { throw new Exception("Lỗi thêm cây trả về Id: " + ex.Message); }
        }

        public bool CapNhatCayCanh(CayCanh cay)
        {
            try
            {
                if (cay.Id <= 0) throw new ArgumentException("ID không hợp lệ");
                string sql = "UPDATE CayCanh SET TenCay = @TenCay, Gia = @Gia, SoLuong = @SoLuong, DanhMucId = @DanhMucId WHERE Id = @Id";
                var pars = new Dictionary<string, object>
                {
                    { "@Id", cay.Id },
                    { "@TenCay", cay.TenCay.Trim() },
                    { "@Gia", cay.Gia },
                    { "@SoLuong", cay.SoLuong },
                    { "@DanhMucId", cay.DanhMucId }
                };
                return _ds.ExecuteQueryWithParams(sql, pars);
            }
            catch (Exception ex) { throw new Exception("Lỗi cập nhật cây: " + ex.Message); }
        }

        public bool XoaCay(int id)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("ID không hợp lệ");
                string sql = "DELETE FROM CayCanh WHERE Id = @Id";
                return _ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@Id", id } });
            }
            catch (Exception ex) { throw new Exception("Lỗi xóa cây: " + ex.Message); }
        }
        public DataTable LayHinhAnhTheoCay(int cayId)
        {
            try
            {
                string sql = "SELECT Id, DuongDan FROM HinhAnhCay WHERE CayCanhId = @CayId";
                return ds.GetTableWithParams(sql, new Dictionary<string, object> { { "@CayId", cayId } });
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy hình ảnh: " + ex.Message);
            }
        }

        public DataTable GetTopSellingProducts(int top = 5)
        {
            try
            {
                string sql = @"SELECT TOP(@Top) cc.TenCay, SUM(ct.SoLuong) AS TongBan
                       FROM ChiTietDonHang ct
                       JOIN CayCanh cc ON ct.CayCanhId = cc.Id
                       GROUP BY cc.TenCay
                       ORDER BY SUM(ct.SoLuong) DESC";
                return ds.GetTableWithParams(sql, new Dictionary<string, object> { { "@Top", top } });
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy SP bán chạy: " + ex.Message); }
        }
        public bool TruTonKho(int cayId, int soLuongTru)
        {
            try
            {
                if (cayId <= 0) throw new ArgumentException("ID không hợp lệ");
                if (soLuongTru <= 0) throw new ArgumentException("Số lượng trừ phải > 0");

                string sql = "UPDATE CayCanh SET SoLuong = CASE WHEN SoLuong - @SL < 0 THEN 0 ELSE SoLuong - @SL END WHERE Id = @Id";
                return ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@Id", cayId }, { "@SL", soLuongTru } });
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trừ tồn kho: " + ex.Message);
            }
        }
        public bool ThemHinhAnhCay(int cayId, string duongDan)
        {
            try
            {
                if (cayId <= 0) throw new ArgumentException("ID cây không hợp lệ");
                if (string.IsNullOrWhiteSpace(duongDan)) throw new ArgumentException("Đường dẫn ảnh rỗng");
                string sql = "INSERT INTO HinhAnhCay (CayCanhId, DuongDan) VALUES (@Id, @Duong)";
                return ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@Id", cayId }, { "@Duong", duongDan } });
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm hình ảnh: " + ex.Message);
            }
        }
    }
}