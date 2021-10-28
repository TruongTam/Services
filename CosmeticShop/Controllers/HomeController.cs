using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CosmeticShop.Data;
using CosmeticShop.Models;
using CosmeticShop.ViewModels;

namespace CosmeticShop.Controllers
{
    public class HomeController : Controller
    {
        private CosmeticShopDbContext _context;
        
        public HomeController(CosmeticShopDbContext context)
        {
            _context = context;
        }

        // Trang chủ

        public IActionResult Index()
        {
            HomeViewModel viewmodel = new HomeViewModel()
            {
                NewProducts = GetListItemProducts(0),
                ViewMoreProduct = GetListItemProducts(1),
                ProductTypes = _context.ProductTypes.ToList(),
                productBrands = _context.ProductBrands.ToList()
                
            };
            return View(viewmodel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        ///
        private List<ItemProductsViewModel> GetListItemProducts(int type)
        {
            List<ItemProductsViewModel> products = new List<ItemProductsViewModel>();
            if (type == 0)
            {
                var query = (from p in _context.Products
                             join s in _context.Slugs
                             on p.Slug_Id equals s.Id
                             join t in _context.ProductTypes
                             on p.ProductType_Id equals t.Id
                             orderby p.DateCreate descending
                             select new ItemProductsViewModel
                             {
                                 Id = p.Id,
                                 Name = p.Name,
                                 Price = p.Price,
                                 Saleoff = p.Saleoff,
                                 Thumbnail = p.Thumbnail,
                                 Stars = p.Stars,
                                 Views = p.Views,
                                 Orders = p.Orders,
                                 NameUrl = s.Url,
                                 TypeUrl = t.URL
                             });
                products = query.Skip(0).Take(8).ToList();
            }
            else if (type == 1)
            {
                var query = (from p in _context.Products
                             join s in _context.Slugs
                             on p.Slug_Id equals s.Id
                             join t in _context.ProductTypes
                             on p.ProductType_Id equals t.Id
                             orderby p.Orders descending, p.Views descending
                             select new ItemProductsViewModel
                             {
                                 Id = p.Id,
                                 Name = p.Name,
                                 Price = p.Price,
                                 Saleoff = p.Saleoff,
                                 Thumbnail = p.Thumbnail,
                                 Stars = p.Stars,
                                 Views = p.Views,
                                 Orders = p.Orders,
                                 NameUrl = s.Url,
                                 TypeUrl = t.URL
                             });
                products = query.Skip(0).Take(8).ToList();
            }
            return products;
        }
        public IActionResult subri()
        {
            LienHe lh = new LienHe
            {
                ProductTypes = _context.ProductTypes.ToList()
            };
            return View(lh);
        }
    }
}