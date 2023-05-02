using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuntiChatClient
{
    internal class User
    {
        public string HoTen { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }

        public User(string hoTen, string taiKhoan, string matKhau)
        {
            HoTen = hoTen;
            TaiKhoan = taiKhoan;
            MatKhau = matKhau;
        }
    }
    }
