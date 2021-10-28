using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CosmeticShop.Models
{
    public class AnoCart
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CartStatus")]
        public int CartStatus_Id { get; set; }
        public CartStatus CartStatus { get; set; }

        [Display(Name = "Tổng số tiền")]
        public int TotalPrice { get; set; } = 0;

        [Display(Name = "Số lượng hàng trong giỏ")]
        public int TotalQuantity { get; set; } = 0;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày tạo")]
        public DateTime DateCreate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày chỉnh sửa cuối")]
        public DateTime DateModify { get; set; }

        // Phần này dành cho khóa ngoại
        #region Foreign Keys
        public ICollection<AnoCartDetail> AnoCartDetails { get; set; }
        #endregion
    }
}
