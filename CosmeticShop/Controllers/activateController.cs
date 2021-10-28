using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmeticShop.Data;
using CosmeticShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticShop.Controllers
{
    public class activateController : Controller
    {
        private readonly CosmeticShopDbContext db;
        public activateController(CosmeticShopDbContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Activate()
        {
            User k = HttpContext.Session.Get<User>("kh");
            if (k != null)
            {
                k.BuyPoints = 1;
                db.Update(k);
                db.SaveChangesAsync();
                HttpContext.Session.Remove("kh");
                HttpContext.Session.SetString("check", "Kích Hoạt Tài Khoản thành công");
                return RedirectToAction("login", "user");
            }
            else
            {
                ModelState.AddModelError("Lỗi", "Không tìm thấy tài khoản. Bạn cần thực hiện đăng kí tài khoản!");
                return RedirectToAction("login", "user");
            }
        }
    }
}