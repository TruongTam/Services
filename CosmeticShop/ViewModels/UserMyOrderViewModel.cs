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
    public class UserMyOrderViewModel
    {
        public Order Order {get;set;}
        public IEnumerable<OrderDetail> OrderDetails {get;set;}
        public List<Product> Products {get;set;}
    }
}
