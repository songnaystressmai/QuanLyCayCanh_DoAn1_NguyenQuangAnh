using QuanLyCayCanh.DAL;
using System;
using System.Collections.Generic;

namespace QuanLyCayCanh.BUS
{
    public class PhieuNhapBUS
    {
        DataService ds = new DataService();

        public bool ThemPhieuNhap(string tenSanPham, int soLuong, decimal donGia, string nhaCungCap)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenSanPham)) throw new Exception("Tên sản phẩm không được rỗng");
                if (soLuong <= 0) throw new Exception("Số lượng phải lớn hơn 0");
                if (donGia < 0) throw new Exception("Đơn giá không hợp lệ");

                string sql = "INSERT INTO PhieuNhap (TenSanPham, SoLuong, DonGia, NhaCungCap) VALUES (@Ten, @SL, @DG, @NCC)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Ten", tenSanPham.Trim() },
                    { "@SL", soLuong },
                    { "@DG", donGia },
                    { "@NCC", nhaCungCap?.Trim() ?? "" }
                };
                return ds.ExecuteQueryWithParams(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm phiếu nhập: " + ex.Message);
            }
        }
    }
}