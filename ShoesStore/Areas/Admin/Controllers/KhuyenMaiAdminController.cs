using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Areas.Admin.ViewModels;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class KhuyenMaiAdminController : Controller
    {
        private readonly IKhuyenMaiAdmin _kmrepo;
        private readonly ShoesDbContext _context;
        public KhuyenMaiAdminController(IKhuyenMaiAdmin kmrepo, ShoesDbContext context)
        {
            _kmrepo = kmrepo;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_kmrepo.GetAllKhuyenmai().ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Khuyenmai km)
        {
            // Reset ModelState để tránh lỗi trùng
            ModelState.Clear();

            bool hasError = false;

            // KIỂM TRA REQUIRED FIELDS
            bool isNgayBdValid = km.Ngaybd != default(DateTime) && km.Ngaybd.Year > 1900;
            bool isNgayKtValid = km.Ngaykt != default(DateTime) && km.Ngaykt.Year > 1900;

            if (!isNgayBdValid)
            {
                ModelState.AddModelError("Ngaybd", "Start date is required.");
                hasError = true;
            }

            if (!isNgayKtValid)
            {
                ModelState.AddModelError("Ngaykt", "End date is required.");
                hasError = true;
            }

            // KIỂM TRA PHẦN TRĂM GIẢM
            if (km.Phantramgiam < 1 || km.Phantramgiam > 100)
            {
                ModelState.AddModelError("Phantramgiam", "Discount percentage must be between 1% and 100%.");
                hasError = true;
            }

            // KIỂM TRA NGÀY
            if (isNgayBdValid && isNgayKtValid && km.Ngaybd >= km.Ngaykt)
            {
                ModelState.AddModelError("Ngaykt", "End date must be later than start date.");
                hasError = true;
            }

            if (!hasError)
            {
                try
                {
                    _kmrepo.AddKhuyenmai(km);
                    TempData["Success"] = "Promotion added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            return View(km);
        }

        // ACTION DELETE CHO GET REQUEST
        public IActionResult Delete(int id)
        {
            try
            {
                _kmrepo.DeleteKhuyenmai(id);
                TempData["Success"] = "Promotion deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting promotion: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddDongSanPham(int makm)
        {
            var khuyenmai = _context.Khuyenmais
                                    .Include(k => k.Madongsanphams).ThenInclude(x => x.MaloaiNavigation)
                                    .FirstOrDefault(k => k.Makm == makm);

            if (khuyenmai == null)
            {
                TempData["Error"] = "Promotion does not exist!";
                return RedirectToAction(nameof(Index));
            }

            var availableDongsanphams = _context.Dongsanphams
                .Select(d => new SelectListItem
                {
                    Value = d.Madongsanpham.ToString(),
                    Text = d.Tendongsp
                });

            var model = new KhuyenMaiViewModel
            {
                Makm = khuyenmai.Makm,
                Ngaybd = khuyenmai.Ngaybd,
                Ngaykt = khuyenmai.Ngaykt,
                Phantramgiam = khuyenmai.Phantramgiam,
                AvailableDongsanphams = availableDongsanphams
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDongSanPham(KhuyenMaiViewModel model)
        {
            var khuyenmai = _context.Khuyenmais
                                    .Include(k => k.Madongsanphams)
                                    .ThenInclude(x => x.MaloaiNavigation)
                                    .FirstOrDefault(k => k.Makm == model.Makm);

            if (khuyenmai == null)
            {
                TempData["Error"] = "Promotion does not exist!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var selectedDongsanphams = _context.Dongsanphams
                                                   .Where(d => model.SelectedDongsanphams.Contains(d.Madongsanpham))
                                                   .ToList();

                foreach (var dongsanpham in selectedDongsanphams)
                {
                    if (!khuyenmai.Madongsanphams.Contains(dongsanpham))
                    {
                        khuyenmai.Madongsanphams.Add(dongsanpham);
                    }
                }

                _context.SaveChanges();
                TempData["Success"] = "Product lines added to promotion successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding product lines: " + ex.Message;
            }

            return RedirectToAction("ListDongSanPham", new { makm = model.Makm });
        }

        public IActionResult ListDongSanPham(int makm)
        {
            var khuyenmai = _context.Khuyenmais
                                    .Include(k => k.Madongsanphams)
                                    .ThenInclude(m => m.MaloaiNavigation)
                                    .FirstOrDefault(k => k.Makm == makm);

            if (khuyenmai == null)
            {
                TempData["Error"] = "Promotion does not exist!";
                return RedirectToAction(nameof(Index));
            }

            return View(khuyenmai);
        }
    }
}
