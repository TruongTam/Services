using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmeticShop.ViewModels
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int TotalPrice { get; set; }
        public string TenAnh { get; set; }
    }
}
