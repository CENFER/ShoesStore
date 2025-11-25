using Microsoft.AspNetCore.Mvc;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class VoucherAdminController : Controller
    {
        private readonly IVoucherAdmin _voucherRepo;
        private readonly ShoesDbContext _context;

        public VoucherAdminController(IVoucherAdmin voucherRepo, ShoesDbContext context)
        {
            _voucherRepo = voucherRepo;
            _context = context;
        }

        public IActionResult Index()
        {
            List<Voucher> vouchers = _voucherRepo.GetAllVouchers();
            return View(vouchers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Voucher voucher)
        {
            bool voucherExists = _context.Vouchers.Any(v => v.Mavoucher == voucher.Mavoucher);
            if (voucherExists)
            {
                ModelState.AddModelError("Mavoucher", $"Voucher code '{voucher.Mavoucher}' already exists!");
            }

            if (voucher.Ngaytao > voucher.Ngayhethan)
            {
                ModelState.AddModelError("Ngayhethan", "Expiration date must be greater than or equal to the creation date.");
            }

            if (voucher.Giatoithieu > voucher.Giamtoida)
            {
                ModelState.AddModelError("Giamtoida", "Minimum value cannot be greater than maximum value.");
                TempData["Error"] = "Minimum value cannot be greater than maximum value.";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _voucherRepo.AddVoucher(voucher);
                    TempData["Success"] = "Voucher added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error: " + ex.Message;
                    return View(voucher);
                }
            }

            TempData["Error"] = "Please check your input and try again.";
            return View(voucher);
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid voucher code!";
                return RedirectToAction(nameof(Index));
            }

            var voucher = _voucherRepo.GetVoucherById(id);
            if (voucher == null)
            {
                TempData["Error"] = "Voucher not found!";
                return RedirectToAction(nameof(Index));
            }

            return View(voucher);
        }

        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid voucher code!";
                return RedirectToAction(nameof(Index));
            }

            var voucher = _voucherRepo.GetVoucherById(id);
            if (voucher == null)
            {
                TempData["Error"] = "Voucher not found!";
                return RedirectToAction(nameof(Index));
            }

            return View(voucher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Voucher voucher)
        {
            if (voucher.Ngaytao > voucher.Ngayhethan)
            {
                ModelState.AddModelError("Ngayhethan", "Expiration date must be greater than or equal to the creation date.");
            }

            if (voucher.Giatoithieu > voucher.Giamtoida)
            {
                ModelState.AddModelError("Giamtoida", "Minimum value cannot be greater than maximum value.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _voucherRepo.UpdateVoucher(voucher);
                    TempData["Success"] = "Voucher updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error: " + ex.Message;
                    return View(voucher);
                }
            }

            TempData["Error"] = "Please check your input and try again.";
            return View(voucher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                _voucherRepo.DeleteVoucher(id);
                TempData["Success"] = "Voucher deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting voucher: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
