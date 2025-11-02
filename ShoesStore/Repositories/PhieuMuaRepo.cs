using Microsoft.EntityFrameworkCore;
using ShoesStore.InterfaceRepositories;
using ShoesStore.Models;
using ShoesStore.ViewModels;

namespace ShoesStore.Repositories
{
    public class PhieuMuaRepo : IPhieuMua
    {
        ShoesDbContext context;
        public PhieuMuaRepo(ShoesDbContext context)
        {
            this.context = context;
        }

        public int AddPhieuMua(PhieuMuaViewModel phieuMua)
        {
            //decimal tongtien = 0;
            //foreach (ShoppingCartItem cartItem in phieuMua.listcartItem)
            //{
            //    if(cartItem.PhanTramGiam > 1)
            //    {
            //        tongtien += cartItem.Quantity * (cartItem.GiaGoc - cartItem.GiaGoc * cartItem.PhanTramGiam / 100);
            //    }
            //    else
            //    {
            //        tongtien += cartItem.Quantity * cartItem.GiaGoc;
            //    }
            //}
            string tentinh = context.Tinhs.FirstOrDefault(x => x.Matinh == phieuMua.maTinh).Tentinh;
            string tenquan = context.Quans.FirstOrDefault(x => x.Maquan == phieuMua.maQuan).Tenquan; 
            string tenphuong = context.Phuongs.FirstOrDefault(x => x.Maphuong == phieuMua.maPhuong).Tenphuong;

            if (phieuMua.khInfo != null)
            {
                Khachhang kh = context.Khachhangs.FirstOrDefault(x => x.Makh == phieuMua.khInfo.Makh);
                kh.Tongxu -= phieuMua.coinApply;
                context.Khachhangs.Update(kh);
                context.SaveChanges();
            }

            if (phieuMua.Choosenvoucher?.Mavoucher != null)
            {
                Voucher vc = context.Vouchers.FirstOrDefault(x => x.Mavoucher == phieuMua.Choosenvoucher.Mavoucher);
                vc.Soluong -= 1;
                context.Vouchers.Update(vc);
                context.SaveChanges();
            }

            string Diachi = phieuMua.Diachi + ", "+tentinh +", "+tenquan+", "+tenphuong;
            Phieumua newpm = new Phieumua()
            {
                Ghichu = phieuMua.GhiChu,
                Makh = phieuMua.khInfo?.Makh,
                Ngaydat = DateTime.Now,
                Tinhtrang = "Pending",
                Mapttt = phieuMua.Mapttt,
                MaptttNavigation = context.Phuongthucthanhtoans.Find(phieuMua.Mapttt),
                Tongtien = phieuMua.totalCost,
                Tennguoinhan = phieuMua.HoTen,
                Sdtnguoinhan = phieuMua.Sdt,
                Emailnguoinhan = phieuMua.Email,
                Diachinguoinhan = Diachi,
                Mavoucher = phieuMua.Choosenvoucher?.Mavoucher
            };
            
            context.Phieumuas.Add(newpm);
            context.SaveChanges();

            List<Chitietphieumua> ctpmList = new List<Chitietphieumua>();
            foreach (ShoppingCartItem cartItem in phieuMua.listcartItem)
            {
                ctpmList.Add(new Chitietphieumua()
                {
                    Mapm = newpm.Mapm,
                    Maspsize = cartItem.Maspsize,
                    Soluong = cartItem.Quantity,
                    Dongia = cartItem.PhanTramGiam > 1 ? (cartItem.GiaGoc - cartItem.GiaGoc * cartItem.PhanTramGiam / 100) : cartItem.GiaGoc,
                });
            }

            context.Chitietphieumuas.AddRange(ctpmList);
            context.SaveChanges();
            return newpm.Mapm;
        }

        public List<Phieumua> GetOrderHistoryByEmail(string email)
        {
            var khachhang = context.Khachhangs.FirstOrDefault(kh => kh.Email == email);
            if (khachhang == null) return new List<Phieumua>();
            return context.Phieumuas.Where(pm => pm.Makh == khachhang.Makh)
                .Include(x=>x.MavoucherNavigation)
                .Include(p => p.Chitietphieumuas)
                .ThenInclude(c => c.MaspsizeNavigation)
                .ThenInclude(s => s.MaspNavigation)
                .ThenInclude(s => s.MadongsanphamNavigation)
                .Include(p => p.Chitietphieumuas)
                .ThenInclude(c => c.MaspsizeNavigation)
                .ThenInclude(s => s.MasizeNavigation)
                .Include(p => p.Chitietphieumuas)
                .ThenInclude(c => c.MaspsizeNavigation)
                .ThenInclude(s => s.MaspNavigation)
                .ThenInclude(s => s.MamauNavigation).OrderByDescending(x=>x.Mapm).ToList();
        }
        public Phieumua GetOrderById(int id)
        {
            return context.Phieumuas
                .Include(p => p.Chitietphieumuas)
                .ThenInclude(c => c.MaspsizeNavigation)
                .ThenInclude(s => s.MaspNavigation)
                .ThenInclude(s => s.MadongsanphamNavigation)
                .Include(p => p.Chitietphieumuas)
                .ThenInclude(c => c.MaspsizeNavigation)
                .ThenInclude(s => s.MasizeNavigation)
                .Include(p => p.Chitietphieumuas)
                .ThenInclude(c => c.MaspsizeNavigation)
                .ThenInclude(s => s.MaspNavigation)
                .ThenInclude(s => s.MamauNavigation)
                .FirstOrDefault(p => p.Mapm == id);

        }
    }
}
