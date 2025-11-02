using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoesStore.InterfaceRepositories;
using ShoesStore.Models;
using ShoesStore.Models.Session;
using ShoesStore.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShoesStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        ISanpham sanphamrepo;
        IDongSanpham _product;
        ISize _size;
        ISanphamSize _tkho;
        IMau _mau; IKhuyenMai kmRepo;
        public ShoppingCartController(ISanpham productDetail, IDongSanpham product, ISize sz, ISanphamSize tkho, IMau mau, IKhuyenMai kmRepo)
        {
            sanphamrepo = productDetail;
            _product = product;
            _size = sz;
            _tkho = tkho;
            _mau = mau;
            this.kmRepo = kmRepo;
        }

        public IActionResult ViewCart()
        {
            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();

            return View(cartItems);
        }

        [Route("ShoppingCart/AddToCart/{id}/{tenSize}/{slton}")]
        public IActionResult AddToCart(int id, string tenSize, int slton)
        {
            Sanpham sp = sanphamrepo.Getsanpham(id);

            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();
            var existingCartItem = cartItems.FirstOrDefault(item => item.sanpham.Masp == id && item.Size == tenSize);

            if (existingCartItem != null)
            {
                if (existingCartItem.Quantity <= slton - 1)
                {
                    existingCartItem.Quantity += 1;
                }
                else
                {
                    return RedirectToAction("HienThiSanPham", "SanPham", new { madongsanpham = sp.Madongsanpham, masp = sp.Masp });
                }
            }
            else
            {
                Dongsanpham dongSanPham = _product.GetDongSanpham(sp.Madongsanpham);
                Mau mau = _mau.GetMau(sp.Mamau);

                cartItems.Add(new ShoppingCartItem()
                {
                    sanpham = sp,
                    Name = dongSanPham.Tendongsp,
                    TenMau = mau.Tenmau,
                    Quantity = 1,
                    tonkho = slton,
                    GiaGoc = dongSanPham.Giagoc,
                    PhanTramGiam = kmRepo.GetKmProductToday(dongSanPham),
                    Size = tenSize,
                    Maspsize = _tkho.GetMaspsize(sp.Masp, tenSize)
                });
            }
            HttpContext.Session.Set("Cart", cartItems);

            return RedirectToAction("ViewCart");
        }

        public IActionResult IncreaseOne(int Masp, string size)
        {
            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart");
            ShoppingCartItem shopCarteIncrease = cartItems
                .FirstOrDefault(x => x.sanpham.Masp == Masp && x.Size == size);

            if (shopCarteIncrease.tonkho >= shopCarteIncrease.Quantity + 1)
            {
                shopCarteIncrease.Quantity += 1;
            }

            HttpContext.Session.Set("Cart", cartItems);
            return PartialView("PartialCartList",cartItems);
        }


        public IActionResult DecreaseOne(int Masp, string size)
        {
            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart");
            ShoppingCartItem shopCarteDecrease = cartItems
                .FirstOrDefault(x => x.sanpham.Masp == Masp && x.Size == size);

            shopCarteDecrease.Quantity -= 1;
            if (shopCarteDecrease.Quantity == 0)
            {
                cartItems.Remove(shopCarteDecrease);
            }
            HttpContext.Session.Set("Cart", cartItems);

            return PartialView("PartialCartList", cartItems);
        }

        public IActionResult Delete(int Masp, string tenSize)
        {

            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart");
            ShoppingCartItem shopCartNeedDelete = cartItems
                .FirstOrDefault(x => x.sanpham.Masp == Masp && x.Size == tenSize);

            if (shopCartNeedDelete != null)
            {
                cartItems.Remove(shopCartNeedDelete);
            }

            HttpContext.Session.Set("Cart", cartItems);

            return PartialView("PartialCartList", cartItems);

        }



    }
}
