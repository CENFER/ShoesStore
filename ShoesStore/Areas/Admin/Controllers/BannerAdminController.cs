using Microsoft.AspNetCore.Mvc;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Areas.Admin.ViewModels;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;
using System;
using System.IO;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class BannerAdminController : Controller
    {
        IWebHostEnvironment hostingenvironment;
        private readonly IBannerRepository _bannerRepository;

        public BannerAdminController(IBannerRepository bannerRepository, IWebHostEnvironment hc)
        {
            _bannerRepository = bannerRepository;
            hostingenvironment = hc;
        }

        public IActionResult IndexBanner()
        {
            var banners = _bannerRepository.GetBanners();
            return View(banners);
        }

        [HttpGet]
        public IActionResult CreateBanner()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateBanner(BannerViewModel bannerViewModel)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                try
                {
                    if (bannerViewModel.FrontImage != null)
                    {
                        string uploadsFolder = Path.Combine(hostingenvironment.WebRootPath, "img", "banner");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        fileName = Guid.NewGuid().ToString() + "_" + bannerViewModel.FrontImage.FileName;
                        string filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            bannerViewModel.FrontImage.CopyTo(fileStream);
                        }
                    }

                    Banner banner = new Banner
                    {
                        Tenbanner = bannerViewModel.Tenbanner,
                        Vitri = fileName,
                        Link = bannerViewModel.Link,
                        Hoatdong = bannerViewModel.Hoatdong,
                        Slogan = bannerViewModel.Slogan
                    };

                    try
                    {
                        _bannerRepository.AddBanner(banner);
                        TempData["Success"] = "Banner created successfully!";
                    }
                    catch (Exception exRepo)
                    {
                        TempData["Error"] = "Error creating banner: " + exRepo.Message;
                        return View(bannerViewModel);
                    }

                    return RedirectToAction("IndexBanner");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error processing file or creating banner: " + ex.Message;
                    return View(bannerViewModel);
                }
            }
            return View(bannerViewModel);
        }

        [HttpGet]
        public IActionResult EditBanner(int id)
        {
            var banner = _bannerRepository.GetBannerById(id);
            if (banner == null)
            {
                return NotFound();
            }

            // Chuyển đổi Banner thành BannerViewModel trước khi truyền vào view
            var bannerViewModel = new BannerViewModel
            {
                Mabanner = banner.Mabanner,
                Tenbanner = banner.Tenbanner,
                Link = banner.Link,
                Hoatdong = banner.Hoatdong,
                Slogan = banner.Slogan
            };

            return View(bannerViewModel);
        }

        [HttpPost]
        public IActionResult EditBanner(BannerViewModel bannerViewModel)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                try
                {
                    if (bannerViewModel.FrontImage != null)
                    {
                        string uploadsFolder = Path.Combine(hostingenvironment.WebRootPath, "img", "banner");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        fileName = Guid.NewGuid().ToString() + "_" + bannerViewModel.FrontImage.FileName;
                        string filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            bannerViewModel.FrontImage.CopyTo(fileStream);
                        }
                    }

                    Banner banner = new Banner
                    {
                        Mabanner = bannerViewModel.Mabanner,
                        Tenbanner = bannerViewModel.Tenbanner,
                        Vitri = fileName,
                        Link = bannerViewModel.Link,
                        Hoatdong = bannerViewModel.Hoatdong,
                        Slogan = bannerViewModel.Slogan
                    };

                    try
                    {
                        _bannerRepository.UpdateBanner(banner);
                        TempData["Success"] = "Banner updated successfully!";
                    }
                    catch (Exception exRepo)
                    {
                        TempData["Error"] = "Error updating banner: " + exRepo.Message;
                        return View(bannerViewModel);
                    }

                    return RedirectToAction("IndexBanner");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error processing file or updating banner: " + ex.Message;
                    return View(bannerViewModel);
                }
            }
            return View(bannerViewModel);
        }

        [HttpPost]
        public IActionResult DeleteBanner(int id)
        {
            try
            {
                _bannerRepository.RemoveBanner(id);
                TempData["Success"] = "Banner deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting banner: " + ex.Message;
            }
            return RedirectToAction("IndexBanner");
        }
    }
}
