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
using Microsoft.AspNetCore.Hosting.Server;
using CosmeticShop.Helper;

namespace CosmeticShop.Controllers
{
    public class CheckoutController : Controller
    {
        List<Product> products = new List<Product>();
        private CosmeticShopDbContext _context;
        public CheckoutController(CosmeticShopDbContext context)
        {
            _context = context;
        }
        public bool IsLogedIn()
        {
            if (HttpContext.Session.GetString("TenTaiKhoan") != null){ return true;}
            else { return false;}
        }
        public IActionResult Index()
        {
            string pay_status = HttpContext.Session.GetString("PayStatus");
            
            if (pay_status == "paid" || HttpContext.Session.GetInt32("OrderId") == null)
            {
                return RedirectToAction("Index","Home");
            }

            
            int orderid = (int)HttpContext.Session.GetInt32("OrderId");
            List<OrderDetail> orderdetails = _context.OrderDetails.Where(o => o.Order_Id == orderid).ToList();

            List<Product> products = new List<Product>();
            foreach(var detail in orderdetails)
            {
                Product pro = _context.Products.Where(o => o.Id == detail.Product_Id).FirstOrDefault();
                products.Add(pro);
            }

            CheckoutIndexViewModel vm = new CheckoutIndexViewModel()
            {
                OrderDetails =  orderdetails,
                Products     = products,
                ProductTypes = _context.ProductTypes.ToList(),
                productBrands = _context.ProductBrands.ToList()
            };

            int sum = 0;
           
            foreach (var detail in vm.OrderDetails){
                sum += detail.PriceTotal;
               
            }
            HttpContext.Session.SetString("TongTien", sum.ToString());

            PayPalConfig payPalConfig = PayPalService.GetPayPalConfig();
            ViewBag.payPalConfig = payPalConfig;
           
            return View(vm);
        }

