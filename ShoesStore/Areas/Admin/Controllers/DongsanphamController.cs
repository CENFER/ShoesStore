using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class DongsanphamController : Controller
    {
        private readonly IDongsanphamAdmin _dongsanphamRepo;
        private readonly ILoaiAdmin _loairepo;

        public DongsanphamController(IDongsanphamAdmin dongsanphamRepo, ILoaiAdmin loairepo)
        {
            _dongsanphamRepo = dongsanphamRepo;
            _loairepo = loairepo;
        }

        public IActionResult Index()
        {
            var list = _dongsanphamRepo.GetAllDongsanpham().ToList();
            return View(list);
        }

        private SelectList GetSelectListItems()
        {
            var loaiList = _loairepo.GetAllLoai().ToList();
            return new SelectList(loaiList, "Maloai", "Tenloai");
        }

        public IActionResult Create()
        {
            ViewBag.Selectloai = GetSelectListItems();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Dongsanpham dongsanpham)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dongsanpham.MaloaiNavigation = null;
                    _dongsanphamRepo.AddDongsanpham(dongsanpham);
                    TempData["Success"] = "Đã thêm dòng sản phẩm mới thành công!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Vui lòng kiểm tra lại dữ liệu nhập vào.";
                ViewBag.Selectloai = GetSelectListItems();
                return View(dongsanpham);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm dòng sản phẩm: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Edit(int id)
        {
            var dongsanpham = _dongsanphamRepo.GetDongsanphamById(id);
            if (dongsanpham == null)
            {
                TempData["Error"] = "Không tìm thấy dòng sản phẩm cần sửa.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Selectloai = GetSelectListItems();
            return View(dongsanpham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Dongsanpham dongsanpham, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _dongsanphamRepo.UpdateDongsanpham(dongsanpham, id);
                    TempData["Success"] = "Đã cập nhật dòng sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
                return View(dongsanpham);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _dongsanphamRepo.DeleteDongsanpham(id);
                TempData["Success"] = "Đã xóa dòng sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa dòng sản phẩm: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
