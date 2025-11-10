using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoesStore.Models;

public partial class Khuyenmai
{
    public int Makm { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
    [Display(Name = "Ngày Bắt Đầu")]
    public DateTime Ngaybd { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
    [Display(Name = "Ngày Kết Thúc")]
    public DateTime Ngaykt { get; set; }

    [Required(ErrorMessage = "Phần trăm giảm là bắt buộc")]
    [Range(1, 100, ErrorMessage = "Phần trăm giảm phải từ 1% đến 100%")]
    [Display(Name = "Phần Trăm Giảm")]
    public int Phantramgiam { get; set; }

    public virtual ICollection<Dongsanpham> Madongsanphams { get; set; } = new List<Dongsanpham>();

    // Validation method cho logic nghiệp vụ
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Ngaykt <= Ngaybd)
        {
            yield return new ValidationResult(
                "Ngày kết thúc phải lớn hơn ngày bắt đầu",
                new[] { nameof(Ngaykt) });
        }
    }
}