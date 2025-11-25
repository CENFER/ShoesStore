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
                    TempData["Error"] = "Please fill in all required information.";
                    return View(loai);
                }

                // Kiểm tra trùng tên
                var existed = _lrepo
                    .GetDistinctLoai()
                    .Any(x => x.Trim().ToLower() == loai.Tenloai.Trim().ToLower());

                if (existed)
                {
                    TempData["Error"] = "This category already exists in the system.";
                    return View(loai);
                }

                _lrepo.AddLoai(loai);
                TempData["Success"] = "Category added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while adding: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Edit(int Id)
        {
            Loai loai = _lrepo.GetLoaiById(Id);
            if (loai == null)
            {
                TempData["Error"] = "Category not found.";
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
                    TempData["Error"] = "Please enter valid information.";
                    return View(loai);
                }

                // Kiểm tra trùng tên (ngoại trừ chính nó)
                var existed = _lrepo
                    .GetDistinctLoai()
                    .Any(x => x.Trim().ToLower() == loai.Tenloai.Trim().ToLower());

                if (existed)
                {
                    TempData["Error"] = "This category name already exists.";
                    return View(loai);
                }

                _lrepo.UpdateLoai(loai, Id);
                TempData["Success"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while updating: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _lrepo.DeleteLoai(id);
                TempData["Success"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while deleting: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
