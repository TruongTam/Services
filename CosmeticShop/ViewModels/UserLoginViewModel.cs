using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CosmeticShop.Data;
using CosmeticShop.Models;

namespace CosmeticShop.ViewModels
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Thông tin đăng nhập không được để trống")]
        [StringLength(255, ErrorMessage = "Thông tin đăng nhập từ 6 đến 255 kí tự",MinimumLength = 6)]
        [Display(Name = "Email hoặc số điện thoại")]
        public string Username {get;set;}

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(25, ErrorMessage = "Mật khẩu từ 6 đến 25 kí tự",MinimumLength = 6)]
        [Display(Name = "Mật khẩu")]
        public string Password {get;set;}

        // Dành cho thanh menu 
        public IEnumerable<ProductType> ProductTypes {get;set;}
        public IEnumerable<ProductBrand> productBrands { get; set; }
    }
}
