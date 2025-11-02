using System;
using System.Collections.Generic;

namespace ShoesStore.Models;

public partial class Taikhoan
{
    public string Email { get; set; } = null!;

    public string Matkhau { get; set; } = null!;

    public int Loaitk { get; set; }

    public virtual Khachhang? Khachhang { get; set; }

    public virtual Nhanvien? Nhanvien { get; set; }
}
