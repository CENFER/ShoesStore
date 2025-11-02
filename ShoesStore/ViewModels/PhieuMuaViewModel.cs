using Microsoft.AspNetCore.Mvc.Rendering;
using ShoesStore.Models;

namespace ShoesStore.ViewModels
{
    public class PhieuMuaViewModel
    {
        public List<ShoppingCartItem> listcartItem {  get; set; }
        public Khachhang? khInfo {  get; set; }
        public string? GhiChu {  get; set; }
        public int Mapttt {  get; set; }

         public string HoTen {  get; set; }
        public string Sdt {  get; set; }
        public string Email {  get; set; }
        public string Diachi {  get; set; }
        public int maTinh {  get; set; }
        public int maQuan {  get; set; }
        public int maPhuong {  get; set; }
        public List<Sodiachi> sodiachis {  get; set; }
        public List<Voucher> voucherList {  get; set; }
        public List<SelectListItem> selectPhuong { get; set; }
        public List<SelectListItem> selectQuan { get; set; }
        public Voucher Choosenvoucher {  get; set; }
        public decimal totalCost {  get; set; }
        public decimal tempCost {  get; set; }
        public decimal coinGet {  get; set; }
        public decimal discountMoney {  get; set; }
        public decimal coinChoosen {  get; set; }
        public decimal coinApply {  get; set; }

    }
}
