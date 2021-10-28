using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmeticShop.Models;

namespace CosmeticShop.ViewModels
{
    public class AdminProductBrandEditViewModel
    {
        public ProductBrand ProductBrand {get;set;}
        public IEnumerable<ProductBrand> ProductBrands { get; set; }
        public IEnumerable<Product> Products {get;set;}
    }
}