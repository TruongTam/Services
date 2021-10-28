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
    public class UserRegisterViewModel
    {
        public User User { get;set;}

        // Dành cho thanh menu 
        public IEnumerable<ProductType> ProductTypes {get;set;}
        public IEnumerable<ProductBrand> productBrands { get; set; }
    }
}
