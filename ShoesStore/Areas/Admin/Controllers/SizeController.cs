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
            var sizes = _repo.GetAllSizes().ToList(); // ✅ cast to List
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
                // ✅ Check duplicate size name (case-insensitive)
                var existingSize = _repo.GetAllSizes()
                    .FirstOrDefault(s => s.Tensize.Trim().ToLower() == size.Tensize.Trim().ToLower());

                if (existingSize != null)
                {
                    TempData["Error"] = $"Size '{size.Tensize}' already exists in the system!";
                    return RedirectToAction("Index");
                }

                _repo.AddSizes(size);
                TempData["Success"] = "New size added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding size: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var size = _repo.GetSizesById(id);
            if (size == null)
            {
                TempData["Error"] = "Size not found for editing.";
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
                // ✅ Check duplicate name (exclude the size being edited)
                var existingSize = _repo.GetAllSizes()
                    .FirstOrDefault(s =>
                        s.Tensize.Trim().ToLower() == size.Tensize.Trim().ToLower() &&
                        s.Masize != id);

                if (existingSize != null)
                {
                    TempData["Error"] = $"Size name '{size.Tensize}' already exists!";
                    return RedirectToAction("Index");
                }

                _repo.UpdateSizes(size, id);
                TempData["Success"] = "Size updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating size: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            try
            {
                _repo.DeleteSizes(id);
                TempData["Success"] = "Size deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting size: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
