using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Models;
using System.Linq;

namespace ShoesStore.Areas.Admin.Repositories
{
    public class KhuyenMaiAdminRepo : IKhuyenMaiAdmin
    {
        ShoesDbContext _context;
        public KhuyenMaiAdminRepo(ShoesDbContext context)
        {
            this._context = context;
        }

        public Khuyenmai GetKhuyenmaiById(int id)
        {
            Khuyenmai km = _context.Khuyenmais.FirstOrDefault(x => x.Makm == id);
            return km;
        }

        public IQueryable<Khuyenmai> GetAllKhuyenmai()
        {
            var khuyenmai = _context.Khuyenmais.Select(km => new Khuyenmai
            {
                Makm = km.Makm,
                Ngaybd = km.Ngaybd,
                Ngaykt = km.Ngaykt,
                Phantramgiam = km.Phantramgiam
            }).OrderByDescending(x => x.Ngaykt);
            return khuyenmai;
        }

        public void AddKhuyenmai(Khuyenmai km)
        {
            _context.Khuyenmais.Add(km);
            _context.SaveChanges();
        }

        public void DeleteKhuyenmai(int Id)
        {
            // Tìm khuyến mãi và include danh sách sản phẩm liên quan
            Khuyenmai km = _context.Khuyenmais.Include(k => k.Madongsanphams).FirstOrDefault(k => k.Makm == Id);

            if (km != null)
            {
                try
                {
                    // Bước 1: Xóa quan hệ many-to-many trong bảng CHITIETKHUYENMAI
                    foreach (var dongsanpham in km.Madongsanphams.ToList())
                    {
                        km.Madongsanphams.Remove(dongsanpham);
                    }

                    // Bước 2: Xóa khuyến mãi
                    _context.Khuyenmais.Remove(km);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // Xử lý lỗi khóa ngoại
                    throw new Exception("Cannot delete promotion because there are products referencing it. Please check and try again.");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting promotion: " + ex.Message);
                }
            }
            else
            {
                throw new Exception("Promotion does not exist!");
            }
        }
    }
}
