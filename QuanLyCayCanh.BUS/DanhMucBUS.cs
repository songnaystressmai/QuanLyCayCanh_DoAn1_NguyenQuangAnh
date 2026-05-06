using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCayCanh.DAL;

namespace QuanLyCayCanh.BUS
{
    public class DanhMucBUS
    {
        DataService ds = new DataService();

        public DataTable LayTatCaDanhMuc()
        {
            try
            {
                return ds.GetTable("SELECT Id, TenDanhMuc FROM DanhMuc ORDER BY Id");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh mục: " + ex.Message);
            }
        }

        public bool ThemDanhMuc(string tenDanhMuc)
        {
            try
            {
                if (string.IsNullOrEmpty(tenDanhMuc)) throw new Exception("Tên danh mục không được rỗng");
                string sql = "INSERT INTO DanhMuc (TenDanhMuc) VALUES (@TenDanhMuc)";
                return ds.ExecuteQueryWithParams(sql, new Dictionary<string, object> { { "@TenDanhMuc", tenDanhMuc.Trim() } });
            }
            catch (Exception ex) { throw new Exception("Lỗi thêm danh mục: " + ex.Message); }
        }

        public bool XoaDanhMuc(int id)
        {
            try
            {
                if (id <= 0) throw new Exception("ID không hợp lệ");
                return ds.ExecuteQueryWithParams("DELETE FROM DanhMuc WHERE Id = @Id", new Dictionary<string, object> { { "@Id", id } });
            }
            catch (Exception ex) { throw new Exception("Lỗi xóa danh mục: " + ex.Message); }
        }
    }
}