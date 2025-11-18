using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Areas.Admin.ViewModels;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;
using System.Security.Claims;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class PhieuMuaAdminController : Controller
    {
        private readonly IPhieuMuaAdmin _pmrepo;
        ShoesDbContext _db;
        public PhieuMuaAdminController(IPhieuMuaAdmin pmrepo, ShoesDbContext db)
        {
            _pmrepo = pmrepo;
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_pmrepo.GetAllPhieuMua().ToList());
        }

        public IActionResult Edit(int id)
        {
            Phieumua phieuMua = _pmrepo.GetPhieuMuaById(id);

            var currentUserEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var currentEmployee = _db.Nhanviens.FirstOrDefault(e => e.Email == currentUserEmail);

            if (currentEmployee != null)
            {
                phieuMua.Manv = currentEmployee.Manv;
            }

            var chitietphieumua = _db.Chitietphieumuas.Where(ct => ct.Mapm == id)
                .Include(x => x.MaspsizeNavigation)
                .ThenInclude(x => x.MasizeNavigation)
                .Include(x => x.MaspsizeNavigation)
                .ThenInclude(x => x.MaspNavigation)
                .ThenInclude(x => x.MadongsanphamNavigation)
                .Include(x => x.MaspsizeNavigation)
                .ThenInclude(x => x.MaspNavigation)
                .ThenInclude(x => x.MamauNavigation)
                .ToList();

            var ctpm = new PhieuMuaDetailViewModel
            {
                Phieumua = phieuMua,
                Chitietphieumuas = chitietphieumua
            };

            return View(ctpm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Phieumua phieuMua, string oldState)
        {
            try
            {
                _pmrepo.UpdatePhieuMua(phieuMua, id, oldState);
                TempData["Success"] = "Cập nhật phiếu mua thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật phiếu mua: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
