using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CosmeticShop.Data;
using CosmeticShop.Models;
using CosmeticShop.ModelsPayPal;

namespace CosmeticShop.Controllers
{
    public class NganLuongController : Controller
    {
        private readonly CosmeticShopDbContext db;
        public NganLuongController(CosmeticShopDbContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ExecutePayment(string buyer_fullname, string buyer_email,
            string buyer_mobile, string option_payment, string bankcode)
        {
            string payment_method = option_payment;
            string str_bankcode = bankcode;


            RequestInfo info = new RequestInfo();
            info.Merchant_id = "48305";
            info.Merchant_password = "1d56734bff914cd62f5e675782b92176";
            info.Receiver_email = "hauxuan94@gmail.com";

            //mk nl ac1d19ab3a0450d7b6d0c3b885448a86
            // secrrt d0c398afaa9c40c3abffc82127a38354 
            //api b6dc94eff0e64a3cafbe6927921bfe03

            info.cur_code = "vnd";
            info.bank_code = str_bankcode;


            //int id_new = db.HoaDon.Max(m => m.MaHd) + 1;
            DateTime currentDate = DateTime.Now;
            // Voucher voucher = db.Voucher.SingleOrDefault(s => s.NgayBatDau <= currentDate && s.NgayHetHan >= currentDate);
            double giamGia = 0;
            // if (voucher != null)
            // {
            //    giamGia = (double)voucher.GiamGia;
            //}
            double totalAmount = 0;

            totalAmount = Double.Parse(HttpContext.Session.GetString("TongTien"));


            info.Order_code = "1";//(id_new + 1).ToString();
            info.Total_amount = totalAmount.ToString();
            info.fee_shipping = "0";
            info.Discount_amount = giamGia.ToString();
            info.order_description = "Thanh toan test thu dong hang";
            info.return_url = "https://cosmeticshop20.azurewebsites.net/Checkout/NganLuongPaid/?orderid=" + HttpContext.Session.GetInt32("OrderId").ToString();
            info.cancel_url = "https://cosmeticshop20.azurewebsites.net/";

            info.Buyer_fullname = buyer_fullname;
            info.Buyer_email = buyer_email;
            info.Buyer_mobile = buyer_mobile;

            APICheckoutV3 objNLChecout = new APICheckoutV3();
            ResponseInfo result = objNLChecout.GetUrlCheckout(info, payment_method);

            if (result.Error_code == "00")
            {

                //SaveOrder(totalAmount, "nganluong");
                return Redirect(result.Checkout_url);
                //Response.Redirect(result.Checkout_url);
            }
            else
                return View();
            // txtserverkt.InnerHtml = result.Description;

        }
        public String CreateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        /*
        public IActionResult UpdateNganLuongState()
        {
            int id_Max = db.HoaDonThanhToan.Max(m => m.MaHtttHd);
            var result = db.HoaDonThanhToan.SingleOrDefault(s => s.MaHtttHd == id_Max);
            if (result == null)
            {
                return Content("Thanh toán thất bại !");
            }
            result.TrangThaiTt = true;
            db.SaveChanges();
            return RedirectToAction("Index", "GioHang");
        }
        
        public List<CartItem> Carts
        {
            get
            {
                List<CartItem> myCart = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (myCart == default(List<CartItem>))
                {
                    myCart = new List<CartItem>();
                }

                return myCart;
            }
        }
        
        public IActionResult SaveOrder(double totalCount, string type)
        {
            try
            {
                KhachHang kh = HttpContext.Session.Get<KhachHang>("user");
                if (kh != null && type == "nganluong")
                {

                    DateTime currentDate = DateTime.Now;
                    Voucher voucher = db.Voucher.SingleOrDefault(s => s.NgayBatDau <= currentDate && s.NgayHetHan >= currentDate);
                    double giamGia = 0;
                    if (voucher != null)
                    {
                        giamGia = (double)voucher.GiamGia;
                    }


                    HoaDon hd = new HoaDon();
                    hd.MaKh = kh.MaKh;
                    hd.NgayDat = DateTime.Now;
                    hd.NgayGiao = DateTime.Now;
                    hd.HoTen = kh.HoTen;
                    hd.DiaChi = kh.DiaChi;
                    hd.SdtNguoinhan = kh.DienThoai;
                    hd.MaTrangThai = 1;
                    hd.PhiVanChuyen = 35000;
                    hd.TongTienHang = (1 - giamGia) * totalCount;
                    db.HoaDon.Add(hd);
                    db.SaveChanges();

                    int id_HD = db.HoaDon.Max(m => m.MaHd);
                    HoaDonThanhToan hdtt = new HoaDonThanhToan();
                    hdtt.MaHd = id_HD;
                    hdtt.MaHttt = "nganluong";
                    hdtt.TrangThaiTt = true;
                    db.HoaDonThanhToan.Add(hdtt);
                    db.SaveChanges();

                    foreach (var item in Carts)
                    {
                        ChiTietHd cthd = new ChiTietHd();
                        cthd.MaHd = item.MaHh;
                        cthd.MaHh = item.MaHh;
                        cthd.DonGia = item.GiaBan;

                        cthd.GiamGia = giamGia;
                        cthd.SoLuong = item.SoLuong;
                        cthd.KichCo = item.KichCo;
                        db.ChiTietHd.Add(cthd);
                        db.SaveChanges();
                        HttpContext.Session.Remove("GioHang");

                    }
                    return new JsonResult(true);
                }

            }
            catch
            {
            }
            return new JsonResult(false);
        }
        */
    }
}