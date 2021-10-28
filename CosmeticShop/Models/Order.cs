using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int User_Id { get; set; }
        public User User { get; set; }

        [ForeignKey("OrderStatus")]
        public int OrderStatus_Id { get; set; }
        public OrderStatus OrderStatus { get; set; }

        [ForeignKey("PayType")]
        public int PayType_Id { get; set; }
        public PayType PayType { get; set; }

        [ForeignKey("PayStatus")]
        public int PayStatus_Id { get; set; }
        public PayStatus PayStatus { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(20, ErrorMessage = "Số điện thoại phải từ 10 đến 20 kí tự", MinimumLength = 10)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tổng số tiền")]
        public int TotalPrice { get; set; } = 0;

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên không quá 100 kí tự")]
        [Display(Name = "Tên")]
        public string NameFirst { get; set; }

        [StringLength(100, ErrorMessage = "Họ đệm không quá 100 kí tự")]
        [Display(Name = "Họ đệm")]
        public string NameMiddle { get; set; }

        [Required(ErrorMessage = "Họ không được để trống")]
        [StringLength(100, ErrorMessage = "Họ không quá 100 kí tự")]
        [Display(Name = "Họ")]
        public string NameLast { get; set; }

        [Required(ErrorMessage = "Số nhà không được để trống")]
        [Display(Name = "Số nhà")]
        public string AddressApartment { get; set; }

        [Required(ErrorMessage = "Đường không được để trống")]
        [Display(Name = "Đường")]
        public string AddressStreet { get; set; }

        [Required(ErrorMessage = "Quận/Huyện không được để trống")]
        [Display(Name = "Quận/Huyện")]
        public string AddressDistrict { get; set; }

        [Required(ErrorMessage = "Thành phố/Tỉnh không được để trống")]
        [Display(Name = "Thành phố/Tỉnh")]
        public string AddressCity { get; set; }

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
        public ICollection<OrderDetail> OrderDetails { get; set; }
        #endregion
    }
}
