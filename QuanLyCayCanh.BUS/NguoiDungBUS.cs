using QuanLyCayCanh.DAL;
using System;
using System.Collections.Generic;
using System.Data;

namespace QuanLyCayCanh.BUS
{
    public class NguoiDungBUS
    {
        DataService da = new DataService();

        public DataTable DangNhap(string user, string pass)
        {
            try
            {
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                    throw new Exception("Tài khoản và mật khẩu không được rỗng");

                string sql = "SELECT Id, TenDangNhap, VaiTroId FROM NguoiDung WHERE TenDangNhap = @User AND MatKhau = @Pass";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@User", user.Trim() },
                    { "@Pass", pass }
                };
                return da.GetTableWithParams(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi đăng nhập: " + ex.Message);
            }
        }

        public DataTable LayTatCaNguoiDung()
        {
            try
            {
                string sql = @"SELECT nd.Id, nd.TenDangNhap, nd.VaiTroId, vt.TenVaiTro 
                              FROM NguoiDung nd 
                              LEFT JOIN VaiTro vt ON nd.VaiTroId = vt.Id 
                              ORDER BY nd.Id DESC";
                return da.GetTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy người dùng: " + ex.Message);
            }
        }

        public bool ThemNguoiDung(string tenDangNhap, string matKhau, int vaiTroId)
        {
            try
            {
                if (string.IsNullOrEmpty(tenDangNhap)) throw new Exception("Tên đăng nhập không được rỗng");
                if (string.IsNullOrEmpty(matKhau)) throw new Exception("Mật khẩu không được rỗng");

                string sql = "INSERT INTO NguoiDung (TenDangNhap, MatKhau, VaiTroId) VALUES (@TenDangNhap, @MatKhau, @VaiTroId)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@TenDangNhap", tenDangNhap.Trim() },
                    { "@MatKhau", matKhau },
                    { "@VaiTroId", vaiTroId }
                };
                return da.ExecuteQueryWithParams(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm người dùng: " + ex.Message);
            }
        }

        public bool CapNhatNguoiDung(int id, string tenDangNhap, int vaiTroId)
        {
            try
            {
                if (id <= 0) throw new Exception("ID không hợp lệ");
                if (string.IsNullOrEmpty(tenDangNhap)) throw new Exception("Tên đăng nhập không được rỗng");

                string sql = "UPDATE NguoiDung SET TenDangNhap = @Ten, VaiTroId = @VaiTroId WHERE Id = @Id";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@Ten", tenDangNhap.Trim() },
                    { "@VaiTroId", vaiTroId }
                };
                return da.ExecuteQueryWithParams(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật người dùng: " + ex.Message);
            }
        }

        public bool XoaNguoiDung(int id)
        {
            try
            {
                if (id <= 0) throw new Exception("ID không hợp lệ");

                string sql = "DELETE FROM NguoiDung WHERE Id = @Id";
                Dictionary<string, object> parameters = new Dictionary<string, object> { { "@Id", id } };
                return da.ExecuteQueryWithParams(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa người dùng: " + ex.Message);
            }
        }

        public DataTable LayVaiTro()
        {
            try
            {
                return da.GetTable("SELECT Id, TenVaiTro FROM VaiTro");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy vai trò: " + ex.Message);
            }
        }
    }
}