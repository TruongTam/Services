using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmeticShop.Models;

namespace CosmeticShop.ViewModels
{
    public class AdminProductTypesViewModel
    {
        public ProductType ProductType { get;set;}
        public IEnumerable<ProductType> ProductTypes {get;set;}
        public IEnumerable<Product> Products {get;set;}
        public IEnumerable<ProductBrand> productBrands { get; set; }
    }
}