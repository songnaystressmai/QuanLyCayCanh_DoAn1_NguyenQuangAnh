using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCayCanh.DAL;

namespace QuanLyCayCanh.BUS
{
    public class KhuyenMaiBUS
    {
        DataService ds = new DataService();

        public DataTable LayTatCaKhuyenMai()
        {
            try
            {
                return ds.GetTable("SELECT Id, TenKhuyenMai, PhanTramGiam, NgayBatDau, NgayKetThuc FROM KhuyenMai ORDER BY NgayBatDau DESC");
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy khuyến mãi: " + ex.Message); }
        }

        public bool ThemKhuyenMai(string tenKhuyenMai, int phanTramGiam, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                if (string.IsNullOrEmpty(tenKhuyenMai)) throw new Exception("Tên khuyến mãi không được rỗng");
                string sql = "INSERT INTO KhuyenMai (TenKhuyenMai, PhanTramGiam, NgayBatDau, NgayKetThuc) VALUES (@Ten, @Tram, @BatDau, @KetThuc)";
                var pars = new Dictionary<string, object> { { "@Ten", tenKhuyenMai.Trim() }, { "@Tram", phanTramGiam }, { "@BatDau", ngayBatDau }, { "@KetThuc", ngayKetThuc } };
                return ds.ExecuteQueryWithParams(sql, pars);
            }
            catch (Exception ex) { throw new Exception("Lỗi thêm khuyến mãi: " + ex.Message); }
        }
    }
}