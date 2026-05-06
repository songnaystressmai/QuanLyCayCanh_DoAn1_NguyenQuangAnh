using System;
using System.Data;
using QuanLyCayCanh.DAL;

namespace QuanLyCayCanh.BUS
{
    public class LichSuDonHangBUS
    {
        private readonly DataService ds = new DataService();

        // Lấy lịch sử (liệt kê mỗi đơn kèm trạng thái mới nhất)
        public DataTable LayTatCaLichSuDonHang()
        {
            try
            {
                string sql = @"SELECT l.Id, dh.Id AS DonHangId, kh.TenKhach, nd.TenDangNhap, t.TenTrangThai, l.ThoiGian
                               FROM LichSuDonHang l
                               JOIN DonHang dh ON l.DonHangId = dh.Id
                               JOIN TrangThaiDonHang t ON l.TrangThaiId = t.Id
                               JOIN KhachHang kh ON dh.KhachHangId = kh.Id
                               JOIN NguoiDung nd ON dh.NguoiDungId = nd.Id
                               ORDER BY l.ThoiGian DESC";
                return ds.GetTable(sql);
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy lịch sử: " + ex.Message); }
        }

        // Lấy lịch sử theo DonHangId
        public DataTable LayLichSuTheoDonHang(int donHangId)
        {
            try
            {
                string sql = @"SELECT l.Id, l.DonHangId, t.TenTrangThai, l.ThoiGian
                               FROM LichSuDonHang l
                               JOIN TrangThaiDonHang t ON l.TrangThaiId = t.Id
                               WHERE l.DonHangId = @DonHangId
                               ORDER BY l.ThoiGian DESC";
                return ds.GetTableWithParams(sql, new System.Collections.Generic.Dictionary<string, object> { { "@DonHangId", donHangId } });
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy lịch sử theo đơn: " + ex.Message); }
        }
    }
}