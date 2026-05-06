using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCayCanh.DAL;

namespace QuanLyCayCanh.BUS
{
    public class NhanVienBUS
    {
        private readonly DataService _ds = new DataService();

        public DataTable LayTatCaNhanVien()
        {
            try
            {
                return _ds.GetTable("SELECT Id, TenNhanVien, SoDienThoai, DiaChi, NguoiDungId FROM NhanVien ORDER BY Id DESC");
            }
            catch (Exception ex) { throw new Exception("Lỗi lấy nhân viên: " + ex.Message); }
        }

        public bool ThemNhanVien(string tenNhanVien, string soDienThoai, string diaChi, int nguoiDungId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenNhanVien)) throw new ArgumentException("Tên nhân viên không được rỗng");

                if (nguoiDungId > 0)
                {
                    object existing = _ds.ExecuteScalar("SELECT COUNT(*) FROM NhanVien WHERE NguoiDungId = @NguoiDungId", new Dictionary<string, object> { { "@NguoiDungId", nguoiDungId } });
                    int count = existing != null ? Convert.ToInt32(existing) : 0;
                    if (count > 0) throw new Exception("Tài khoản đã được liên kết với nhân viên khác.");
                }

                string sql = "INSERT INTO NhanVien (TenNhanVien, SoDienThoai, DiaChi, NguoiDungId) VALUES (@Ten, @SDT, @DiaChi, @NguoiDungId)";
                var pars = new Dictionary<string, object>
                {
                    { "@Ten", tenNhanVien.Trim() },
                    { "@SDT", soDienThoai?.Trim() ?? "" },
                    { "@DiaChi", diaChi?.Trim() ?? "" },
                    { "@NguoiDungId", nguoiDungId > 0 ? (object)nguoiDungId : DBNull.Value }
                };
                return _ds.ExecuteQueryWithParams(sql, pars);
            }
            catch (Exception ex) { throw new Exception("Lỗi thêm nhân viên: " + ex.Message); }
        }

        public bool CapNhatNhanVien(int id, string tenNhanVien, string soDienThoai, string diaChi, int nguoiDungId)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("ID không hợp lệ");
                if (string.IsNullOrWhiteSpace(tenNhanVien)) throw new ArgumentException("Tên nhân viên không được rỗng");

                if (nguoiDungId > 0)
                {
                    object existing = _ds.ExecuteScalar("SELECT COUNT(*) FROM NhanVien WHERE NguoiDungId = @NguoiDungId AND Id <> @Id", new Dictionary<string, object> { { "@NguoiDungId", nguoiDungId }, { "@Id", id } });
                    int count = existing != null ? Convert.ToInt32(existing) : 0;
                    if (count > 0) throw new Exception("Tài khoản đã được liên kết với nhân viên khác.");
                }

                string sql = "UPDATE NhanVien SET TenNhanVien = @Ten, SoDienThoai = @SDT, DiaChi = @DiaChi, NguoiDungId = @NguoiDungId WHERE Id = @Id";
                var pars = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@Ten", tenNhanVien.Trim() },
                    { "@SDT", soDienThoai?.Trim() ?? "" },
                    { "@DiaChi", diaChi?.Trim() ?? "" },
                    { "@NguoiDungId", nguoiDungId > 0 ? (object)nguoiDungId : DBNull.Value }
                };
                return _ds.ExecuteQueryWithParams(sql, pars);
            }
            catch (Exception ex) { throw new Exception("Lỗi cập nhật nhân viên: " + ex.Message); }
        }

        public bool XoaNhanVien(int id)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("ID không hợp lệ");
                string sql = "DELETE FROM NhanVien WHERE Id = @Id";
                return _ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@Id", id } });
            }
            catch (Exception ex) { throw new Exception("Lỗi xóa nhân viên: " + ex.Message); }
        }
    }
}