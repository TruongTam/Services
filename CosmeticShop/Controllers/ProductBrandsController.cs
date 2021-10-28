using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CosmeticShop.Data;
using CosmeticShop.Models;

namespace CosmeticShop.Controllers
{
    public class ProductBrandsController : Controller
    {
        private readonly CosmeticShopDbContext _context;

        public ProductBrandsController(CosmeticShopDbContext context)
        {
            _context = context;
        }

        // GET: ProductBrands suong1


        // GET: ProductBrands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productBrand = await _context.ProductBrands
                .Include(p => p.Admin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productBrand == null)
            {
                return NotFound();
            }

            return View(productBrand);
        }

        // GET: ProductBrands/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("Admin_ID") == 1)
            {

                ViewData["Admin_Id"] = new SelectList(_context.Admins, "Id", "Email");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");

            }

        }

        // POST: ProductBrands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Admin_Id,Name,DateCreate,DateModify")] ProductBrand productBrand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productBrand);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProductBrandsOverview", "Admin");
            }
            ViewData["Admin_Id"] = new SelectList(_context.Admins, "Id", "Email", productBrand.Admin_Id);
            return View(productBrand);
        }

        // GET: ProductBrands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productBrand = await _context.ProductBrands.FindAsync(id);
            if (productBrand == null)
            {
                return NotFound();
            }
            ViewData["Admin_Id"] = new SelectList(_context.Admins, "Id", "Email", productBrand.Admin_Id);
            return View(productBrand);
        }

        // POST: ProductBrands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Admin_Id,Name,DateCreate,DateModify")] ProductBrand productBrand)
        {
            if (id != productBrand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productBrand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductBrandExists(productBrand.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ProductBrandsOverview", "Admin");
            }
            ViewData["Admin_Id"] = new SelectList(_context.Admins, "Id", "Email", productBrand.Admin_Id);
            return View(productBrand);
        }

        // GET: ProductBrands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productBrand = await _context.ProductBrands
                .Include(p => p.Admin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productBrand == null)
            {
                return NotFound();
            }

            return View(productBrand);
        }

        // POST: ProductBrands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productBrand = await _context.ProductBrands.FindAsync(id);
            _context.ProductBrands.Remove(productBrand);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductBrandsOverview", "Admin");
        }

        private bool ProductBrandExists(int id)
        {
            return _context.ProductBrands.Any(e => e.Id == id);
        }
    }
}