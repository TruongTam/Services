using CosmeticShop;
using CosmeticShop.Data;
using CosmeticShop.Models;
using System.Collections.Generic;
namespace CosmeticShop.ViewModels
{
    public class ListItemProductsViewModel
    {
        public string Value { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public string OrderBy { get; set; }
        public IEnumerable<ItemProductsViewModel> ItemProducts { get; set; }
        public IEnumerable<ProductType> ProductTypes { get; set; }
        public IEnumerable<ProductBrand> productBrands { get; set; }

    }
}