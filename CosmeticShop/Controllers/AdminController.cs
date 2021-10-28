using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using CosmeticShop.Models;
using CosmeticShop.Data;
using CosmeticShop.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CosmeticShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly CosmeticShopDbContext _context;
        private readonly IHostingEnvironment _hostingEnviroment;
       
        public AdminController(CosmeticShopDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnviroment = hostingEnvironment;
        }
 
        #region Functions
        // Đã đăng nhập chưa
        public bool IsLogedIn()
        {
            if (HttpContext.Session.GetInt32("Role_Id") != null && HttpContext.Session.GetInt32("Admin_Id") != null)
            {
                return true;
            }
            return false;
        }

        // Có được phép truy cập hay không
        public bool IsAllowed()
        {
            if (HttpContext.Session.GetInt32("Role_Id") <= 3 && HttpContext.Session.GetInt32("Role_Id") > 0)
            {
                return true;
            }
            return false;
        }

        // Kiểm tra Slug đã tồn tại hay chưa
        public bool IsSlugExists(string data)
        {
            Slug slug = _context.Slugs.Where(s => s.Url == data).FirstOrDefault();
            if (slug != null){ return true;}
            return false;
        }

        // Xóa ảnh khỏi thư mục uploads theo sản phẩm
        public void RemoveImage(Product product)
        {
            string url = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","uploads",product.Thumbnail);
            if (System.IO.File.Exists(url)){
                System.IO.File.Delete(url);
            }
        }
        // Xóa ảnh khỏi thư mục uploads theo tên ảnh
        public void RemoveImage(string image)
        {
            string url = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","uploads",image);
            if (System.IO.File.Exists(url)){System.IO.File.Delete(url);}
        } 

        // Xóa sản phẩm
        public void RemoveProduct(Product product)
        {
            // Xóa ảnh
            RemoveImage(product.Thumbnail);
            
            // Xóa các bình luận
            List<Comment> comments = _context.Comments.Where(c => c.Product_Id == product.Id).ToList();
            if (comments != null)
            {
                foreach(var comment in comments)
                {
                    _context.Comments.Remove(comment);
                    _context.SaveChanges();
                }
            }
            
            // Xóa slug
            Slug slug_delete = _context.Slugs.Where(s => s.Id == product.Slug_Id).FirstOrDefault();
            _context.Slugs.Remove(slug_delete);
            
            // Xóa trong CartDetail
            List<CartDetail> cartdetails  = _context.CartDetails.Where(c => c.Product_Id == product.Id).ToList();
            if (cartdetails != null)
            {
                foreach (var detail in cartdetails)
                {
                    _context.CartDetails.Remove(detail);
                    _context.SaveChanges();
                }
            }
            
            // Xóa trong AnoCartDetail
            //List<AnoCartDetail> anocartdetails  = _context.AnoCartDetails.Where(c => c.Product_Id == product.Id).ToList();
            //if (cartdetails != null)
            //{
            //    foreach (var detail in anocartdetails)
            //    {
            //        _context.AnoCartDetails.Remove(detail);
            //        _context.SaveChanges();
            //    }
            //}

            _context.Products.Remove(product);
            //_context.SaveChanges();
        }
        #endregion

        // SuperAdmin = 1, Admin = 2, Author = 3
        // Trang phân công
        public IActionResult Index()
        {
            if (IsLogedIn() == true){ return View();}
            else{ return RedirectToAction("Login");}
        }

        // Trang đăng nhập
        public IActionResult Login(){ return View();}

        [HttpPost]
        public IActionResult Login(Admin admin)
        {
            Admin Auth = _context.Admins.Where(ad => ad.Username == admin.Username && ad.Password == admin.Password).FirstOrDefault();
            if (Auth == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                return View("Login");
            }
            else 
            if (Auth.Role_Id <= 3) //có 3 loại 
            {
                HttpContext.Session.SetString("Name", Auth.Username);
                HttpContext.Session.SetInt32("Role_Id", Auth.Role_Id);
                HttpContext.Session.SetInt32("Admin_Id", Auth.Id);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            if (IsLogedIn() == true)
            {
                HttpContext.Session.Remove("Role_Id");
                HttpContext.Session.Remove("Admin_Id");
            }
            return RedirectToAction("Login");
        }


        #region Nhóm trang sản phẩm
        // Trang tổng quát sản phẩm
        #region Sản phẩm
        public IActionResult ProductsOverview()
        {
            if (IsLogedIn() == true)
            {
                AdminProductsViewModel vm = new AdminProductsViewModel()
                {
                    Products = _context.Products.OrderByDescending(p => p.Id).ToList()
                };
                return View(vm);
            }
            else {return RedirectToAction("Login");}
        }


        [HttpGet]
        [Route("SearchProductAdmin")]
        public async Task<IActionResult> SearchProAdmin(string key_s, int? page)
        {
            if (key_s == null)
            {
                return Redirect("~/Admin/ProductsOverview");
            }
            else
            {
                AdminProductsViewModel vm = new AdminProductsViewModel()
                {
                    Products = _context.Products.Where(p => p.Name.ToUrlFriendly().Contains(key_s.ToUrlFriendly().Trim())).ToList()
                };
                return View(vm);
            }
         

        }

        [HttpGet]
        [Route("SearchProductBarndAdmin")]
        public async Task<IActionResult> SearchProductBarndAdmin(string key_s, int? page)
        {
            if (key_s == null)
            {
                return Redirect("~/Admin/ProductBrandsOverview");
            }
            else
            {
                AdminProductBrandsViewModel vm = new AdminProductBrandsViewModel()
                {
                    ProductBrand = new ProductBrand() { Admin_Id = (int)HttpContext.Session.GetInt32("Admin_Id") },
                    ProductBrands = _context.ProductBrands.Where(p => p.Name.ToUrlFriendly().Contains(key_s.ToUrlFriendly().Trim())).ToList(),
                    Products = _context.Products.ToList()
                };
                return View(vm);
            }


        }

        [HttpGet]
        [Route("SearchUsersOverview")]
        public async Task<IActionResult> SearchUsersOverview(string key_s, int? page)
        {
            if (key_s == null)
            {
                return Redirect("~/Admin/UsersOverview");
            }
            else
            {
               
                   
                  
                return View(_context.Users.Where(p => (p.NameFirst +p.NameMiddle+p.NameLast).ToUrlFriendly().Contains(key_s.ToUrlFriendly().Trim()))   );
            }


        }
        // tim user
        [HttpGet]
        [Route("SearchProductTypeAdmin")]
        public async Task<IActionResult> SearchProductTypeAdmin(string key_s, int? page)
        {
            if (key_s == null)
            {
                return Redirect("~/Admin/ProductTypesOverview");
            }
            else
            {
                AdminProductTypesViewModel vm = new AdminProductTypesViewModel()
                {
                    ProductType = new ProductType()
                    {
                        URL = "Chưa có đường dẫn url",
                        Admin_Id = (int)HttpContext.Session.GetInt32("Admin_Id")
                    },
                    ProductTypes = _context.ProductTypes.OrderBy(p => p.Type).ToList(),
                    Products = _context.Products.ToList(),
                    productBrands = _context.ProductBrands.ToList()
                };
                return View(vm);
               
            }


        }
        public IActionResult ProductNew()
        {
            if (IsLogedIn() == true)
            {
                // Tạo slug mới
                var new_slug = new Slug();
                new_slug.Url = DateTime.Now.ToString();
                new_slug.DateCreate = DateTime.Now;
                new_slug.DateModify = DateTime.Now;
                _context.Slugs.Add(new_slug);
                _context.SaveChanges();

                ProductNewViewModel viewmodel = new ProductNewViewModel()
                {
                    Product = new Product()
                    {
                        Slug_Id = _context.Slugs.LastOrDefault().Id,
                        Admin_Id = (int)HttpContext.Session.GetInt32("Admin_Id")
                    },
                    ProductBrands = _context.ProductBrands.ToList(),
                    ProductTypes = _context.ProductTypes.ToList()
                };
                return View(viewmodel);
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpPost]
        public IActionResult ProductNew(ProductNewViewModel viewmodel)
        {
            if (IsLogedIn() == true)
            {
                Product new_product = viewmodel.Product;
                new_product.Thumbnail = "#";
                // Tải hình ảnh sản phẩm lên thư mục wwwroot/uploads
                if (viewmodel.Thumbnail != null)
                {
                    new_product.Thumbnail = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    string uniqueName = new_product.Thumbnail; // Tạo tên hình ảnh theo chuỗi ngày tháng lúc đăng ảnh
                    string newpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"); // Trỏ đường dẫn đến thư mục wwwroot/uploads
                    newpath = Path.Combine(newpath, uniqueName); // Trỏ đường dẫn đến tên hình ảnh
                    newpath = newpath + Path.GetExtension(viewmodel.Thumbnail.FileName); // Gắn đuôi (loại file) cho hình
                    viewmodel.Thumbnail.CopyTo(new FileStream(newpath, FileMode.Create)); // Copy hình từ nguồn sang wwwroot/uploads
                    new_product.Thumbnail += Path.GetExtension(viewmodel.Thumbnail.FileName);
                }

                // Đổi slug theo tên sản phẩm
                Slug edit_slug = _context.Slugs.Where(s => s.Id == new_product.Slug_Id).FirstOrDefault();
                string new_slug = StaticClass.ToUrlFriendly(new_product.Name);
                if (IsSlugExists(new_slug) == false){ edit_slug.Url = new_slug; }
                
                _context.Products.Add(new_product);
                _context.SaveChanges();
                return RedirectToAction("ProductsOverview", "Admin");
            }
            else
            { return RedirectToAction("Login");}
        }
        
        [HttpGet]
        public IActionResult ProductDetail(int id)
        {
            if (IsLogedIn() == true)
            {
                Product detail = _context.Products.Where(p => p.Id == id).FirstOrDefault();
                if (detail != null)
                {
                    string brand = _context.ProductBrands.Where(p => p.Id == detail.ProductBrand_Id).FirstOrDefault().Name;
                    if (brand == null) { brand = "Chưa xác định nhãn hàng"; }

                    string type = _context.ProductTypes.Where(p => p.Id == detail.ProductType_Id).FirstOrDefault().Type;
                    if (type == null) { type = "Chưa xác định loại sản phẩm"; }

                    string admin = _context.Admins.Where(p => p.Id == detail.Admin_Id).FirstOrDefault().Username;
                    if (admin == null) { admin = "Không rõ người đăng";}

                    ProductDetailViewModel vm = new ProductDetailViewModel()
                    {
                        Product       = detail,
                        BrandName     = brand,
                        ProductType   = type,
                        AdminUsername = admin
                    };
                    return View(vm);
                }
                else{ return RedirectToAction("ProductsOverview","Admin");}
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult ProductEdit(int id)
        {
            if (IsLogedIn() == true)
            {
                Product edit = _context.Products.Where(p => p.Id == id).FirstOrDefault();
                if (edit != null)
                {
                    ProductEditViewModel vm = new ProductEditViewModel()
                    {
                        Product = edit,
                        ProductBrands = _context.ProductBrands.OrderBy(p => p.Name).ToList(),
                        ProductTypes  = _context.ProductTypes.OrderBy(p => p.Type).ToList()
                    };
                    return View(vm);
                }
                else{ return RedirectToAction("ProductsOverview","Admin");}
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpPost]
        public IActionResult ProductEdit(ProductEditViewModel vm)
        {
            if (IsLogedIn() == true)
            {
                if (ModelState.IsValid)
                {
                    Product data = vm.Product;
                    Product up = _context.Products.Where(p => p.Id == data.Id).FirstOrDefault();

                    up.Name            = data.Name;
                    up.Description     = data.Description;
                    up.ProductBrand_Id = data.ProductBrand_Id;
                    up.ProductType_Id  = data.ProductType_Id;
                    up.Description     = data.Description;
                    up.Price           = data.Price;
                    up.Saleoff         = data.Saleoff;
                    up.DateModify      = data.DateModify;
                    up.Instore         = data.Instore;
                    up.BuyPoints       = data.BuyPoints;

                    if (vm.Thumbnail != null)
                    {
                        // Xóa ảnh cũ
                        string old_image = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","uploads",data.Thumbnail);
                        if (System.IO.File.Exists(old_image))
                        {
                            System.IO.File.Delete(old_image);
                        }
                
                        // Lưu ảnh mới
                        string rename = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + Path.GetExtension(vm.Thumbnail.FileName);
                        string new_image = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","uploads", rename);
                        vm.Thumbnail.CopyTo(new FileStream(new_image,FileMode.Create));
                        up.Thumbnail = rename;
                    }
                    // Đổi slug theo tên sản phẩm
                    Slug edit_slug = _context.Slugs.Where(s => s.Id == up.Slug_Id).FirstOrDefault();
                    string new_slug = StaticClass.ToUrlFriendly(up.Name);
                    if (IsSlugExists(new_slug) == false){ edit_slug.Url = new_slug; }

                    _context.SaveChanges();

                    return View(
                        new ProductEditViewModel()
                        {
                            Product       = _context.Products.Where(p => p.Id == vm.Product.Id).FirstOrDefault(),
                            ProductBrands = _context.ProductBrands.OrderBy(p => p.Name).ToList(),
                            ProductTypes  = _context.ProductTypes.OrderBy(p => p.Type).ToList()
                        }
                    );
                }
                else
                {
                    return View(
                        new ProductEditViewModel()
                        {
                            Product = vm.Product,
                            ProductBrands = _context.ProductBrands.OrderBy(p => p.Name).ToList(),
                            ProductTypes  = _context.ProductTypes.OrderBy(p => p.Type).ToList()
                        }
                    );
                }
            }
            else { return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult ProductDelete(int id)
        {
            if (IsLogedIn() == true)
            {
                Product delete = _context.Products.Where(p => p.Id == id).FirstOrDefault();
                if (delete != null)
                {
                    RemoveProduct(delete);
                }
                return RedirectToAction("ProductsOverview","Admin");
            }
            else { return RedirectToAction("Login");}
        }
        #endregion
        #region Các nhãn hàng
        // Trang tổng quát các nhãn hàng
        public IActionResult ProductBrandsOverview()
        {
            if (IsLogedIn() == true)
            {  
                AdminProductBrandsViewModel vm = new AdminProductBrandsViewModel()
                { 
                    ProductBrand  = new ProductBrand(){ Admin_Id = (int)HttpContext.Session.GetInt32("Admin_Id")},
                    ProductBrands = _context.ProductBrands.OrderBy(p => p.Name).ToList(),
                    Products      = _context.Products.ToList()
                };
                return View(vm);
            }
            else{ return RedirectToAction("Login");}
        }


        [HttpPost]
        public IActionResult ProductBrandsOverview(AdminProductBrandsViewModel vm)
        {
            if (IsLogedIn() == true)
            {
                if (ModelState.IsValid)
                {
                    _context.ProductBrands.Add(vm.ProductBrand);
                    _context.SaveChanges();
                    return View(
                        new AdminProductBrandsViewModel()
                        { 
                            ProductBrand  = new ProductBrand(){ Admin_Id = (int)HttpContext.Session.GetInt32("Admin_Id")},
                            ProductBrands = _context.ProductBrands.OrderBy(p => p.Name).ToList(),
                            Products      = _context.Products.ToList()
                        }
                    );
                }
                else
                {
                    return View(
                        new AdminProductBrandsViewModel()
                        { 
                            ProductBrand  = vm.ProductBrand,
                            ProductBrands = _context.ProductBrands.OrderBy(p => p.Name).ToList(),
                            Products      = _context.Products.ToList()
                        }
                    );
                }
            }
            else{ return RedirectToAction("Login");}
        }
        
        [HttpGet]
        public IActionResult ProductBrandEdit(int id)
        {
            if (IsLogedIn() == true)
            {
                ProductBrand edit = _context.ProductBrands.Where(p => p.Id == id).FirstOrDefault();
                if (edit != null)
                {
                    AdminProductBrandEditViewModel vm = new AdminProductBrandEditViewModel()
                    {
                        ProductBrand  = edit,
                        ProductBrands = _context.ProductBrands.OrderBy(p => p.Name).ToList(),
                        Products      = _context.Products.ToList()
                    };
                    return View(vm);
                }
                else{ return RedirectToAction("ProductBrandsOverview");}
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpPost]
        public IActionResult ProductBrandEdit(AdminProductBrandEditViewModel vm)
        {
            if (IsLogedIn() == true)
            {
                if (ModelState.IsValid)
                {
                    ProductBrand data = vm.ProductBrand;
                    ProductBrand up = _context.ProductBrands.Where(p => p.Id == data.Id).FirstOrDefault();
                    up.Name = data.Name;
                    up.DateModify = DateTime.Now;
                    _context.SaveChanges();
                }
                return View(
                    new AdminProductBrandEditViewModel()
                    {
                        ProductBrand  = vm.ProductBrand,
                        ProductBrands = _context.ProductBrands.OrderBy(p => p.Name).ToList(),
                        Products      = _context.Products.ToList()
                    }
                );
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult ProductBrandDelete(int id)
        {
            if (IsLogedIn() == true)
            {
                ProductBrand delete = _context.ProductBrands.Where(p => p.Id == id).FirstOrDefault();
                if (delete != null)
                {
                   // Xóa tất cả sản phẩm
                   // Lý do không đổi ProductBrand_Id = 0 vì đó là khóa ngoại
                   // mà Microsoft thì không cho phép gán khóa ngoại cho giá trị không tồn tại
        
                   List<Product> products = _context.Products.Where(p => p.ProductBrand_Id == id).ToList();
                   foreach(Product product in products)
                   {
                       RemoveProduct(product);
                   }

                   // Xóa nhãn hàng sau khi đã xóa tất cả sản phẩm liên quan
                    _context.ProductBrands.Remove(delete);
                    _context.SaveChanges();
                }
                return RedirectToAction("ProductBrandsOverview");
            }
            else{ return RedirectToAction("Login");}
        }

        #endregion
        // Trang tổng quát loại sản phẩm
        #region Loại sản phẩm
        public IActionResult ProductTypesOverview()
        {
            if (IsLogedIn() == true)
            {
                AdminProductTypesViewModel vm = new AdminProductTypesViewModel()
                {
                    ProductType = new ProductType(){
                        URL = "Chưa có đường dẫn url",
                        Admin_Id = (int)HttpContext.Session.GetInt32("Admin_Id")
                    },
                    ProductTypes = _context.ProductTypes.OrderBy(p => p.Type).ToList(),
                    Products     = _context.Products.ToList(),
                    productBrands = _context.ProductBrands.ToList()
                };
                return View(vm);
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpPost]
        public IActionResult ProductTypesOverview(AdminProductTypesViewModel vm)
        {
            if (IsLogedIn() == true)
            {
                if (ModelState.IsValid)
                {
                    vm.ProductType.URL = StaticClass.ToUrlFriendly(vm.ProductType.Type);
                    _context.ProductTypes.Add(vm.ProductType);
                    _context.SaveChanges();
                }
                return View(new AdminProductTypesViewModel()
                {
                    ProductType = new ProductType(){
                        URL = "Chưa có đường dẫn url",
                        Admin_Id = (int)HttpContext.Session.GetInt32("Admin_Id")
                    },
                    ProductTypes = _context.ProductTypes.OrderBy(p => p.Type).ToList(),
                    Products     = _context.Products.ToList()
                });
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult ProductTypeEdit(int id)
        {
            if (IsLogedIn() == true)
            {
                ProductType edit = _context.ProductTypes.Where(p => p.Id == id).FirstOrDefault();
                if (edit != null)
                {
                    return View(new AdminProductTypesViewModel()
                    {
                        ProductType  = edit,
                        ProductTypes = _context.ProductTypes.OrderBy(p => p.Type).ToList(),
                        Products     = _context.Products.ToList()
                    });
                }
                else
                { return RedirectToAction("ProductTypesOverview");}
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpPost]
        public IActionResult ProductTypeEdit(AdminProductTypesViewModel vm)
        {
            if (IsLogedIn() == true)
            {
                if (ModelState.IsValid)
                {
                      ProductType data = vm.ProductType;
                      ProductType up = _context.ProductTypes.Where(p => p.Id == data.Id).FirstOrDefault();
                      up.Type = data.Type;
                      up.DateModify = data.DateModify;
                      up.URL = StaticClass.ToUrlFriendly(data.Type);
                      _context.SaveChanges();
                }
                return View(
                    new AdminProductTypesViewModel()
                    {
                        ProductType  = _context.ProductTypes.Where(p => p.Id == vm.ProductType.Id).FirstOrDefault(),
                        ProductTypes = _context.ProductTypes.OrderBy(p => p.Type).ToList(),
                        Products     = _context.Products.ToList()
                    }
                ); 
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult ProductTypeDelete(int id)
        {
            if (IsLogedIn() == true)
            {
                ProductType delete = _context.ProductTypes.Where(p => p.Id == id).FirstOrDefault();
                if (delete != null)
                {
                    // Xóa tất cả sản phẩm liên quan
                    List<Product> products = _context.Products.Where(p => p.ProductType_Id == id).ToList();
                    foreach (Product product in products)
                    {
                        RemoveProduct(product);
                    }

                    // Xóa loại sản phẩm
                    _context.ProductTypes.Remove(delete);
                    _context.SaveChanges();
                }
                return RedirectToAction("ProductTypesOverview");
            }
            else{ return RedirectToAction("Login");}
        }
        #endregion
        #endregion

        #region Nhóm trang quản trị
        // Trang tổng quan quản trị
        public IActionResult AdminsOverview()
        {
            if (IsLogedIn() == true){ return View(_context.Admins.OrderByDescending(a => a.Id));}
            else{ return RedirectToAction("Login");}
        }
        //public IActionResult AdminEdit(int id)
        //{
        //    if(IsLogedIn() == true)
        //    {

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login");
        //    }
        //}
        public IActionResult AdminNew()
        {
            if (IsLogedIn() == true)
            { 
                return View(new AdminAdminNewViewModel()
                {
                    Admin = new Admin(),
                    Roles = _context.Roles.OrderBy(r => r.Name)
                });
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpPost]
        public IActionResult AdminNew(AdminAdminNewViewModel vm)
        {
            if (IsLogedIn() == true)
            { 
                if (ModelState.IsValid)
                {
                    _context.Admins.Add(vm.Admin);
                    _context.SaveChanges();
                    return RedirectToAction("AdminsOverview");
                }
                else
                {
                    return View(new AdminAdminNewViewModel()
                    {
                        Admin = vm.Admin,
                        Roles = _context.Roles.OrderBy(r => r.Name)
                    });
                }
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult AdminEdit(int id)
        {
            if (IsLogedIn() == true)
            { 
                Admin edit = _context.Admins.Where(a => a.Id == id).FirstOrDefault();
                if (edit != null)
                {
                    return View(
                        new AdminAdminNewViewModel()
                        {
                            Admin = edit,
                            Roles = _context.Roles.OrderBy(r => r.Name)
                        });
                }
                else { return RedirectToAction("AdminsOverview");}
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpPost]
        public IActionResult AdminEdit(AdminAdminNewViewModel vm)
        {
            if (IsLogedIn() == true)
            { 
                if (ModelState.IsValid)
                {
                    Admin data = vm.Admin;
                    Admin up = _context.Admins.Where(a => a.Id == data.Id).FirstOrDefault();

                    up.Password   = data.Password;
                    up.Email      = data.Email;
                    up.Role_Id    = data.Role_Id;
                    up.DateModify = data.DateModify;

                    _context.SaveChanges();
                }
                
                return View(
                    new AdminAdminNewViewModel()
                    {
                        Admin = _context.Admins.Where(a => a.Id == vm.Admin.Id).FirstOrDefault(),
                        Roles = _context.Roles.OrderBy(r => r.Name)
                    });
                
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        //dính quá nhiều khóa ngoại 
        public IActionResult AdminDelete(int id)
        {
            if (IsLogedIn() == true)
            { 
                Admin admin = _context.Admins.Where(a => a.Id == id).FirstOrDefault();
                if (admin != null)
                {
                   // Xóa tất cả sản phẩm, nhãn hàng, loại sản phẩm do quản trị viên này tạo
                    List<Product> products = _context.Products.Where(p => p.Admin_Id == id).ToList();

                    List<ProductBrand> brands = _context.ProductBrands.Where(p => p.Admin_Id == id).ToList();
                    List<ProductType> types = _context.ProductTypes.Where(p => p.Admin_Id == id).ToList();
                   
                    if (products != null) { foreach (var p in products) { RemoveProduct(p); } }
                    if (brands != null) { foreach (var b in brands) { _context.ProductBrands.Remove(b); _context.SaveChanges(); } }
                    if (types != null) { foreach (var t in types) { _context.ProductTypes.Remove(t); _context.SaveChanges(); } }

                    _context.Admins.Remove(admin);
                    _context.SaveChanges();
                }
                
                return RedirectToAction("AdminsOverview");
            }
            else{ return RedirectToAction("Login");}
        }

        #endregion

        #region Nhóm trang khách hàng
        // Nhóm trang này không có chức năng thêm mới và chỉnh sửa
        // là do việc thêm mới và chỉnh sửa thông tin là ở người dùng, quản trị viên không được can thiệp vào.
        // Trang tổng quát người dùng
        public IActionResult UsersOverview()
        {
            if (IsLogedIn() == true){ return View(_context.Users.OrderByDescending(u => u.Id));}
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult UserDetail(int id)
        {
            User detail = _context.Users.Where(u => u.Id == id).FirstOrDefault();
            if (detail != null){ return View(detail);}
            else {return RedirectToAction("UsersOverview");}
        }

        [HttpGet]
        public IActionResult UserDelete(int id)
        {
            User delete = _context.Users.Where(u => u.Id == id).FirstOrDefault();
            if (delete != null)
            {
                // Xóa các trả lời bình luận của người dùng
                List<CommentReply> replies = _context.CommentReplies.Where(c => c.User_Id == id).ToList();
                if (replies != null)
                {
                    foreach (var reply in replies){
                        _context.CommentReplies.Remove(reply);
                        _context.SaveChanges();
                    }
                }

                // Xóa các bình luận của người dùng
                List<Comment> comments = _context.Comments.Where(c => c.User_Id == id).ToList();
                if (comments != null)
                {
                    foreach (var comment in comments)
                    {
                        _context.Comments.Remove(comment);
                        _context.SaveChanges();
                    }
                }

                // Xóa chi tiết giỏ hàng
                Cart cart_delete = _context.Carts.Where(c => c.User_Id == id).FirstOrDefault();
                if (cart_delete != null)
                {
                     List<CartDetail> details = _context.CartDetails.Where(c => c.Cart_Id == cart_delete.Id).ToList();
                    if (details != null)
                    {
                        foreach (var detail in details)
                        {
                            _context.CartDetails.Remove(detail);
                            _context.SaveChanges();
                        }
                    } 

                    // Xóa giỏ hàng
                    _context.Carts.Remove(cart_delete);
                    _context.SaveChanges();
                }   
               
                // Xóa chi tiết và đơn đặt hàng
                Order order_delete = _context.Orders.Where(o => o.User_Id == id).FirstOrDefault();
                if (order_delete != null) // Đơn đặt hàng có thể null do khách hàng tạo tài khoản nhưng chưa đặt hàng
                {
                    List<OrderDetail> order_details = _context.OrderDetails.Where(o => o.Order_Id == order_delete.Id).ToList();
                    foreach (var detail in order_details)
                    {
                        _context.OrderDetails.Remove(detail);
                        _context.SaveChanges();
                    }
                    _context.Orders.Remove(order_delete);
                    _context.SaveChanges();
                }

                _context.Users.Remove(delete);
                _context.SaveChanges();
            }
            return RedirectToAction("UsersOverview");
        }
        #endregion
       
        #region Nhóm trang đơn đặt hàng
        public IActionResult Session()
        {
            return View();
        }
        
        // Trang tổng quát các đơn hàng
        public IActionResult OrdersOverview()
        {
            if (IsLogedIn() == true)
            { 
                return View(
                    new AdminOrdersOverviewViewModel()
                    {
                        Orders = _context.Orders.OrderByDescending(o => o.Id).OrderBy(o => o.OrderStatus_Id).ThenByDescending(o => o.PayType_Id).ThenBy(o => o.PayStatus_Id).ToList()
                    }
                );
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult OrderCheck(int id)
        {
            if (IsLogedIn() == true)
            { 
                Order check = _context.Orders.Where(o => o.Id == id).FirstOrDefault();
                if (check != null)
                {
                    IEnumerable<OrderDetail> details = _context.OrderDetails.Where(o => o.Order_Id == id).ToList();
                    List<Product> products = new List<Product>();
                    foreach (var detail in details)
                    {
                        Product product = _context.Products.Where(p => p.Id == detail.Product_Id).FirstOrDefault();
                        products.Add(product);
                    }
                    return View(
                        new AdminOrderCheckViewModel()
                        {
                            Order = check,
                            OrderDetails = details,
                            Products = products
                        }
                    );
                }
                else
                {
                    return RedirectToAction("OrdersOverview");
                }
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpPost]
        public IActionResult OrderCheck(AdminOrderCheckViewModel vm)
        {
            if (IsLogedIn() == true)
            { 
                IEnumerable<OrderDetail> details = _context.OrderDetails.Where(o => o.Order_Id == vm.Order.Id).ToList();
                List<Product> products = new List<Product>();
                foreach (var detail in details)
                {
                    Product product = _context.Products.Where(p => p.Id == detail.Product_Id).FirstOrDefault();
                    products.Add(product);
                }
                if (ModelState.IsValid)
                {
                    Order up = _context.Orders.Where(o => o.Id == vm.Order.Id).FirstOrDefault();
                    up.OrderStatus_Id = vm.Order.OrderStatus_Id;
                    _context.SaveChanges();
                    return View(
                        new AdminOrderCheckViewModel()
                        {
                            Order = up,
                            OrderDetails = details,
                            Products = products
                        }
                    );
                }
                return View(
                    new AdminOrderCheckViewModel()
                    {
                        Order = vm.Order,
                        OrderDetails = details,
                        Products = products
                    }
                );
            }
            else{ return RedirectToAction("Login");}
        }

        [HttpGet]
        public IActionResult OrderDetail(int id)
        {
            if (IsLogedIn() == true)
            { 
                Order check = _context.Orders.Where(o => o.Id == id).FirstOrDefault();
                if (check != null)
                {
                    IEnumerable<OrderDetail> details = _context.OrderDetails.Where(o => o.Order_Id == id).ToList();
                    List<Product> products = new List<Product>();
                    foreach (var detail in details)
                    {
                        Product product = _context.Products.Where(p => p.Id == detail.Product_Id).FirstOrDefault();
                        products.Add(product);
                    }
                    return View(
                        new AdminOrderCheckViewModel()
                        {
                            Order = check,
                            OrderDetails = details,
                            Products = products
                        }
                    );
                }
                else
                {
                    return RedirectToAction("OrdersOverview");
                }
            }
            else{ return RedirectToAction("Login");}
        }
        #endregion

        // Trang thống kê
        public IActionResult Report()
        {
            if (HttpContext.Session.GetInt32("Role_Id") <= 3)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // Trang cài đặt
        public IActionResult Setting()
        {
            if (HttpContext.Session.GetInt32("Role_Id") <= 3)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");

            }
        }
    }
}