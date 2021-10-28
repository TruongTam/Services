using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CosmeticShop.Models
{
    public class AnoCartDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AnoCart")]
        public int Cart_Id { get; set; }
        public AnoCart AnoCart { get; set; }

        [ForeignKey("Product")]
        public int Product_Id { get; set; }
        public Product Product { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        public int PriceSingle { get; set; }

        [Display(Name = "Tổng tiền")]
        public int PriceTotal { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày tạo")]
        public DateTime DateCreate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày chỉnh sửa cuối")]
        public DateTime DateModify { get; set; }
    }
}
