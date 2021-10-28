using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmeticShop.Models;

namespace CosmeticShop.ViewModels
{
    public class AdminOrdersOverviewViewModel
    {
        public IEnumerable<Order> Orders {get;set;}
    }
}