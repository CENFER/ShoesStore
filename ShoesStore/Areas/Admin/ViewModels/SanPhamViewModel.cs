using ShoesStore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesStore.Areas.Admin.ViewModels
{
    public class SanPhamViewModel
    {
        public int DongsanphamId { get; set; }
        public string DongsanphamName { get; set; }
        public string TypeName {  get; set; }
        public enum TrangThaiEnum
        {
            [VietnameseName("On sale")]
            Dangban = 1,

            [VietnameseName("Stopped selling")]
            NgungBan = 2,

            [VietnameseName("Hot product")]
            Hot = 3,

            [VietnameseName("Newest")]
            New = 4
        }



        public TrangThaiEnum TrangThai { get; set; }

        [Required(ErrorMessage = "No color selected for the product")]
        public string IdMau { get; set; }

        public List<string> tenSize { get; set; }
        public List<int> slton { get; set; }

        [Required(ErrorMessage = "Please provide a main image")]
        [Display(Name = "Main image")]
        [NotMapped] // Don't save this image to database, save in wwwroot
        public IFormFile AvatarImage { get; set; }

        [Required(ErrorMessage = "Please provide a top image")]
        [Display(Name = "Top shoe image")]
        [NotMapped]
        public IFormFile TopImage { get; set; }

        [Required(ErrorMessage = "Please provide a sole image")]
        [Display(Name = "Sole image")]
        [NotMapped]
        public IFormFile BottomImage { get; set; }

        public IFormFile VideoFile { get; set; }
    }
}
