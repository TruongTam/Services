using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class ProductBrand
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Admin")]
        public int Admin_Id { get; set; }
        public Admin Admin { get; set; }

        [Required(ErrorMessage = "Tên nhãn hiệu không được để trống")]
        [Display(Name = "Tên nhãn hiệu")]
        public string Name { get; set; }

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
        public ICollection<Product> Products { get; set; }
        #endregion
    }
}
