using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.ViewModels
{
    public class ItemProductsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả sản phẩm")]
        public string Description { get; set; } = "Sản phẩm này hiện chưa có mô tả";

        [Display(Name = "Đơn giá")]
        public int Price { get; set; } = 0;

        [Range(0.0, 100.0)]
        [Display(Name = "Phần trăm khuyến mãi")]
        public double Saleoff { get; set; } = 0.0;

        [Display(Name = "Ảnh đại diện")]
        public string Thumbnail { get; set; } = "#";
        //11
        [Range(0.0, 5.0)]
        [Display(Name = "Đánh giá trung bình")]
        public double Stars { get; set; } = 5.0;

        [Display(Name = "Lượt xem")]
        public int Views { get; set; } = 0;

        [Display(Name = "Lượt đặt hàng")]
        public int Orders { get; set; } = 0;
        public string TypeUrl { get; set; }
        public string NameUrl { get; set; }
    }
}