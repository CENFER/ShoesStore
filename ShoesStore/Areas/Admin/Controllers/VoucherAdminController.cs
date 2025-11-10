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

        // CHỈ CÓ 1 CONSTRUCTOR DUY NHẤT
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
            // KIỂM TRA TRÙNG MÃ
            bool voucherExists = _context.Vouchers.Any(v => v.Mavoucher == voucher.Mavoucher);
            if (voucherExists)
            {
                ModelState.AddModelError("Mavoucher", $"Mã voucher '{voucher.Mavoucher}' đã tồn tại trong hệ thống!");
            }

            // KIỂM TRA NGÀY
            if (voucher.Ngaytao > voucher.Ngayhethan)
            {
                ModelState.AddModelError("Ngayhethan", "Ngày hết hạn phải lớn hơn hoặc bằng ngày tạo");
            }

            // KIỂM TRA GIÁ
            if (voucher.Giatoithieu > voucher.Giamtoida)
            {
                ModelState.AddModelError("Giamtoida", "Giá tối thiểu không được lớn hơn giá tối đa");
                TempData["Error"] = "Giá tối thiểu không được lớn hơn giá tối đa";
            }

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

            TempData["Error"] = "Vui lòng kiểm tra lại thông tin!";
            return View(voucher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Voucher voucher)
        {
            // KIỂM TRA NGÀY
            if (voucher.Ngaytao > voucher.Ngayhethan)
            {
                ModelState.AddModelError("Ngayhethan", "Ngày hết hạn phải lớn hơn hoặc bằng ngày tạo");
            }

            // THÊM KIỂM TRA GIÁ
            if (voucher.Giatoithieu > voucher.Giamtoida)
            {
                ModelState.AddModelError("Giamtoida", "Giá tối thiểu không được lớn hơn giá tối đa");
            }

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

            TempData["Error"] = "Vui lòng kiểm tra lại thông tin!";
            return View(voucher);
        }

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