using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class CommentReply
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int User_Id { get; set; }
        public User User { get; set; }

        [ForeignKey("Comment")]
        public int Comment_Id { get; set; }
        public Comment Comment { get; set; }

        [Display(Name = "Nội dung trả lời")]
        public string Content { get; set; }

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
