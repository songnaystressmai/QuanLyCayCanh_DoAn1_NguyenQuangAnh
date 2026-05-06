using System;

namespace QuanLyCayCanh.DTO
{
    public class VaiTro
    {
        public int Id { get; set; }
        public string TenVaiTro { get; set; }
    }

    public class NguoiDung
    {
        public int Id { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public int VaiTroId { get; set; }
    }

    public class CayCanh
    {
        public int Id { get; set; }
        public string TenCay { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public int DanhMucId { get; set; }
    }

    public class KhachHang
    {
        public int Id { get; set; }
        public string TenKhach { get; set; }
        public string SoDienThoai { get; set; }
    }

    public class DonHang
    {
        public int Id { get; set; }
        public int KhachHangId { get; set; }
        public int NguoiDungId { get; set; }
        public decimal TongTien { get; set; }
        public DateTime NgayTao { get; set; }
    }

}