using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class Setting
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Admin")]
        public int Admin_Id { get; set; }
        public Admin Admin { get; set; }

        [Required(ErrorMessage = "Tên thuộc tính không được để trống.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá trị thuộc tính không được để trống.")]
        public string Value { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCreate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateModify { get; set; }
    }
}
