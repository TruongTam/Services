using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class PayStatus
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string Status { get; set; }

        // Phần này dành cho khóa ngoại
        #region Foreign Keys
        public ICollection<Order> Orders { get; set; }
        #endregion
    }
}
