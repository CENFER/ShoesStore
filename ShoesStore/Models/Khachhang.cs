using System;
using System.Collections.Generic;

namespace ShoesStore.Models;

public partial class Khachhang
{
    public int Makh { get; set; }

    public string Tenkh { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public bool? Gioitinh { get; set; }

    public DateTime? Ngaysinh { get; set; }

    public decimal Tongxu { get; set; }

    public virtual ICollection<Binhluan> Binhluans { get; set; } = new List<Binhluan>();

    public virtual Taikhoan EmailNavigation { get; set; } = null!;

    public virtual ICollection<Phieumua> Phieumuas { get; set; } = new List<Phieumua>();

    public virtual ICollection<Sodiachi> Sodiachis { get; set; } = new List<Sodiachi>();
}
