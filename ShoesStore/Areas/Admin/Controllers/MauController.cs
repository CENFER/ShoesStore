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
                TempData["Error"] = "Invalid data.";
                return View(mau);
            }

            // ⚠️ Kiểm tra trùng mã màu
            var allIds = _repo.GetAllIdMau();
            if (allIds.Contains(mau.Mamau))
            {
                TempData["Error"] = "This color code already exists in the system.";
                return RedirectToAction("Index");
            }

            try
            {
                _repo.AddColors(mau);
                TempData["Success"] = "New color added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding color: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(string Id)
        {
            Mau mau = _repo.GetColorsById(Id);
            if (mau == null)
            {
                TempData["Error"] = "Color to edit not found.";
                return RedirectToAction("Index");
            }
            return View(mau);
        }

        [HttpPost]
        public IActionResult Edit(Mau mau, string Id)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data.";
                return View(mau);
            }

            try
            {
                _repo.UpdateColors(mau, Id);
                TempData["Success"] = "Color updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating color: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            try
            {
                _repo.DeleteColors(id);
                TempData["Success"] = "Color deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting color: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
