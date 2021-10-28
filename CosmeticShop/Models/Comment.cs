using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int User_Id{ get; set; }
        public User User { get; set; }

        [ForeignKey("Product")]
        public int Product_Id { get; set; }
        public Product Product { get; set; }
        
        [Required(ErrorMessage = "Hãy chấm điểm sản phẩm trước khi bình luận hoặc đánh giá.")]
        [Range(0.0,5.0)]
        [Display(Name = "Điểm đánh giá")]
        public double Stars { get; set; }

        [Display(Name = "Nội dung bình luận")]
        public string Content { get; set; }

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
        public ICollection<CommentReply> CommentReplies { get; set; }    
        #endregion
    }
}
