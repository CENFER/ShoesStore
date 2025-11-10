using Microsoft.AspNetCore.Mvc;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Areas.Admin.Repositories;
using ShoesStore.Models;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly ISizeAdmin _repo;

        public SizeController(ISizeAdmin repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var sizes = _repo.GetAllSizes().ToList(); // ✅ ép sang List
            return View(sizes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Size size)
        {
            if (!ModelState.IsValid)
                return View(size);

            try
            {
                // ✅ Kiểm tra trùng tên size (không phân biệt hoa thường)
                var existingSize = _repo.GetAllSizes()
                    .FirstOrDefault(s => s.Tensize.Trim().ToLower() == size.Tensize.Trim().ToLower());

                if (existingSize != null)
                {
                    TempData["Error"] = $"Kích cỡ '{size.Tensize}' đã tồn tại trong hệ thống!";
                    return RedirectToAction("Index");
                }

                _repo.AddSizes(size);
                TempData["Success"] = "Đã thêm kích cỡ mới thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm kích cỡ: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var size = _repo.GetSizesById(id);
            if (size == null)
            {
                TempData["Error"] = "Không tìm thấy kích cỡ để sửa.";
                return RedirectToAction("Index");
            }

            return View(size);
        }

        [HttpPost]
        public IActionResult Edit(Size size, int id)
        {
            if (!ModelState.IsValid)
                return View(size);

            try
            {
                // ✅ Kiểm tra trùng tên (loại trừ chính size đang sửa)
                var existingSize = _repo.GetAllSizes()
                    .FirstOrDefault(s =>
                        s.Tensize.Trim().ToLower() == size.Tensize.Trim().ToLower() &&
                        s.Masize != id);

                if (existingSize != null)
                {
                    TempData["Error"] = $"Tên kích cỡ '{size.Tensize}' đã tồn tại!";
                    return RedirectToAction("Index");
                }

                _repo.UpdateSizes(size, id);
                TempData["Success"] = "Đã cập nhật kích cỡ thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật kích cỡ: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            try
            {
                _repo.DeleteSizes(id);
                TempData["Success"] = "Đã xóa kích cỡ thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa kích cỡ: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
