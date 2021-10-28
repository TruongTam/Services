using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CosmeticShop.ViewModels
{
    public class CommentsViewModel
    {
        [Key]
        public int Id { get; set; }

        public string User { get; set; }

        public int Product_Id { get; set; }

        public double Stars { get; set; }

        [Display(Name = "Nội dung bình luận")]
        public string Content { get; set; }

        public DateTime DateCreate { get; set; }
        public DateTime DateModify { get; set; }
    }
}
