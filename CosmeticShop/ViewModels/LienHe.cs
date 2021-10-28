using CosmeticShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmeticShop.ViewModels
{
    public class LienHe
    {
        public IEnumerable<ProductType> ProductTypes { get; set; }
        public IEnumerable<ProductBrand> productBrands { get; set; }
    }
}
