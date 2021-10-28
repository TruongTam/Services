using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
namespace CosmeticShop.Controllers
{
    public class PaypalController : Controller
    {
        IConfiguration _iconfiguration;
        public PaypalController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
        }
        public ActionResult PaypalView(string item, string sum)
        {
            //sương
            CosmeticShop.Models.PaypalModel paypal = new Models.PaypalModel();
            paypal.cmd = "_xclick";
            paypal.business = _iconfiguration.GetSection("appSettings").GetSection("BusinessAccountKey").Value;

            bool useSandbox = Convert.ToBoolean(_iconfiguration.GetSection("appSettings").GetSection("UseSandbox").Value);
            if (useSandbox)
            {
                ViewBag.actionURL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            else
            {
                ViewBag.actionURL = "https://www.paypal.com/cgi-bin/webscr";
            }

            paypal.cancel_return = System.Configuration.ConfigurationManager.AppSettings["CancelURL"];
            paypal.@return = ConfigurationManager.AppSettings["ReturnURL"];
            paypal.notify_url = ConfigurationManager.AppSettings["NotifyURL"];
            paypal.currency_code = ConfigurationManager.AppSettings["CurrencyCode"];

            paypal.item_name = item;
            paypal.amount = sum;
            return View(paypal);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}