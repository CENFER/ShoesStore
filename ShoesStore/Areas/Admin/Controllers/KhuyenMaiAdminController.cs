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

            // KIỂM TRA REQUIRED FIELDS - chỉ kiểm tra nếu là default value hoặc min value
            bool isNgayBdValid = km.Ngaybd != default(DateTime) && km.Ngaybd.Year > 1900; // Loại bỏ năm 0001
            bool isNgayKtValid = km.Ngaykt != default(DateTime) && km.Ngaykt.Year > 1900; // Loại bỏ năm 0001

            if (!isNgayBdValid)
            {
                ModelState.AddModelError("Ngaybd", "Ngày bắt đầu là bắt buộc");
                hasError = true;
            }

            if (!isNgayKtValid)
            {
                ModelState.AddModelError("Ngaykt", "Ngày kết thúc là bắt buộc");
                hasError = true;
            }

            // KIỂM TRA PHẦN TRĂM GIẢM
            // Debug để xem giá trị thực tế
            Console.WriteLine($"Phantramgiam value: {km.Phantramgiam}, Type: {km.Phantramgiam.GetType()}");

            if (km.Phantramgiam < 1 || km.Phantramgiam > 100)
            {
                // Đây là trường hợp nhập số nhưng không hợp lệ (bao gồm cả số 0)
                ModelState.AddModelError("Phantramgiam", "Phần trăm giảm phải từ 1% đến 100%");
                hasError = true;
            }

            // KIỂM TRA NGÀY (chỉ kiểm tra nếu cả 2 đều có giá trị hợp lệ)
            if (isNgayBdValid && isNgayKtValid && km.Ngaybd >= km.Ngaykt)
            {
                ModelState.AddModelError("Ngaykt", "Ngày kết thúc phải lớn hơn ngày bắt đầu");
                hasError = true;
            }

            // CHỈ THỰC HIỆN KHI KHÔNG CÓ LỖI
            if (!hasError)
            {
                try
                {
                    _kmrepo.AddKhuyenmai(km);
                    TempData["Success"] = "Đã thêm khuyến mãi thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi: " + ex.Message);
                }
            }

            // NẾU CÓ LỖI VALIDATION, TRẢ VỀ VIEW VỚI MODEL VÀ LỖI
            return View(km);
        }
        // ACTION DELETE CHO GET REQUEST
        public IActionResult Delete(int id)
        {
            try
            {
                _kmrepo.DeleteKhuyenmai(id);
                TempData["Success"] = "Đã xóa khuyến mãi thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa khuyến mãi: " + ex.Message;
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
                TempData["Error"] = "Khuyến mãi không tồn tại!";
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
                TempData["Error"] = "Khuyến mãi không tồn tại!";
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
                TempData["Success"] = "Đã thêm dòng sản phẩm vào khuyến mãi thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm dòng sản phẩm: " + ex.Message;
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
                TempData["Error"] = "Khuyến mãi không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            return View(khuyenmai);
        }
    }
}