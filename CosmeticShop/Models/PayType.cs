using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class PayType
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Loại thanh toán")]
        public string Type { get; set; }

        // Phần này dành cho khóa ngoại
        #region Foreign Keys
        public ICollection<Order> Orders { get; set; }
        #endregion
    }
}
