using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using test.Models;
using test.Params;

namespace test.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Net1041Bai3Context _context;

        public ProductsController(Net1041Bai3Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Search(SearchParams filter)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();
            ViewData["SearchParams"] = filter;

            if (!string.IsNullOrEmpty(filter.Ten))
            {
                products = products.Where(p => p.ProductName.ToLower().Contains(filter.Ten.ToLower()));
            }
            if (filter.From.HasValue && filter.To.HasValue)
            {
                products = products.Where(p => p.Price >= filter.From.Value && p.Price <= filter.To.Value);
            }
            else if (filter.From.HasValue)
            {
                products = products.Where(p => p.Price >= filter.From.Value);
            }
            else if (filter.To.HasValue)
            {
                products = products.Where(p => p.Price <= filter.To.Value);
            }

            if (!string.IsNullOrEmpty(filter.TenProps) && !string.IsNullOrEmpty(filter.Sort))
            {
                var propertyInfo = typeof(Product).GetProperty(filter.TenProps);
                if (propertyInfo != null)
                {
                    if (filter.Sort.ToLower() == "asc")
                    {
                        products = products.OrderBy(p => EF.Property<object>(p, filter.TenProps));
                    }
                    else if (filter.Sort.ToLower() == "desc")
                    {
                        products = products.OrderByDescending(p => EF.Property<object>(p, filter.TenProps));
                    }
                }
            }
            var kichCoTrang = filter.KichCoTrang;
            var trangHienTai = filter.TrangHienTai < 1 ? 1 : filter.TrangHienTai;
            var list = await Pagination<Product>.CreateAsync(products, trangHienTai, kichCoTrang);

            return View("Index", list);
        }
        public async Task<IActionResult> Pagination(int trangHienTai = 1, int kichCoTrang = 5)
        {
            var data = _context.Products.Include(p => p.Category).AsQueryable();
            var paginatedList = await Pagination<Product>.CreateAsync(data, trangHienTai, kichCoTrang);
            return View("Index",paginatedList);
        }
        // GET: Products
        public async Task<IActionResult> Index()
        {
       

            var products = _context.Products.Include(p => p.Category).AsQueryable();
            var list = await Pagination<Product>.CreateAsync(products, 1, 5);
            ViewData["SearchParams"] = new SearchParams();
            return View(list);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,Quantity,CategoryId,Brand,Model,ManufactureDate,ExpirationDate,Rating,IsAvailable,CreatedDate,UpdatedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Price,Quantity,CategoryId,Brand,Model,ManufactureDate,ExpirationDate,Rating,IsAvailable,CreatedDate,UpdatedDate")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
