 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
   
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ProductBrand")]
        public int ProductBrand_Id { get; set; }
        public ProductBrand ProductBrand { get; set; }

        [ForeignKey("ProductType")]
        public int ProductType_Id { get; set; }
        public ProductType ProductType { get; set; }

        [ForeignKey("Slug")]
        public int Slug_Id { get; set; }
        public virtual Slug Slug { get; set; }

        [ForeignKey("Admin")]
        public int Admin_Id { get; set; }
        public Admin Admin { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả sản phẩm")]
        [Column(TypeName = "ntext")]
        public string Description { get; set; } 

        [Display(Name = "Đơn giá")]
        public int Price { get; set; } = 0;

        [Range(0.0,100.0)]
        [Display(Name = "Phần trăm khuyến mãi")]
        public double Saleoff { get; set; } = 0.0;

        [Display(Name = "Ảnh đại diện")]
        public string Thumbnail { get; set; } = "#";

        [Display(Name = "Số lượng trong kho")]
        public int Instore { get; set; } = 0;

        [Range(0.0,5.0)]
        [Display(Name = "Đánh giá trung bình")]
        public double Stars { get; set; } = 5.0;

        [Display(Name = "Điểm tích lũy")]
        public int BuyPoints { get; set; } = 0;

        [Display(Name = "Lượt xem")]
        public int Views { get; set; } = 0;

        [Display(Name = "Lượt đặt hàng")]
        public int Orders { get; set; } = 0;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày tạo")]
        public DateTime DateCreate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày chỉnh sửa cuối")]
        public DateTime DateModify { get; set; }


        [Display(Name = "Bài viết về sản phẩm")]
        [Column(TypeName = "ntext")]
        public string Article { get; set; } 

        // Phần này dành cho khóa ngoại
        #region Foreign Keys
        public ICollection<Comment> Comments { get; set; }
        public ICollection<CartDetail> CartDetails { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
       // public ICollection<ProductBrand> ProductBrands { get; set; }
        #endregion
    }
}
