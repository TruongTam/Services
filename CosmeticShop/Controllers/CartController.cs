using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CosmeticShop.Models;
using CosmeticShop.ViewModels;
using CosmeticShop.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CosmeticShop.ModelsPayPal;

namespace CosmeticShop.Controllers
{
    public class CartController : Controller
    {
        private CosmeticShopDbContext _context;
        public CartController(CosmeticShopDbContext context)
        {
            _context = context;
        }
        
        [Route("/giohang")]
        public async Task<IActionResult> Index()
        {
            List<CartItemViewModel> cartItem = new List<CartItemViewModel>();
            double sum = 0;
            if (HttpContext.Session.GetString("IdTaiKhoan") == null)
            {
                ViewBag.Tam = 1;
                if (HttpContext.Session.GetInt32("IdCart") != null)
                {
                    cartItem = await (from d in _context.AnoCartDetails
                                      join c in _context.AnoCarts
                                      on d.Cart_Id equals c.Id
                                      join p in _context.Products
                                      on d.Product_Id equals p.Id
                                      where c.Id == HttpContext.Session.GetInt32("IdCart")
                                      select new CartItemViewModel
                                      {
                                          Id = d.Id,
                                          Name = p.Name,
                                          Quantity = d.Quantity,
                                          Price = d.PriceSingle,
                                          TotalPrice = d.PriceTotal,
                                          TenAnh = p.Thumbnail
                                       
                                      }).ToListAsync();
                }
                foreach (var item in cartItem)
                {
                    sum += item.TotalPrice;
                }
            }
            else
            {
                cartItem = await (from d in _context.CartDetails
                                    join c in _context.Carts
                                    on d.Cart_Id equals c.Id
                                    join p in _context.Products
                                    on d.Product_Id equals p.Id
                                    where c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan")
                                    select new CartItemViewModel
                                    {
                                        Id = d.Id,
                                        Name = p.Name,
                                        Quantity = d.Quantity,
                                        Price = d.PriceSingle,
                                        TotalPrice = d.PriceTotal,
                                        TenAnh = p.Thumbnail
                                    }).ToListAsync();
                foreach (var item in cartItem)
                {
                    sum += item.TotalPrice;
                }
            }

            CartViewModel vm = new CartViewModel()
            {
                ProductTypes = _context.ProductTypes.ToList(),
                productBrands = _context.ProductBrands.ToList(),
                CartItemViewModels = cartItem
            };


            


            HttpContext.Session.SetString("TongTien", sum.ToString());
            string Ten= HttpContext.Session.GetString("TenTaiKhoan");
            string DiaChi = HttpContext.Session.GetString("diachi");
            ViewBag.ten = Ten;
            ViewBag.diachi = DiaChi;
            PayPalConfig payPalConfig = PayPalService.GetPayPalConfig();
            ViewBag.payPalConfig = payPalConfig;




            return View(vm);
        }
        [Route("Success")]
        public IActionResult Success()
        {
            var result = PTDHolder.Success(Request.Query["tx"].ToString());
            return View("Success");
        }

