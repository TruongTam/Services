using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmeticShop.Models;

namespace CosmeticShop.ViewModels
{
    public class CheckoutIndexViewModel
    {
        public List<OrderDetail> OrderDetails {get;set;}
        public List<Product> Products{get;set;}
        public IEnumerable<ProductType> ProductTypes {get;set;}
        public IEnumerable<ProductBrand> productBrands { get; set; }
    }
}