using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticShop.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Role")]
        public int Role_Id { get; set; }
        public Role Role { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(20,ErrorMessage = "Số điện thoại phải từ 10 đến 20 kí tự",MinimumLength = 10)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(25, ErrorMessage = "Mật khẩu từ 6 đến 25 kí tự",MinimumLength = 6)]
        [Display(Name = "Mật khẩu")]
        public string Password {get;set;}

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100,ErrorMessage = "Tên không quá 100 kí tự")]
        [Display(Name = "Tên")]
        public string NameFirst { get; set; }

        [StringLength(100,ErrorMessage = "Họ đệm không quá 100 kí tự")]
        [Display(Name = "Họ đệm")]
        public string NameMiddle { get; set; }

        [Required(ErrorMessage = "Họ không được để trống")]
        [StringLength(100, ErrorMessage = "Họ không quá 100 kí tự")]
        [Display(Name = "Họ")]
        public string NameLast { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; } = "Nam";

        [Display(Name = "Ngày sinh")]
        public DateTime DateBirth { get; set; }

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

        [Display(Name = "Số điểm tích lũy")]
        public int BuyPoints { get; set; } = 0;

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
        public ICollection<Comment> Comments { get; set; }
        public ICollection<CommentReply> CommentReplies { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }

        #endregion
    }
}
