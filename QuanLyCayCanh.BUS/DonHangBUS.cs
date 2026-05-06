using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCayCanh.DAL;

namespace QuanLyCayCanh.BUS
{
    public class DonHangBUS
    {
        private readonly DataService ds = new DataService();

        public DataTable LayTatCaDonHang()
        {
            try
            {
                string sql = @"SELECT dh.Id, kh.TenKhach, nd.TenDangNhap, dh.TongTien, dh.NgayTao 
                              FROM DonHang dh 
                              JOIN KhachHang kh ON dh.KhachHangId = kh.Id 
                              JOIN NguoiDung nd ON dh.NguoiDungId = nd.Id 
                              ORDER BY dh.NgayTao DESC";
                return ds.GetTable(sql);
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy đơn hàng: " + ex.Message); }
        }

        public DataTable LayDonHangTheoId(int id)
        {
            try
            {
                string sql = @"SELECT dh.Id, kh.TenKhach, nd.TenDangNhap, dh.TongTien, dh.NgayTao 
                              FROM DonHang dh 
                              JOIN KhachHang kh ON dh.KhachHangId = kh.Id 
                              JOIN NguoiDung nd ON dh.NguoiDungId = nd.Id 
                              WHERE dh.Id = @Id";
                return ds.GetTableWithParams(sql, new Dictionary<string, object> { { "@Id", id } });
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy đơn theo Id: " + ex.Message); }
        }

        // Tạo đơn hàng trả về Id mới
        public int TaoDonHang(int khId, int ndId)
        {
            try
            {
                if (khId <= 0) throw new ArgumentException("Khách hàng không hợp lệ");
                if (ndId <= 0) throw new ArgumentException("Người dùng không hợp lệ");

                string sql = "INSERT INTO DonHang (KhachHangId, NguoiDungId, TongTien) VALUES (@KhId, @NdId, 0); SELECT SCOPE_IDENTITY();";
                object res = ds.ExecuteScalar(sql, new Dictionary<string, object> { { "@KhId", khId }, { "@NdId", ndId } });
                return res != null ? Convert.ToInt32(Convert.ToDecimal(res)) : 0;
            }
            catch (Exception ex) { throw new Exception("Lỗi tạo đơn hàng: " + ex.Message); }
        }

        // Cập nhật tổng tiền bằng hàm fn_TinhTongTien trên SQL Server (nếu đã có)
        public bool UpdateTongTien(int donHangId)
        {
            try
            {
                if (donHangId <= 0) throw new ArgumentException("ID đơn hàng không hợp lệ");
                string sql = "UPDATE DonHang SET TongTien = dbo.fn_TinhTongTien(@Id) WHERE Id = @Id";
                return ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@Id", donHangId } });
            }
            catch (Exception ex) { throw new Exception("Lỗi cập nhật tổng tiền: " + ex.Message); }
        }

        // Nếu muốn cập nhật thủ công (fallback nếu hàm DB không tồn tại)
        public bool UpdateTongTienManually(int donHangId, decimal tong)
        {
            try
            {
                if (donHangId <= 0) throw new ArgumentException("ID đơn hàng không hợp lệ");
                string sql = "UPDATE DonHang SET TongTien = @Tong WHERE Id = @Id";
                return ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@Tong", tong }, { "@Id", donHangId } });
            }
            catch (Exception ex) { throw new Exception("Lỗi cập nhật tổng tiền thủ công: " + ex.Message); }
        }

        public bool ThemLichSuDonHang(int donHangId, int trangThaiId)
        {
            try
            {
                if (donHangId <= 0) throw new ArgumentException("ID đơn hàng không hợp lệ");
                string sql = "INSERT INTO LichSuDonHang (DonHangId, TrangThaiId) VALUES (@DonHangId, @TrangId)";
                return ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@DonHangId", donHangId }, { "@TrangId", trangThaiId } });
            }
            catch (Exception ex) { throw new Exception("Lỗi thêm lịch sử: " + ex.Message); }
        }

        public int DemDonHang()
        {
            try
            {
                object result = ds.ExecuteScalar("SELECT COUNT(*) FROM DonHang");
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex) { throw new Exception("Lỗi đếm đơn hàng: " + ex.Message); }
        }

        public decimal GetTotalRevenue()
        {
            try
            {
                object result = ds.ExecuteScalar("SELECT SUM(TongTien) FROM DonHang");
                return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
            }
            catch (Exception ex) { throw new Exception("Lỗi tính doanh thu: " + ex.Message); }
        }
    }
}