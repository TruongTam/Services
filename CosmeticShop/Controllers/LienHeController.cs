using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmeticShop.Data;
using CosmeticShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticShop.Controllers
{
    public class LienHeController : Controller
    {
        private CosmeticShopDbContext _context;
        public LienHeController(CosmeticShopDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            LienHe lh = new LienHe
            {
                ProductTypes = _context.ProductTypes.ToList(),
               productBrands = _context.ProductBrands.ToList()
            };
            return View(lh);
        }
        public IActionResult About()
        {
            LienHe lh = new LienHe
            {
                ProductTypes = _context.ProductTypes.ToList(),
                productBrands = _context.ProductBrands.ToList()
            };
            return View(lh);
        }
    }
}