        // nhận từ ajax?
        public async Task<int> AddItem(int id)
        {
            AnoCartDetail anoCartDetail;
            AnoCart anoCart;

            Cart cart;
            CartDetail cartDetail;
            Product product = await (from p in _context.Products
                               where p.Id == id
                               select p).FirstOrDefaultAsync();

            if (product == null) return -1;

            //tai khoan chua dang nhap
            if (HttpContext.Session.GetInt32("IdTaiKhoan") == null)
            {
                //khởi tạo cart lần đầu
                if (HttpContext.Session.GetInt32("IdCart") == null)
                {
                    anoCart = new AnoCart
                    {
                        CartStatus_Id = 2,
                        TotalPrice = product.Price - (int)(product.Price * product.Saleoff) / 100,
                        TotalQuantity = 1,
                        DateCreate = DateTime.Now,
                        DateModify = DateTime.Now
                    };

                    //thêm cart vào database
                    _context.AnoCarts.Add(anoCart);
                    _context.SaveChanges();

                    //lấy và set id cart vừa tạo vào session
                    int idCart = await(from a in _context.AnoCarts select a.Id).MaxAsync();
                    HttpContext.Session.SetInt32("IdCart", idCart);

                    //tạo mới anoCartDetail
                    anoCartDetail = new AnoCartDetail
                    {
                        Cart_Id = HttpContext.Session.GetInt32("IdCart").Value,
                        Product_Id = id,
                        Quantity = 1,
                        PriceTotal = product.Price - (int)(product.Price * product.Saleoff) / 100,
                        PriceSingle =product.Price - (int)(product.Price * product.Saleoff) / 100,
                        DateCreate = DateTime.Now,
                        DateModify = DateTime.Now
                    };
                    _context.AnoCartDetails.Add(anoCartDetail);
                    _context.SaveChanges();

                    return 1;
                }

                //thêm sản phẩm theo id khi cart đã được tạo
                else
                {
                    //lấy anoCartDetail theo id sản phẩm và id cart
                    anoCartDetail = await (from d in _context.AnoCartDetails
                                                             where d.Product_Id == id & d.Cart_Id == HttpContext.Session.GetInt32("IdCart")
                                                             select d).SingleOrDefaultAsync();

                    //nếu chưa có anoCartDetail theo sản phẩm và cart
                    //tạo mới
                    if (anoCartDetail == null)
                    {
                        anoCartDetail = new AnoCartDetail
                        {
                            Cart_Id = HttpContext.Session.GetInt32("IdCart").Value,
                            Product_Id = product.Id,
                            Quantity = 1,
                            PriceTotal = product.Price - (int)(product.Price * product.Saleoff) / 100,
                            PriceSingle = product.Price - (int)(product.Price * product.Saleoff) / 100,
                            DateCreate = DateTime.Now,
                            DateModify = DateTime.Now
                        };

                        //thêm anoCartDrtail vào database
                        _context.AnoCartDetails.Add(anoCartDetail);
                        _context.SaveChanges();

                        //cập nhật anoCart
                        anoCart = await (from c in _context.AnoCarts where c.Id == HttpContext.Session.GetInt32("IdCart") select c).SingleOrDefaultAsync();
                        anoCart.TotalPrice += (product.Price - (int)(product.Price * product.Saleoff) / 100);
                        anoCart.TotalQuantity += 1;
                        _context.AnoCarts.Update(anoCart);
                        _context.SaveChanges();

                        return 1;
                    }
                    //đã có anoCartDetail tang so luong, gia tien (anoCart, anoCartDetails)
                    else
                    {
                        //Cap nhat anoCartDetails
                        anoCartDetail = await (from d in _context.AnoCartDetails
                                                                 where d.Product_Id == id & d.Cart_Id == HttpContext.Session.GetInt32("IdCart")
                                                                 select d).SingleOrDefaultAsync();
                        anoCartDetail.Quantity += 1;
                        anoCartDetail.PriceTotal = anoCartDetail.Quantity * anoCartDetail.PriceSingle;
                        
                        //cap nhat anoCart
                        anoCart = await (from c in _context.AnoCarts where c.Id == HttpContext.Session.GetInt32("IdCart") select c).SingleOrDefaultAsync();
                        anoCart.TotalPrice += (product.Price - (int)(product.Price * product.Saleoff) / 100);
                        anoCart.TotalQuantity += 1;
                        _context.AnoCarts.Update(anoCart);
                        _context.SaveChanges();

                        return 1;
                    }
                }
            }
            
            //tai khoan da dang nhap
            else
            {
                cart = await (from c in _context.Carts
                            where c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan")
                            select c).SingleOrDefaultAsync();
                    
                //tai khoan nay chua co cart , thi tao moi
                if (cart == null)
                {
                    //them moi cart
                    cart = new Cart
                    {
                        CartStatus_Id = 2,
                        User_Id = HttpContext.Session.GetInt32("IdTaiKhoan").Value,
                        TotalPrice = product.Price - (int)(product.Price * product.Saleoff) / 100,
                        TotalQuantity = 1,
                        DateCreate = DateTime.Now,
                        DateModify = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                    _context.SaveChanges();

                    //them moi cartDetail
                    cartDetail = new CartDetail
                    {
                        Cart_Id = cart.Id,
                        Product_Id = id,
                        Quantity = 1,
                        PriceTotal = product.Price - (int)(product.Price * product.Saleoff) / 100,
                        PriceSingle = product.Price - (int)(product.Price * product.Saleoff) / 100,
                        DateCreate = DateTime.Now,
                        DateModify = DateTime.Now
                    };
                    _context.CartDetails.Add(cartDetail);
                    _context.SaveChanges();

                    return 1;
                }
                //tai khoan nay da tao cart
                else if (cart != null)
                {
                    //lấy cartDetail theo id sản phẩm và id cart
                    cartDetail = await (from d in _context.CartDetails
                                        where d.Product_Id == id & d.Cart_Id == cart.Id
                                        select d).SingleOrDefaultAsync();

                    //Chua co cartDetail them moi
                    if (cartDetail == null)
                    {
                        cartDetail = new CartDetail
                        {
                            Cart_Id = cart.Id,
                            Product_Id = product.Id,
                            Quantity = 1,
                            PriceTotal = product.Price - (int)(product.Price * product.Saleoff) / 100,
                            PriceSingle = product.Price - (int)(product.Price * product.Saleoff) / 100,
                            DateCreate = DateTime.Now,
                            DateModify = DateTime.Now
                        };

                        //thêm cartDrtail vào database
                        _context.CartDetails.Add(cartDetail);
                        _context.SaveChanges();

                        //cập nhật Cart
                        cart = await (from c in _context.Carts where c.Id == cart.Id select c).SingleOrDefaultAsync();
                        cart.TotalPrice += (product.Price - (int)(product.Price * product.Saleoff) / 100);
                        cart.TotalQuantity += 1;
                        _context.Carts.Update(cart);
                        _context.SaveChanges();

                        return 1;
                    }

                    //đã có cartDetail tang so luong, gia tien (cart, cartDetails) 
                    else 
                    {
                        //Cap nhat CartDetails
                        cartDetail = await (from d in _context.CartDetails
                                            where d.Product_Id == id & d.Cart_Id == cart.Id
                                            select d).SingleOrDefaultAsync();
                        cartDetail.Quantity += 1;
                        cartDetail.PriceTotal = cartDetail.Quantity * cartDetail.PriceSingle;

                        //cap nhat Cart
                        cart = await (from c in _context.Carts where c.Id == cart.Id select c).SingleOrDefaultAsync();
                        cart.TotalPrice += (product.Price - (int)(product.Price * product.Saleoff) / 100);
                        cart.TotalQuantity += 1;
                        _context.Carts.Update(cart);
                        _context.SaveChanges();

                        return 1;
                    }
                }
            }
            return 0;
        }
        public async Task<IActionResult> SingleAddToCart(int id)
        {
            await AddItem(id);
            return RedirectToAction("Index","Cart");
        }
        public async Task<IActionResult> PlusItem(int id)
        {
            List<CartItemViewModel> cartItem = new List<CartItemViewModel>();
            //đã đăng nhập
            if (HttpContext.Session.GetInt32("IdTaiKhoan").HasValue)
            {
                CartDetail cartDetail = await (from c in _context.Carts
                                               join cd in _context.CartDetails
                                               on c.Id equals cd.Cart_Id
                                               where cd.Id == id & c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan").Value
                                               select cd).SingleOrDefaultAsync();
                if (cartDetail != null)
                {
                    if (cartDetail.Quantity >= 1)
                    {
                        cartDetail.Quantity += 1;
                        cartDetail.PriceTotal += cartDetail.PriceSingle;
                        _context.CartDetails.Update(cartDetail);
                        _context.SaveChanges();

                        Cart cart = await(from c in _context.Carts 
                                    where c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan").Value
                                    select c).SingleOrDefaultAsync();
                        cart.TotalQuantity += 1;
                        cart.TotalPrice += cartDetail.PriceSingle;
                        _context.Carts.Update(cart);
                        _context.SaveChanges();
                    }
                    cartItem = await (from d in _context.CartDetails
                                      join c in _context.Carts
                                      on d.Cart_Id equals c.Id
                                      join p in _context.Products
                                      on d.Product_Id equals p.Id
                                      where c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan") & d.Quantity > 0
                                      select new CartItemViewModel
                                      {
                                          Id = d.Id,
                                          Name = p.Name,
                                          Quantity = d.Quantity,
                                          Price = d.PriceSingle,
                                          TotalPrice = d.PriceTotal,
                                          TenAnh=p.Thumbnail
                                      }).ToListAsync();
                    return PartialView("Users/_CartItemPartial", cartItem);
                }
                return PartialView("Users/_CartItemPartial", cartItem);
            }
            //chưa đăng nhập
            else
            {
                AnoCartDetail anoCartDetail = await (from c in _context.AnoCarts
                                               join cd in _context.AnoCartDetails
                                               on c.Id equals cd.Cart_Id
                                               where cd.Id == id & c.Id == HttpContext.Session.GetInt32("IdCart").Value
                                               select cd).SingleOrDefaultAsync();
                if (anoCartDetail != null)
                {
                    if (anoCartDetail.Quantity >= 1)
                    {
                        anoCartDetail.Quantity += 1;
                        anoCartDetail.PriceTotal += anoCartDetail.PriceSingle;
                        _context.AnoCartDetails.Update(anoCartDetail);
                        _context.SaveChanges();

                        AnoCart anoCart = await (from c in _context.AnoCarts
                                           where c.Id == HttpContext.Session.GetInt32("IdCart").Value
                                           select c).SingleOrDefaultAsync();
                        anoCart.TotalQuantity += 1;
                        anoCart.TotalPrice += anoCartDetail.PriceSingle;
                        _context.AnoCarts.Update(anoCart);
                        _context.SaveChanges();
                    }
                    cartItem = await (from d in _context.AnoCartDetails
                                        join c in _context.AnoCarts
                                        on d.Cart_Id equals c.Id
                                        join p in _context.Products
                                        on d.Product_Id equals p.Id
                                        where c.Id == HttpContext.Session.GetInt32("IdCart") & d.Quantity > 0
                                      select new CartItemViewModel
                                                              {
                                                                  Id = d.Id,
                                                                  Name = p.Name,
                                                                  Quantity = d.Quantity,
                                                                  Price = d.PriceSingle,
                                                                  TotalPrice = d.PriceTotal,
                                                                  TenAnh=p.Thumbnail
                                                                  
                                                              }).ToListAsync();
                    return PartialView("Users/_CartItemPartial", cartItem);
                }
                return PartialView("Users/_CartItemPartial", cartItem);
            }
        }

        public async Task<IActionResult> MinusItem(int id)
        {
            List<CartItemViewModel> cartItem = new List<CartItemViewModel>();
            //đã đăng nhập
            if (HttpContext.Session.GetInt32("IdTaiKhoan").HasValue)
            {
                CartDetail cartDetail = await (from c in _context.Carts
                                               join cd in _context.CartDetails
                                               on c.Id equals cd.Cart_Id
                                               where cd.Id == id & c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan").Value
                                               select cd).SingleOrDefaultAsync();
                if (cartDetail != null)
                {
                    if (cartDetail.Quantity >= 1)
                    {
                        if(cartDetail.Quantity == 1)
                        {
                            _context.CartDetails.Remove(cartDetail);
                            _context.SaveChanges();
                        }
                        else
                        {
                            cartDetail.Quantity -= 1;
                            cartDetail.PriceTotal -= cartDetail.PriceSingle;
                            _context.CartDetails.Update(cartDetail);
                            _context.SaveChanges();
                        }

                        Cart cart = await (from c in _context.Carts
                                           where c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan").Value
                                           select c).SingleOrDefaultAsync();
                        cart.TotalQuantity -= 1;
                        cart.TotalPrice -= cartDetail.PriceSingle;
                        _context.Carts.Update(cart);
                        _context.SaveChanges();
                    }
                    cartItem = await (from d in _context.CartDetails
                                      join c in _context.Carts
                                      on d.Cart_Id equals c.Id
                                      join p in _context.Products
                                      on d.Product_Id equals p.Id
                                      where c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan") & d.Quantity > 0
                                      select new CartItemViewModel
                                      {
                                          Id = d.Id,
                                          Name = p.Name,
                                          Quantity = d.Quantity,
                                          Price = d.PriceSingle,
                                          TotalPrice = d.PriceTotal,
                                          TenAnh=p.Thumbnail
                                      }).ToListAsync();
                    return PartialView("Users/_CartItemPartial", cartItem);
                }
                return PartialView("Users/_CartItemPartial", cartItem);
            }
            //chưa đăng nhập
            else
            {
                AnoCartDetail anoCartDetail = await (from c in _context.AnoCarts
                                                     join cd in _context.AnoCartDetails
                                                     on c.Id equals cd.Cart_Id
                                                     where cd.Id == id & c.Id == HttpContext.Session.GetInt32("IdCart").Value
                                                     select cd).SingleOrDefaultAsync();
                if (anoCartDetail != null)
                {
                    if (anoCartDetail.Quantity >= 1)
                    {
                        if (anoCartDetail.Quantity == 1)
                        {
                            _context.AnoCartDetails.Remove(anoCartDetail);
                            _context.SaveChanges();
                        }
                        else
                        {
                            anoCartDetail.Quantity -= 1;
                            anoCartDetail.PriceTotal -= anoCartDetail.PriceSingle;
                            _context.AnoCartDetails.Update(anoCartDetail);
                            _context.SaveChanges();
                        }

                        AnoCart anoCart = await (from c in _context.AnoCarts
                                                 where c.Id == HttpContext.Session.GetInt32("IdCart").Value
                                                 select c).SingleOrDefaultAsync();
                        anoCart.TotalQuantity -= 1;
                        anoCart.TotalPrice -= anoCartDetail.PriceSingle;
                        _context.AnoCarts.Update(anoCart);
                        _context.SaveChanges();
                    }
                    cartItem = await (from d in _context.AnoCartDetails
                                      join c in _context.AnoCarts
                                      on d.Cart_Id equals c.Id
                                      join p in _context.Products
                                      on d.Product_Id equals p.Id
                                      where c.Id == HttpContext.Session.GetInt32("IdCart") & d.Quantity > 0
                                      select new CartItemViewModel
                                      {
                                          Id = d.Id,
                                          Name = p.Name,
                                          Quantity = d.Quantity,
                                          Price = d.PriceSingle,
                                          TotalPrice = d.PriceTotal,
                                          TenAnh=p.Thumbnail
                                      }).ToListAsync();
                    return PartialView("Users/_CartItemPartial", cartItem);
                }
                return PartialView("Users/_CartItemPartial", cartItem);
            }
        }
    
        public async Task<IActionResult> DeleteItem(int id)
        {
            List<CartItemViewModel> cartItem = new List<CartItemViewModel>();
            //đã đăng nhập
            if (HttpContext.Session.GetInt32("IdTaiKhoan").HasValue)
            {
                CartDetail cartDetail = await (from c in _context.Carts
                                               join cd in _context.CartDetails
                                               on c.Id equals cd.Cart_Id
                                               where cd.Id == id & c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan").Value
                                               select cd).SingleOrDefaultAsync();
                if (cartDetail != null)
                {
                    if (cartDetail.Quantity >= 1)
                    {
                        Cart cart = await (from c in _context.Carts
                                           where c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan").Value
                                           select c).SingleOrDefaultAsync();
                        cart.TotalQuantity -= cartDetail.Quantity;
                        cart.TotalPrice -= cartDetail.PriceTotal;
                        _context.Carts.Update(cart);
                        _context.SaveChanges();

                        _context.CartDetails.Remove(cartDetail);
                        _context.SaveChanges();
                    }
                    cartItem = await (from d in _context.CartDetails
                                      join c in _context.Carts
                                      on d.Cart_Id equals c.Id
                                      join p in _context.Products
                                      on d.Product_Id equals p.Id
                                      where c.User_Id == HttpContext.Session.GetInt32("IdTaiKhoan") & d.Quantity > 0
                                      select new CartItemViewModel
                                      {
                                          Id = d.Id,
                                          Name = p.Name,
                                          Quantity = d.Quantity,
                                          Price = d.PriceSingle,
                                          TotalPrice = d.PriceTotal,
                                          TenAnh= p.Thumbnail
                                            
                                      }).ToListAsync();
                    return PartialView("Users/_CartItemPartial", cartItem);
                }
                return PartialView("Users/_CartItemPartial", cartItem);
            }
            //chưa đăng nhập
            else
            {
                AnoCartDetail anoCartDetail = await (from c in _context.AnoCarts
                                                     join cd in _context.AnoCartDetails
                                                     on c.Id equals cd.Cart_Id
                                                     where cd.Id == id & c.Id == HttpContext.Session.GetInt32("IdCart").Value
                                                     select cd).SingleOrDefaultAsync();
                if (anoCartDetail != null)
                {
                    if (anoCartDetail.Quantity >= 1)
                    {                       
                        AnoCart anoCart = await (from c in _context.AnoCarts
                                                 where c.Id == HttpContext.Session.GetInt32("IdCart").Value
                                                 select c).SingleOrDefaultAsync();
                        anoCart.TotalQuantity -= anoCartDetail.Quantity;
                        anoCart.TotalPrice -= anoCartDetail.PriceTotal;
                        _context.AnoCarts.Update(anoCart);
                        _context.SaveChanges();

                        _context.AnoCartDetails.Remove(anoCartDetail);
                        _context.SaveChanges();

                    }
                    cartItem = await (from d in _context.AnoCartDetails
                                      join c in _context.AnoCarts
                                      on d.Cart_Id equals c.Id
                                      join p in _context.Products
                                      on d.Product_Id equals p.Id
                                      where c.Id == HttpContext.Session.GetInt32("IdCart") & d.Quantity > 0
                                      select new CartItemViewModel
                                      {
                                          Id = d.Id,
                                          Name = p.Name,
                                          Quantity = d.Quantity,
                                          Price = d.PriceSingle,
                                          TotalPrice = d.PriceTotal,
                                          TenAnh=p.Thumbnail
                                      }).ToListAsync();
                    return PartialView("Users/_CartItemPartial", cartItem);
                }
                return PartialView("Users/_CartItemPartial", cartItem);
            }
        }
    }
}