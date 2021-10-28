using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using CosmeticShop.Models;
using CosmeticShop.Data;
using CosmeticShop.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Rendering;
using CosmeticShop.Helper;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CosmeticShop.Controllers
{
    public class UserController : Controller
    {
        private readonly CosmeticShopDbContext _context;

        public UserController(CosmeticShopDbContext context)
        {
            _context = context;
        }

        #region Functions
        // Kiểm tra xem có đăng nhập hay chưa
        public bool IsLogedIn()
        {
            if (HttpContext.Session.GetInt32("IdTaiKhoan") != null && HttpContext.Session.GetInt32("TenTaiKhoan") != null)
            {
                return true;
            }
            return false;
        }

        // Kiểm tra email với mật khẩu
        public bool EmailLogin(string email,string password)
        {
            User login = _context.Users.Where(u => u.Email == email).FirstOrDefault();
            if (login == null){ return false;}
            else if (login.Password != password){ return false;}
            else {return true;}
        }

        // Kiểm tra số điện thoại với mật khẩu
        public bool PhoneNumberLogin(string phonenumber,string password)
        {
            User login = _context.Users.Where(u => u.PhoneNumber == phonenumber).FirstOrDefault();
            if (login == null){ return false;}
            else if (login.Password != password){ return false;}
            else {return true;}
        }

        #endregion
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Information()
        {
            return View();
        }

        public IActionResult MyOrder()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(
                new UserRegisterViewModel()
                {
                    User = new User(){
                        Role_Id  = 4 // Role_Id của user là 4
                    },
                    ProductTypes = _context.ProductTypes.ToList(),
                    productBrands = _context.ProductBrands.ToList()
                }
            );
        }

        [HttpPost]
        public IActionResult Register(UserRegisterViewModel vm)
        {
            
            if (ModelState.IsValid)
            {

                _context.Users.Add(vm.User);
                _context.SaveChanges();
                string content = "Chúc Mừng: " + vm.User.NameLast + " " + vm.User.NameMiddle + " " + vm.User.NameFirst;
                content += "<br>Đã Đăng Kí Thành Công  ";
               
                // content += "<br>Click vô đây xác nhận đăng nhập: https://cosmeticshop20.azurewebsites.net/User/Login";
                content += "Click vào <a href='https://cosmeticshop20.azurewebsites.net/activate/activate'>link</a> này để kích hoạt tài khoản.";
                const string accountSid = "ACff7a2b137de5ac0d7fe4d1396756abe9";
                const string authToken = "4d8822f20cfb83f5e8d6f3f16b18c6de";

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: vm.User.NameLast + " " + vm.User.NameMiddle + " " + vm.User.NameFirst + " Đã đăng kí tài khoản",
                    from: new Twilio.Types.PhoneNumber("+12028581210"),
                    to: new Twilio.Types.PhoneNumber("+84867601320")
                );
                //bool a = MailHelper.Send(vm.User.Email, vm.User.Email, "Chúc Mừng Bạn Đã Đăng Kí Thành Công", content);
                if (MailHelper.Send(vm.User.Email, vm.User.Email, "Chúc Mừng Bạn Đã Đăng Kí Thành Công", content))
                {
                    ViewBag.msg = "Success";
                }
                else
                {
                    ViewBag.msg = "Fail";
                }
                HttpContext.Session.Set("kh", vm.User);
                return RedirectToAction("Login");
            }
            else
            {

               
                return View(
                    new UserRegisterViewModel()
                    {
                        User = vm.User,
                        ProductTypes = _context.ProductTypes.ToList(),
                        productBrands = _context.ProductBrands.ToList()
                    }
                );
            }
        }

        public IActionResult Login()
        {
           
            return View(
                new UserLoginViewModel()
                {
                    ProductTypes = _context.ProductTypes.ToList(),
                    productBrands = _context.ProductBrands.ToList()
                }
            );
        }

        [HttpPost]
        public IActionResult Login(UserLoginViewModel vm)
        {
            ViewBag.hau = "hauahu";
            if (ModelState.IsValid)
            {
                string u = vm.Username;
                string p = vm.Password;

                if (EmailLogin(u,p) == true || PhoneNumberLogin(u,p) == true)
                {
                    HttpContext.Session.Remove("check");
                    //int check = -1;
                    User abc = _context.Users.Where(us => us.Email == u).FirstOrDefault();
                    if (abc == null ) {
                        abc =_context.Users.Where(us => us.PhoneNumber == u).FirstOrDefault();
                    }
                     if(abc.BuyPoints > 0)
                    {
                        User data = _context.Users.Where(us => us.Email == u).FirstOrDefault();
                        if (data == null) { data = _context.Users.Where(us => us.PhoneNumber == u).FirstOrDefault(); }

                        string displayname = data.NameLast;
                        if (data.NameMiddle != null) { displayname += " " + data.NameMiddle + " "; }
                        displayname += " " + data.NameFirst;

                        int id = data.Id;

                        string diachi = data.AddressStreet + " , " + data.AddressDistrict + " , " + data.AddressApartment + " , " + data.AddressCity;

                        HttpContext.Session.SetInt32("IdTaiKhoan", id);
                        HttpContext.Session.SetString("TenTaiKhoan", displayname);
                        HttpContext.Session.SetString("diachi", diachi);
                        HttpContext.Session.SetString("email", abc.Email);
                        return RedirectToAction("Index", "Home");
                    }
                     else
                    {
                        HttpContext.Session.SetString("check", " Tài khoản bạn chưa được kích hoạt, vui lòng kiểm tra mail để kích hoạt tài khoản.");
                        ViewBag.mess = " Tài khoản bạn chưa được kích hoạt, vui lòng kiểm tra mail để kích hoạt tài khoản";
                        ModelState.AddModelError("Lỗi", " Tài khoản bạn chưa được kích hoạt, vui lòng kiểm tra mail để kích hoạt tài khoản");
                        return  RedirectToAction("Login");
                    }
                }

                else
                {
                    HttpContext.Session.SetString("check", "Tên đăng nhập hoặc mật khẩu không hợp lệ.");

                    ViewBag.mess = "Tên đăng nhập hoặc mật khẩu không hợp lệ.";
                    ModelState.AddModelError("Lỗi", "Tên đăng nhập hoặc mật khẩu không hợp lệ.");
                    return RedirectToAction("Login");
                }
            }
            
            return View(
                new UserLoginViewModel()
                {
                    Username = vm.Username,
                    Password = vm.Password,
                    ProductTypes = _context.ProductTypes.ToList(),
                    productBrands = _context.ProductBrands.ToList()
                }
            );
        }

        public IActionResult Logout()
        {
            if (IsLogedIn() == true)
            {
                HttpContext.Session.Remove("check");
               HttpContext.Session.Remove("IdTaiKhoan");
                HttpContext.Session.Remove("TenTaiKhoan");
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult MyOrders(int id)
        {
            List<Order> orders = _context.Orders.Where(o => o.User_Id == id).ToList();

            UserMyOrdersViewModel vm  = new UserMyOrdersViewModel();
            List<UserMyOrderViewModel> myorderviewmodels = new List<UserMyOrderViewModel>();
            foreach (var order in orders)
            {
                List<OrderDetail> orderdetails = _context.OrderDetails.Where(od => od.Order_Id == order.Id).ToList();
                List<Product> products = new List<Product>();

                foreach (var detail in orderdetails)
                {
                    Product pro = _context.Products.Where(p => p.Id == detail.Product_Id).FirstOrDefault();
                    products.Add(pro);
                }

                UserMyOrderViewModel ovm = new UserMyOrderViewModel()
                {
                    Order = order,
                    OrderDetails = orderdetails,
                    Products = products,
                    
                };

                myorderviewmodels.Add(ovm);             
            }

            vm.MyOrderViewModels = myorderviewmodels;
            vm.ProductTypes = _context.ProductTypes.ToList();
            vm.productBrands =_context.ProductBrands.ToList();
            return View(vm);
        }

        [HttpGet]
        public IActionResult MyOrderDetail(int user_id,int order_id)
        {
            List<Order> orders = _context.Orders.Where(o => o.User_Id == user_id).ToList();

            UserMyOrderDetailViewModel vm  = new UserMyOrderDetailViewModel();
            List<UserMyOrderViewModel> myorderviewmodels = new List<UserMyOrderViewModel>();
            foreach (var order in orders)
            {
                List<OrderDetail> orderdetails = _context.OrderDetails.Where(od => od.Order_Id == order.Id).ToList();
                List<Product> products = new List<Product>();

                foreach (var detail in orderdetails)
                {
                    Product pro = _context.Products.Where(p => p.Id == detail.Product_Id).FirstOrDefault();
                    products.Add(pro);
                }

                UserMyOrderViewModel ovm = new UserMyOrderViewModel()
                {
                    Order = order,
                    OrderDetails = orderdetails,
                    Products = products
                };
                if (ovm.Order.Id == order_id){vm.MyOrderViewModel = ovm;}
                myorderviewmodels.Add(ovm);             
            }

            
            vm.MyOrderViewModels = myorderviewmodels;
            vm.ProductTypes = _context.ProductTypes.ToList();
            vm.productBrands = _context.ProductBrands.ToList();
            return View(vm);
        }
    }
}