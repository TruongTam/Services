using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmeticShop.Models;

namespace CosmeticShop.ViewModels
{
    public class AdminAdminNewViewModel
    {
        public Admin Admin {get;set;}
        public IEnumerable<Role> Roles {get;set;}
    }
}