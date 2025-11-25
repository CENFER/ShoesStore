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
                    TempData["Success"] = "Product line added successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Please check the entered data.";
                ViewBag.Selectloai = GetSelectListItems();
                return View(dongsanpham);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding product line: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Edit(int id)
        {
            var dongsanpham = _dongsanphamRepo.GetDongsanphamById(id);
            if (dongsanpham == null)
            {
                TempData["Error"] = "Product line not found.";
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
                    TempData["Success"] = "Product line updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Invalid data. Please check again.";
                return View(dongsanpham);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating product line: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _dongsanphamRepo.DeleteDongsanpham(id);
                TempData["Success"] = "Product line deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting product line: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
