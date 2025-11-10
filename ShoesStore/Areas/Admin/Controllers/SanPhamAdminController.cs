using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Areas.Admin.Repositories;
using ShoesStore.Areas.Admin.ViewModels;
using ShoesStore.Models;
using ShoesStore.Models.Authentication;
using System.Diagnostics;
using System.Net.Mime;

namespace ShoesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthenticationM_S]
    public class SanPhamAdminController : Controller
    {
        ISanPhamAdmin prDetailAdmin;
        IDongsanphamAdmin pAdmin;
        ILoaiAdmin loaiAdmin;
        ISizeAdmin sizeAdmin;
        ISpSizeAdmin tkhoAdmin;
        IMauAdmin mauAdmin;
        IWebHostEnvironment hostEnvironment;
        public SanPhamAdminController(ISanPhamAdmin _prDetailAdmin,
            IDongsanphamAdmin _pAdmin, ILoaiAdmin _loaiAdmin, ISizeAdmin _sizeAdmin,
             ISpSizeAdmin _tkhoAdmin, IMauAdmin _mauAdmin, IWebHostEnvironment _hostEnvironment)
        {
            prDetailAdmin = _prDetailAdmin;
            pAdmin = _pAdmin;
            loaiAdmin = _loaiAdmin;
            sizeAdmin = _sizeAdmin;
            hostEnvironment = _hostEnvironment;
            tkhoAdmin = _tkhoAdmin;
            mauAdmin = _mauAdmin;
        }

        public IActionResult SanPhamList(int madongsp)
        {
            List<Sanpham> ctSp = prDetailAdmin.GetCTSPList(madongsp);
            TempData["madongsp"] = madongsp;

            Dongsanpham sp = pAdmin.GetDongsanphamById(madongsp);
            int maloai = sp.Maloai;

            Loai l = loaiAdmin.GetLoaiById(maloai);

            string TenLoai = l.Tenloai;
            string tendongsp = sp.Tendongsp;

            TempData["Tendongsp"] = tendongsp;
            TempData["TenLoai"] = TenLoai;
            return View(ctSp);
        }

        public IActionResult AddSanPham(int madongsp)
        {
            List<string> szName = sizeAdmin.GetSizeNameList();
            CreateData(madongsp);

            SanPhamViewModel pDetailVM = new SanPhamViewModel
            {
                DongsanphamId = madongsp,
                DongsanphamName = pAdmin.GetDongsanphamById(madongsp).Tendongsp,
                TypeName = loaiAdmin.GetLoaiById(pAdmin.GetDongsanphamById(madongsp).Maloai).Tenloai,
                tenSize = szName,
                slton = new List<int>()
            };

            for (int i = 0; i < szName.Count(); ++i)
            {
                pDetailVM.slton.Add(0);
            }

            return View(pDetailVM);
        }

        [HttpPost]
        public IActionResult AddSanPham(SanPhamViewModel pDetailView)
        {
            CreateData(pDetailView.DongsanphamId);
            //Kiểm tra mã m
            if (string.IsNullOrEmpty(pDetailView.IdMau))
            {
                TempData["Error"] = "Hiện tại chưa có mã màu trong hệ thống hoặc bạn chưa chọn màu.";
                return RedirectToAction("SanPhamList", "SanPhamAdmin", new { madongsp = pDetailView.DongsanphamId });
            }

            // Không cần kiểm tra ModelState, vì view đã kiểm tra lỗi nhập liệu
            string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, "img", pDetailView.DongsanphamId.ToString());
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // 🔹 Xử lý ảnh đại diện
            string avatarImage = "default.jpg"; // giá trị mặc định nếu không chọn
            if (pDetailView.AvatarImage != null)
            {
                avatarImage = Guid.NewGuid().ToString() + "_" + pDetailView.AvatarImage.FileName;
                string filepath = Path.Combine(uploadFolder, avatarImage);
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    pDetailView.AvatarImage.CopyTo(stream);
                }
            }

            // 🔹 Xử lý ảnh mặt trên
            string topImage = "default.jpg";
            if (pDetailView.TopImage != null)
            {
                topImage = Guid.NewGuid().ToString() + "_" + pDetailView.TopImage.FileName;
                string filepathTop = Path.Combine(uploadFolder, topImage);
                using (var stream = new FileStream(filepathTop, FileMode.Create))
                {
                    pDetailView.TopImage.CopyTo(stream);
                }
            }

            // 🔹 Xử lý ảnh đế giày
            string bottomImage = "default.jpg";
            if (pDetailView.BottomImage != null)
            {
                bottomImage = Guid.NewGuid().ToString() + "_" + pDetailView.BottomImage.FileName;
                string filepathBottom = Path.Combine(uploadFolder, bottomImage);
                using (var stream = new FileStream(filepathBottom, FileMode.Create))
                {
                    pDetailView.BottomImage.CopyTo(stream);
                }
            }

            // 🔹 Xử lý video nếu có
            string videoPath = "";
            if (pDetailView.VideoFile != null)
            {
                string videosUploadFolder = Path.Combine(hostEnvironment.WebRootPath, "videos", pDetailView.DongsanphamId.ToString());
                if (!Directory.Exists(videosUploadFolder))
                {
                    Directory.CreateDirectory(videosUploadFolder);
                }
                videoPath = ProcessUploadedFile(pDetailView.VideoFile, videosUploadFolder);
            }

            // 🔹 Tạo mới đối tượng sản phẩm
            Sanpham ctSp = new Sanpham
            {
                Madongsanpham = pDetailView.DongsanphamId,
                Mamau = pDetailView.IdMau,
                Anhdaidien = avatarImage,
                Anhmattren = topImage,
                Anhdegiay = bottomImage,
                Video = videoPath,
                TrangThai = (Sanpham.TrangThaiEnum)pDetailView.TrangThai,
            };

            // 🔹 Lưu DB
            prDetailAdmin.AddChitietSp(ctSp);

            // 🔹 Lưu danh sách size
            List<int> IdSizeList = sizeAdmin.GetMasizeList();
            List<Sanphamsize> tkhos = IdSizeList.Zip(pDetailView.slton,
                (idSize, SAndAmount) => new Sanphamsize
                {
                    Masp = ctSp.Masp,
                    Masize = idSize,
                    Slton = SAndAmount
                }).ToList();

            tkhoAdmin.AddListTonKho(tkhos);

            // ✅ Thông báo thành công
            TempData["Success"] = "Đã thêm sản phẩm mới thành công!";

            return RedirectToAction("SanPhamList", "SanPhamAdmin", new { madongsp = pDetailView.DongsanphamId });
        }
        private string ProcessUploadedFile(IFormFile file, string uploadFolder)
        {
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }

        public IActionResult UpdateSanPham(int masp)
        {
            Sanpham sp = prDetailAdmin.GetChitietSpById(masp);

            List<string> szName = sizeAdmin.GetSizeNameList();

            CreateData(sp.Madongsanpham, sp);

            SanPhamViewModel pDetailVM = new SanPhamViewModel
            {
                DongsanphamId = sp.Madongsanpham,
                DongsanphamName = sp.MadongsanphamNavigation.Tendongsp,
                TypeName = loaiAdmin.GetLoaiById(sp.MadongsanphamNavigation.Maloai).Tenloai,
                tenSize = szName,
                slton = sp.Sanphamsizes.Select(x => x.Slton).ToList(),
                TrangThai = (SanPhamViewModel.TrangThaiEnum)sp.TrangThai
            };

            return View(pDetailVM);
        }

        [HttpPost]
        public IActionResult UpdateSanPham(int masp, SanPhamViewModel pDetailView)
        {
            Sanpham sp = prDetailAdmin.GetChitietSpById(masp);
            CreateData(pDetailView.DongsanphamId, sp);

            try
            {
                string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, "img", pDetailView.DongsanphamId.ToString());
                if (pDetailView.AvatarImage != null)
                {
                    string avatarImg = Guid.NewGuid().ToString() + "_" + pDetailView.AvatarImage.FileName;
                    string filePath = Path.Combine(uploadFolder, avatarImg);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        pDetailView.AvatarImage.CopyTo(stream);
                        stream.Close();
                    }
                    DeleteImage(pDetailView.DongsanphamId, sp.Anhdaidien);
                    sp.Anhdaidien = avatarImg;
                }
                if (pDetailView.TopImage != null)
                {
                    string topImg = Guid.NewGuid().ToString() + "_" + pDetailView.TopImage.FileName;
                    string filepath2 = Path.Combine(uploadFolder, topImg);
                    using (var stream = new FileStream(filepath2, FileMode.Create))
                    {
                        pDetailView.TopImage.CopyTo(stream);
                        stream.Close();
                    }
                    DeleteImage(pDetailView.DongsanphamId, sp.Anhmattren);
                    sp.Anhmattren = topImg;
                }
                if (pDetailView.BottomImage != null)
                {
                    string bottomImg = Guid.NewGuid().ToString() + "_" + pDetailView.BottomImage.FileName;
                    string filepath3 = Path.Combine(uploadFolder, bottomImg);
                    using (var stream = new FileStream(filepath3, FileMode.Create))
                    {
                        pDetailView.BottomImage.CopyTo(stream);
                        stream.Close();
                    }
                    DeleteImage(pDetailView.DongsanphamId, sp.Anhdegiay);
                    sp.Anhdegiay = bottomImg;
                }
                if (pDetailView.VideoFile != null)
                {
                    string videosUploadFolder = Path.Combine(hostEnvironment.WebRootPath, "videos", pDetailView.DongsanphamId.ToString());
                    if (!Directory.Exists(videosUploadFolder))
                    {
                        Directory.CreateDirectory(videosUploadFolder);
                    }
                    string videoPath = ProcessUploadedFile(pDetailView.VideoFile, videosUploadFolder);
                    if (!string.IsNullOrEmpty(sp.Video))
                    {
                        DeleteVideo(pDetailView.DongsanphamId, sp.Video);
                    }
                    sp.Video = videoPath;
                }

                sp.TrangThai = (Sanpham.TrangThaiEnum)pDetailView.TrangThai;
                prDetailAdmin.UpdateChitietSp(sp);

                List<int> MaSizeList = sizeAdmin.GetMasizeList();
                List<Sanphamsize> spSize = MaSizeList.Zip(pDetailView.slton,
                                                        (Masize, slton) => new Sanphamsize
                                                        {
                                                            Masp = masp,
                                                            Masize = Masize,
                                                            Slton = slton,
                                                        }).ToList();

                tkhoAdmin.UpdateListSpSize(spSize);

                // ✅ Thông báo khi cập nhật thành công
                TempData["Success"] = "Đã cập nhật sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật sản phẩm: " + ex.Message;
            }

            return RedirectToAction("SanPhamList", "SanPhamAdmin", new { madongsp = pDetailView.DongsanphamId });
        }

        public IActionResult DeleteSanPham(int masp)
        {
            try
            {
                var ctSp = prDetailAdmin.GetChitietSpById(masp);

                if (ctSp == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm để xóa.";
                    return RedirectToAction("SanPhamList");
                }

                int madongsp = ctSp.Madongsanpham;

                // ✅ Xóa file ảnh & video
                DeleteImage(madongsp, ctSp.Anhdaidien);
                DeleteImage(madongsp, ctSp.Anhmattren);
                DeleteImage(madongsp, ctSp.Anhdegiay);
                DeleteVideo(madongsp, ctSp.Video);

                // ✅ Xóa sản phẩm & dữ liệu liên quan
                prDetailAdmin.DeleteChitietSp(masp);

                TempData["Success"] = "Đã xóa sản phẩm thành công!";
                return RedirectToAction("SanPhamList", new { madongsp = madongsp });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
                return RedirectToAction("SanPhamList");
            }
        }



        public void DeleteImage(int madongsp, string imagePath)
        {
            string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, "img", madongsp.ToString());
            string filepath = Path.Combine(uploadFolder, imagePath);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
        }

        public void DeleteVideo(int madongsp, string? videoPath)
        {
            string videosFolder = Path.Combine(hostEnvironment.WebRootPath, "videos", madongsp.ToString());
            string videoFilePath = Path.Combine(videosFolder, videoPath ?? "");
            if (System.IO.File.Exists(videoFilePath))
            {
                System.IO.File.Delete(videoFilePath);
            }
        }

        public void CreateData(int madongsp, Sanpham sp = null)
        {
            List<Mau> mauList = mauAdmin.GetAllMauNotUsedForSp(madongsp);
            if (sp != null)
            {
                mauList.Insert(0, sp.MamauNavigation);
                ViewBag.Anhdaidien = sp.Anhdaidien;
                ViewBag.Anhmattren = sp.Anhmattren;
                ViewBag.Anhdegiay = sp.Anhdegiay;
                ViewBag.Video = sp.Video;
                ViewBag.Masp = sp.Masp;
            }
            ViewBag.ColorChoice = new SelectList(mauList, "Mamau", "Tenmau");
        }
    }
}
