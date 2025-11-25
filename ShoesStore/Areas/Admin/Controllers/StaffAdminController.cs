using Microsoft.AspNetCore.Mvc;
using ShoesStore.Areas.Admin.ViewModels;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;
using System.Diagnostics;
using System.Linq;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationAdmin]

    public class StaffAdminController : Controller
    {
        private readonly ShoesDbContext _db;

        public StaffAdminController(ShoesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult ListEmployees()
        {
            return View(_db.Nhanviens.ToList());
        }

        public IActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateEmployee(RegisterAdminViewModel rgAdmin)
        {
            Taikhoan newTk = new Taikhoan
            {
                Email = rgAdmin.Nhanvien.Email,
                Matkhau = rgAdmin.Taikhoan.Matkhau,
                Loaitk = 1
            };

            rgAdmin.Taikhoan = newTk;
            rgAdmin.Nhanvien.EmailNavigation.Email = rgAdmin.Nhanvien.Email;
            rgAdmin.Nhanvien.EmailNavigation.Loaitk = 1;

            rgAdmin.Nhanvien.Diachi = rgAdmin.Nhanvien.Diachi == "123" ? "" : rgAdmin.Nhanvien.Diachi;

            var existingEmployee = _db.Taikhoans.FirstOrDefault(c => c.Email == rgAdmin.Taikhoan.Email);

            if (existingEmployee != null)
            {
                ModelState.AddModelError("Nhanvien.Email", "Email already exists.");
                TempData["Error"] = "Email already exists. Please use another email.";
                return View(rgAdmin);
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid input. Please check the form again.";
                return View(rgAdmin);
            }

            _db.Nhanviens.Add(rgAdmin.Nhanvien);
            _db.SaveChanges();

            TempData["Success"] = "Employee added successfully.";
            return RedirectToAction("ListEmployees");
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _db.Nhanviens.Find(id);

            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return NotFound();
            }

            employee.EmailNavigation = _db.Taikhoans.Find(employee.Email);

            return View(employee);
        }

        [HttpPost]
        public IActionResult EditEmployee(Nhanvien employee)
        {
            if (ModelState.IsValid)
            {
                _db.Nhanviens.Update(employee);
                _db.SaveChanges();

                TempData["Success"] = "Employee updated successfully.";
                return RedirectToAction("ListEmployees");
            }

            TempData["Error"] = "Invalid update data. Please check again.";
            return View(employee);
        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _db.Nhanviens.Find(id);

            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return NotFound();
            }

            _db.Nhanviens.Remove(employee);
            _db.SaveChanges();

            TempData["Success"] = "Employee deleted successfully.";
            return RedirectToAction("ListEmployees");
        }
    }
}
