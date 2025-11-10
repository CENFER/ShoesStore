using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Models;
using System.Collections.Generic;
using System.Linq;

namespace ShoesStore.Areas.Admin.Repositories
{
    public class VoucherAdminRepo : IVoucherAdmin
    {
        private readonly ShoesDbContext context;

        public VoucherAdminRepo(ShoesDbContext context)
        {
            this.context = context;
        }

        public List<Voucher> GetAllVouchers()
        {
            return context.Vouchers.ToList();
        }

        public Voucher GetVoucherById(string id)
        {
            return context.Vouchers.FirstOrDefault(v => v.Mavoucher == id);
        }

        public void AddVoucher(Voucher voucher)
        {
            // KIỂM TRA TRÙNG MÃ
            bool exists = context.Vouchers.Any(v => v.Mavoucher == voucher.Mavoucher);
            if (exists)
            {
                throw new System.Exception($"Mã voucher '{voucher.Mavoucher}' đã tồn tại trong hệ thống. Vui lòng chọn mã khác.");
            }

            context.Vouchers.Add(voucher);
            context.SaveChanges();
        }

        public void UpdateVoucher(Voucher voucher)
        {
            // KIỂM TRA VOUCHER CÓ TỒN TẠI KHÔNG
            var existingVoucher = context.Vouchers.FirstOrDefault(v => v.Mavoucher == voucher.Mavoucher);
            if (existingVoucher == null)
            {
                throw new Exception("Voucher không tồn tại!");
            }

            // CẬP NHẬT THÔNG TIN
            existingVoucher.Soluong = voucher.Soluong;
            existingVoucher.Giatoithieu = voucher.Giatoithieu;
            existingVoucher.Giamtoida = voucher.Giamtoida;
            existingVoucher.Ngaytao = voucher.Ngaytao;
            existingVoucher.Ngayhethan = voucher.Ngayhethan;

            context.Vouchers.Update(existingVoucher);
            context.SaveChanges();
        }

        public void DeleteVoucher(string id)
        {
            // Tìm và xử lý đơn hàng liên quan TRƯỚC KHI xóa voucher
            var relatedOrders = context.Phieumuas.Where(p => p.Mavoucher == id).ToList();

            foreach (var order in relatedOrders)
            {
                order.Mavoucher = null;
            }
            context.SaveChanges();

            // Sau đó mới xóa voucher
            var voucher = context.Vouchers.FirstOrDefault(v => v.Mavoucher == id);
            if (voucher != null)
            {
                context.Vouchers.Remove(voucher);
                context.SaveChanges();
            }
        }
    }
}