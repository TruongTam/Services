using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int Order_Id { get; set; }
        public Order Order { get; set; }

        [ForeignKey("Product")]
        public int Product_Id { get; set; }
        public Product Product { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        public int PriceSingle { get; set; }

        [Display(Name = "Tổng tiền")]
        public int PriceTotal { get; set; }
    }
}
