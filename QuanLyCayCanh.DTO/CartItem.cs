using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCayCanh.DTO
{
    public class CartItem
    {
        public int CayId { get; set; }
        public string Ten { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
    }
}