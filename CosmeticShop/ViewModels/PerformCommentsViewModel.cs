using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmeticShop.ViewModels
{
    public class PerformCommentsViewModel
    {
        public IEnumerable<CommentsViewModel> Comments { get; set; }
        public int IdProduct { get; set; }
        public bool StillComments { get; set; }
    }
}
