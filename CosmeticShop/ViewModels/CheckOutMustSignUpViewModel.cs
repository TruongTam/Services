using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CosmeticShop.Models;

namespace CosmeticShop.ViewModels
{
    public class CheckOutMustSignUpViewModel
    {
        public User User {get;set;}
        public int AnoCartId {get;set;}
        public IEnumerable<ProductType> ProductTypes { get; set; }

        public IEnumerable<ProductBrand> productBrands { get; set; }
    }
}
