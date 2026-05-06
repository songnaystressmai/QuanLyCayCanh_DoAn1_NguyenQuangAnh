using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCayCanh.DAL;

namespace QuanLyCayCanh.BUS
{
    public class ChiTietDonHangBUS
    {
        private readonly DataService ds = new DataService();
        private readonly DataService _ds = new DataService();

        // Thêm chi tiết đơn hàng
        public bool ThemChiTietDonHang(int donHangId, int cayCanhId, int soLuong, decimal gia)
        {
            try
            {
                if (donHangId <= 0) throw new ArgumentException("ID đơn hàng không hợp lệ");
                if (cayCanhId <= 0) throw new ArgumentException("ID cây không hợp lệ");
                if (soLuong <= 0) throw new ArgumentException("Số lượng phải > 0");
                if (gia < 0) throw new ArgumentException("Giá không được âm");

                string sql = "INSERT INTO ChiTietDonHang (DonHangId, CayCanhId, SoLuong, Gia) VALUES (@DonHangId, @CayCanhId, @SoLuong, @Gia)";
                return ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> {
            { "@DonHangId", donHangId },
            { "@CayCanhId", cayCanhId },
            { "@SoLuong", soLuong },
            { "@Gia", gia }
        });
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm chi tiết đơn: " + ex.Message);
            }
        }

        // Lấy chi tiết theo DonHangId
        public DataTable LayChiTietDonHang(int donHangId)
        {
            try
            {
                string sql = @"SELECT ct.Id, ct.DonHangId, ct.CayCanhId, cc.TenCay, ct.SoLuong, ct.Gia, (ct.SoLuong * ct.Gia) AS ThanhTien
                               FROM ChiTietDonHang ct
                               JOIN CayCanh cc ON ct.CayCanhId = cc.Id
                               WHERE ct.DonHangId = @DonHangId";
                return _ds.GetTableWithParams(sql, new Dictionary<string, object> { { "@DonHangId", donHangId } });
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy chi tiết đơn hàng: " + ex.Message);
            }
        }
        public DataTable GetRecentSales()
        {
            try
            {
                string sql = @"SELECT dh.NgayTao AS Ngay, cc.TenCay AS SanPham, ct.SoLuong, ct.Gia, (ct.SoLuong*ct.Gia) AS ThanhTien
                       FROM ChiTietDonHang ct
                       JOIN DonHang dh ON ct.DonHangId = dh.Id
                       JOIN CayCanh cc ON ct.CayCanhId = cc.Id
                       ORDER BY dh.NgayTao DESC";
                return ds.GetTable(sql);
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy doanh số gần đây: " + ex.Message); }
        }
        // Xóa chi tiết đơn hàng
        public bool XoaChiTietDonHang(int id)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("ID không hợp lệ");
                string sql = "DELETE FROM ChiTietDonHang WHERE Id = @Id";
                return _ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@Id", id } });
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa chi tiết đơn hàng: " + ex.Message);
            }
        }
    }
}