using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoesStore.Models;

public partial class Voucher
{
    public string Mavoucher { get; set; } = null!;

    public int Soluong { get; set; }

    public decimal Giatoithieu { get; set; }

    public decimal Giamtoida { get; set; }

    public DateTime Ngaytao { get; set; }

    public DateTime Ngayhethan { get; set; }

    public virtual ICollection<Phieumua> Phieumuas { get; set; } = new List<Phieumua>();

    // Validation method
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Ngaytao > Ngayhethan)
        {
            yield return new ValidationResult(
                "Ngày hết hạn phải lớn hơn hoặc bằng ngày tạo",
                new[] { nameof(Ngayhethan) });
        }

        // THÊM VALIDATION CHO GIÁ
        if (Giatoithieu > Giamtoida)
        {
            yield return new ValidationResult(
                "Giá tối thiểu không được lớn hơn giá tối đa",
                new[] { nameof(Giamtoida) });
        }
    }
}