using CosmeticShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CosmeticShop.ViewModels
{
    public class ProductEditViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<ProductBrand> ProductBrands { get; set; }
        public IEnumerable<ProductType> ProductTypes { get; set; }
        public IFormFile Thumbnail { get; set; }
    }
}
