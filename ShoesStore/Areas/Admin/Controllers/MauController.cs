using Microsoft.AspNetCore.Mvc;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;
using System.Linq;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class MauController : Controller
    {
        private readonly IMauAdmin _repo;
        public MauController(IMauAdmin repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var maus = _repo.GetAllColors().ToList();
            return View(maus);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Mau mau)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return View(mau);
            }

            // ⚠️ Kiểm tra trùng mã màu
            var allIds = _repo.GetAllIdMau();
            if (allIds.Contains(mau.Mamau))
            {
                TempData["Error"] = "Đã tồn tại mã màu này trong hệ thống.";
                return RedirectToAction("Index");
            }

            try
            {
                _repo.AddColors(mau);
                TempData["Success"] = "Thêm màu mới thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm màu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(string Id)
        {
            Mau mau = _repo.GetColorsById(Id);
            if (mau == null)
            {
                TempData["Error"] = "Không tìm thấy màu cần sửa.";
                return RedirectToAction("Index");
            }
            return View(mau);
        }

        [HttpPost]
        public IActionResult Edit(Mau mau, string Id)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return View(mau);
            }

            try
            {
                _repo.UpdateColors(mau, Id);
                TempData["Success"] = "Cập nhật màu thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật màu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            try
            {
                _repo.DeleteColors(id);
                TempData["Success"] = "Xóa màu thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa màu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
