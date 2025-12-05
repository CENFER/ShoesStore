using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoesStore.Models;

public partial class Khuyenmai
{
    public int Makm { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [Display(Name = "Start Date")]
    public DateTime Ngaybd { get; set; }

    [Required(ErrorMessage = "End date is required")]
    [Display(Name = "End Date")]
    public DateTime Ngaykt { get; set; }

    [Required(ErrorMessage = "Discount percentage is required")]
    [Range(1, 100, ErrorMessage = "Discount percentage must be between 1% and 100%")]
    [Display(Name = "Discount Percentage")]
    public int Phantramgiam { get; set; }

    public virtual ICollection<Dongsanpham> Madongsanphams { get; set; } = new List<Dongsanpham>();

    // Custom validation for business rules
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Ngaykt <= Ngaybd)
        {
            yield return new ValidationResult(
                "End date must be greater than start date",
                new[] { nameof(Ngaykt) });
        }
    }
}
