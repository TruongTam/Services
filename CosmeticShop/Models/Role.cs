using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CosmeticShop.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Phân quyền")]
        public string Name { get; set; }

        // Phần này dành cho khóa ngoại
        #region Foreign Keys
        public ICollection<Admin> Admins { get; set; }
        public ICollection<User> Users { get; set; }
        #endregion
    }
}
