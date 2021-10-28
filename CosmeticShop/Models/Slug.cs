using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class Slug
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn không được để trống")]
        [Display(Name = "Đường dẫn")]
        public string Url { get; set; }

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
        public virtual Product Product { get; set; }
        #endregion
    }
}