        [HttpGet]
        public IActionResult MustSignUp(int anocartid)
        {
            CheckOutMustSignUpViewModel vm = new CheckOutMustSignUpViewModel()
            {
                User = new User(){ Role_Id = 4},
                AnoCartId = anocartid,
                ProductTypes = _context.ProductTypes.ToList(),
                productBrands = _context.ProductBrands.ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult MustSignUp(CheckOutMustSignUpViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(vm.User);
                _context.SaveChanges();

                AnoCart anocart = _context.AnoCarts.Where(ac => ac.Id == vm.AnoCartId).FirstOrDefault();
    
                Cart cart = new Cart();
                int userid = _context.Users.Where(u => u.PhoneNumber == vm.User.PhoneNumber).FirstOrDefault().Id;
                
                cart.User_Id = userid;
                cart.CartStatus_Id = anocart.CartStatus_Id;
                cart.TotalQuantity = anocart.TotalQuantity;
                cart.TotalPrice    = anocart.TotalPrice;
                cart.DateCreate    = anocart.DateCreate;
                cart.DateModify    = anocart.DateModify;

                _context.Carts.Add(cart);
                _context.SaveChanges();                
                
                int cartid = _context.Carts.Where(c => c.User_Id == userid).FirstOrDefault().Id;
                
                if (_context.AnoCartDetails.Where(ac => ac.Cart_Id == anocart.Id).FirstOrDefault() != null)
                {
                    List<AnoCartDetail> anodetails = _context.AnoCartDetails.Where(ac => ac.Cart_Id == anocart.Id).ToList();
                    foreach(AnoCartDetail detail in anodetails)
                    {
                        CartDetail cdt  = new CartDetail(){
                            Cart_Id     = cartid,
                            Product_Id  = detail.Product_Id,
                            PriceSingle = detail.PriceSingle,
                            PriceTotal  = detail.PriceTotal,
                            Quantity    = detail.Quantity,
                            DateCreate  = detail.DateCreate,
                            DateModify  = detail.DateModify
                        };
                        _context.CartDetails.Add(cdt);
                        _context.SaveChanges();
                    }
                }
                return RedirectToAction("AlreadySignUp","Checkout",new {userid = userid});
            }
            else
            {
                return View(
                    new CheckOutMustSignUpViewModel()
                    {
                        User = vm.User,
                        AnoCartId = vm.AnoCartId,
                        ProductTypes = _context.ProductTypes.ToList(),
                        productBrands = _context.ProductBrands.ToList()
                    }
                );
            }
        }

        [HttpGet]
        public IActionResult AlreadySignUp(int userid)
        {
            User user = _context.Users.Where(u => u.Id == userid).FirstOrDefault();
            Cart cart = _context.Carts.Where(c => c.User_Id == userid).FirstOrDefault();

            int orderid = 0;
            var check = _context.Orders.Where(o => o.PhoneNumber == user.PhoneNumber).FirstOrDefault();

            if (check == null)
            {
                Order order = new Order();
                order.User_Id          = user.Id;
                order.NameLast         = user.NameLast;
                order.NameMiddle       = user.NameMiddle;
                order.NameFirst        = user.NameFirst;
                order.AddressApartment = user.AddressApartment;
                order.AddressStreet    = user.AddressStreet;
                order.AddressDistrict  = user.AddressDistrict;
                order.AddressCity      = user.AddressCity;
                order.PhoneNumber      = user.PhoneNumber;
                order.OrderStatus_Id   = 1; // Trạng thái đơn hàng - Đang đợi xử lý - waiting
                order.PayStatus_Id     = 1; // Trạng thái thanh toán - Chưa thanh toán - notyet
                order.PayType_Id       = 1; // Hình thức thanh toán - Tiền mặt - cash
                order.DateCreate       = DateTime.Now;
                order.DateModify       = DateTime.Now;
                order.TotalPrice       = cart.TotalPrice;
                _context.Orders.Add(order);
                _context.SaveChanges();
                orderid = _context.Orders.Where(o => o.PhoneNumber == user.PhoneNumber).FirstOrDefault().Id;
            }
            else
            {
                var list = _context.Orders.Where(o => o.PhoneNumber == user.PhoneNumber);
                if (HttpContext.Session.GetString("PayStatus") == "paid")
                {
                    Order order = new Order();
                    order.User_Id          = user.Id;
                    order.NameLast         = user.NameLast;
                    order.NameMiddle       = user.NameMiddle;
                    order.NameFirst        = user.NameFirst;
                    order.AddressApartment = user.AddressApartment;
                    order.AddressStreet    = user.AddressStreet;
                    order.AddressDistrict  = user.AddressDistrict;
                    order.AddressCity      = user.AddressCity;
                    order.PhoneNumber      = user.PhoneNumber;
                    order.OrderStatus_Id   = 1; // Trạng thái đơn hàng - Đang đợi xử lý - waiting
                    order.PayStatus_Id     = 1; // Trạng thái thanh toán - Chưa thanh toán - notyet
                    order.PayType_Id       = 1; // Hình thức thanh toán - Tiền mặt - cash
                    order.DateCreate       = DateTime.Now;
                    order.DateModify       = DateTime.Now;
                    order.TotalPrice       = cart.TotalPrice;
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                    orderid = _context.Orders.Where(o => o.PhoneNumber == user.PhoneNumber).FirstOrDefault().Id;
                }
                else
                {
                    orderid = list.Where(o => o.OrderStatus_Id == 1).FirstOrDefault().Id;
                }
            }
            
            List<CartDetail> details = _context.CartDetails.Where(cd => cd.Cart_Id == cart.Id).ToList();
            if (details != null)
            {
                foreach(CartDetail detail in details)
                {
                    OrderDetail od = new OrderDetail();

                    od.Order_Id = orderid;
                    od.Product_Id = detail.Product_Id;
                    od.PriceSingle = detail.PriceSingle;
                    od.Quantity= detail.Quantity;
                    od.PriceTotal = detail.PriceTotal;

                    _context.OrderDetails.Add(od);
                    _context.SaveChanges();
                }
            }

          //  String content = System.IO.File.ReadAllText(Server.MapPahth("~/template/neworder.html"));
            
            ClearCart(cart.Id);
            HttpContext.Session.SetInt32("OrderId",orderid);
            HttpContext.Session.SetInt32("IdTaiKhoan",user.Id);
            HttpContext.Session.Remove("IdCart");
            HttpContext.Session.SetString("PayStatus","notyet");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Cancel(int id)
        {   
            Order delete = _context.Orders.Where(o => o.Id == id).FirstOrDefault();
            List<OrderDetail> details = _context.OrderDetails.Where(od => od.Order_Id == id).ToList();
            if (details != null)
            {
                foreach(var detail in details)
                {
                    _context.OrderDetails.Remove(detail);
                    _context.SaveChanges();
                }
            }
            _context.Orders.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction("Index","Home");
        }

        public void ClearCart(int id)
        {
            Cart cart = _context.Carts.Where(c => c.Id == id).FirstOrDefault();
            List<CartDetail> details = _context.CartDetails.Where(cd => cd.Cart_Id == id).ToList();
            if (details != null)
            {
                foreach(CartDetail detail in details)
                {
                    _context.CartDetails.Remove(detail);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
        }

        public void ClearAnoCart(int id)
        {
            AnoCart cart = _context.AnoCarts.Where(c => c.Id == id).FirstOrDefault();
            List<AnoCartDetail> details = _context.AnoCartDetails.Where(cd => cd.Cart_Id == id).ToList();
            if (details != null)
            {
                foreach(AnoCartDetail detail in details)
                {
                    _context.AnoCartDetails.Remove(detail);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
        }

        [HttpGet]
        public IActionResult PayPalPaid(int orderid)
        {
            Order order = _context.Orders.Where(o => o.Id == orderid).FirstOrDefault();
            order.PayStatus_Id = 2; // Đã thanh toán
            order.PayType_Id   = 2; // Loại thanh toán PayPal
            _context.SaveChanges();
            string email = HttpContext.Session.GetString("email");

            string content = "<br>Bạn đã đặt đơn hàng thành công. Tổng tiền :" + HttpContext.Session.GetString("TongTien");
            if (MailHelper.Send(email, email, "Đơn Hàng", content))
            {
                ViewBag.msg = "Success";
            }
            else
            {
                ViewBag.msg = "Fail";
            }
            HttpContext.Session.Remove("TongTien");
            HttpContext.Session.SetString("PayStatus","paid");
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult NganLuongPaid(int orderid)
        {
            Order order = _context.Orders.Where(o => o.Id == orderid).FirstOrDefault();
            order.PayStatus_Id = 2; // Đã thanh toán
            order.PayType_Id   = 3; // Loại thanh toán Ngân lượng
            _context.SaveChanges();

            string email = HttpContext.Session.GetString("email");

            string content = "<br>Bạn đã đặt đơn hàng thành công. Tổng tiền :" + HttpContext.Session.GetString("TongTien");
            if (MailHelper.Send(email, email, "Đơn Hàng", content))
            {
                ViewBag.msg = "Success";
            }
            else
            {
                ViewBag.msg = "Fail";
            }
            HttpContext.Session.Remove("TongTien");
            HttpContext.Session.SetString("PayStatus","paid");
            return RedirectToAction("Index","Home");
        }

        public IActionResult PayAfter()
        {
            string email = HttpContext.Session.GetString("email");

            string content = "<br>Bạn đã đặt đơn hàng thành công. Tổng tiền :" + HttpContext.Session.GetString("TongTien");
            if (MailHelper.Send(email, email, "Đơn Hàng", content))
            {
                ViewBag.msg = "Success";
            }
            else
            {
                ViewBag.msg = "Fail";
            }
            HttpContext.Session.Remove("TongTien");
            HttpContext.Session.SetString("PayStatus","paid");
            return RedirectToAction("Index","Home");
        }
    }
}