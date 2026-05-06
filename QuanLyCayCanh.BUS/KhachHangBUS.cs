using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCayCanh.DAL;

namespace QuanLyCayCanh.BUS
{
    public class KhachHangBUS
    {
        DataService ds = new DataService();

        public DataTable LayTatCaKhachHang()
        {
            try
            {
                return ds.GetTable("SELECT Id, TenKhach, SoDienThoai FROM KhachHang ORDER BY Id DESC");
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy khách hàng: " + ex.Message); }
        }
        public int GetKhachHangCount()
        {
            try
            {
                object result = ds.ExecuteScalar("SELECT COUNT(*) FROM KhachHang");
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi đếm khách hàng: " + ex.Message);
            }
        }
        public bool ThemKhachHang(string tenKhach, string soDienThoai)
        {
            try
            {
                if (string.IsNullOrEmpty(tenKhach)) throw new Exception("Tên khách hàng không được rỗng");
                string sql = "INSERT INTO KhachHang (TenKhach, SoDienThoai) VALUES (@TenKhach, @SoDienThoai)";
                return ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@TenKhach", tenKhach.Trim() }, { "@SoDienThoai", soDienThoai.Trim() } });
            }
            catch (Exception ex) { throw new Exception("Lỗi thêm khách hàng: " + ex.Message); }
        }
    }
}