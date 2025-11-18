using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;
using ShoesStore.ViewModels;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class LoaiController : Controller
    {
        private readonly ILoaiAdmin _lrepo;
        public LoaiController(ILoaiAdmin lrepo)
        {
            _lrepo = lrepo;
        }

        public IActionResult Index()
        {
            return View(_lrepo.GetAllLoai().ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Idloai,Tenloai,Slogan")] Loai loai)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                    return View(loai);
                }

                // Kiểm tra trùng tên
                var existed = _lrepo
                    .GetDistinctLoai()
                    .Any(x => x.Trim().ToLower() == loai.Tenloai.Trim().ToLower());

                if (existed)
                {
                    TempData["Error"] = "Đã tồn tại loại giày này trong hệ thống.";
                    return View(loai);
                }

                _lrepo.AddLoai(loai);
                TempData["Success"] = "Thêm loại giày mới thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Edit(int Id)
        {
            Loai loai = _lrepo.GetLoaiById(Id);
            if (loai == null)
            {
                TempData["Error"] = "Không tìm thấy loại giày.";
                return RedirectToAction("Index");
            }
            return View(loai);
        }

        [HttpPost]
        public IActionResult Edit(Loai loai, int Id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Vui lòng nhập thông tin hợp lệ.";
                    return View(loai);
                }

                // Kiểm tra trùng tên (ngoại trừ chính nó)
                var existed = _lrepo
                    .GetDistinctLoai()
                    .Any(x => x.Trim().ToLower() == loai.Tenloai.Trim().ToLower());

                if (existed)
                {
                    TempData["Error"] = "Tên loại giày này đã tồn tại.";
                    return View(loai);
                }

                _lrepo.UpdateLoai(loai, Id);
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi sửa: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                _lrepo.DeleteLoai(id);
                TempData["Success"] = "Đã xóa loại giày thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
