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

        // GET: Index
        public IActionResult Index()
        {
            List<Voucher> vouchers = _voucherRepo.GetAllVouchers();
            return View(vouchers);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Voucher voucher)
        {
            // Validation logic...
            if (ModelState.IsValid)
            {
                try
                {
                    _voucherRepo.AddVoucher(voucher);
                    TempData["Success"] = "Đã thêm voucher thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi: " + ex.Message;
                    return View(voucher);
                }
            }
            return View(voucher);
        }

        // GET: Edit
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Mã voucher không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var voucher = _voucherRepo.GetVoucherById(id);
            if (voucher == null)
            {
                TempData["Error"] = "Voucher không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            return View(voucher);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Voucher voucher)
        {
            // Validation logic...
            if (ModelState.IsValid)
            {
                try
                {
                    _voucherRepo.UpdateVoucher(voucher);
                    TempData["Success"] = "Đã cập nhật voucher thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi: " + ex.Message;
                    return View(voucher);
                }
            }
            return View(voucher);
        }

        // GET: Delete (Hiển thị xác nhận)
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Mã voucher không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var voucher = _voucherRepo.GetVoucherById(id);
            if (voucher == null)
            {
                TempData["Error"] = "Voucher không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            return View(voucher);
        }

        // POST: Delete (Xác nhận xóa)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                _voucherRepo.DeleteVoucher(id);
                TempData["Success"] = "Đã xóa voucher thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa voucher: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}