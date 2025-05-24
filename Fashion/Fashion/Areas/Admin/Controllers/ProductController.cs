using Fashion.Areas.Admin.ViewModels.Product;
using Fashion.Data;
using Fashion.Helpers.Extensions;
using Fashion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();
            ViewBag.categories = categories;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM request)
        {
            var categories = await _context.Categories.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();
            ViewBag.categories = categories;

            foreach (var item in request.ProductImages)
            {
                if (!item.CheckFileType("image/"))
                {
                    ModelState.AddModelError("ProductImages", "Input type mus be only image");
                    return View(request);
                }

                if (!item.CheckFileSize(500))
                {
                    ModelState.AddModelError("ProductImages", "Image size must be smaller than 500KB");
                    return View(request);
                }
            }

            List<ProductImage> productImages = new();

            foreach (var item in request.ProductImages)
            {
                string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;

                string filePath = _env.GenerateFilePath("img/product", fileName);

                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                productImages.Add(new ProductImage { Name = fileName, IsMain = false });
            }

            productImages.FirstOrDefault().IsMain = true;

            Product product = new Product();
            product.Name = request.Name;
            product.Size = request.Size;
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = (int)request.CategoryId;
            product.ProductImages = productImages;

            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.Include(p => p.ProductImages).Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            foreach (var item in product.ProductImages)
            {
                string filePath = _env.GenerateFilePath("img/product", item.Name);

                filePath.DeleteFile();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var categories = await _context.Categories.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();
            ViewBag.categories = categories;
            if (id == null) return BadRequest();
            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(new ProductEditVM { Name = product.Name, Price = product.Price,Size=product.Size, Description = product.Description, CategoryId = product.CategoryId, ProductImages = product.ProductImages });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProductEditVM request)
        {
            var categories = await _context.Categories.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();
            ViewBag.categories = categories;
            if (id == null) return BadRequest();
            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            
            if (request.UploadImages is not null)
            {
                foreach (var item in request.UploadImages)
                {
                    if (!item.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("UploadImages", "Input type mus be only image");
                        return View(request);
                    }

                    if (!item.CheckFileSize(500))
                    {
                        ModelState.AddModelError("UploadImages", "Image size must be smaller than 500KB");
                        return View(request);
                    }
                }

                foreach (var item in product.ProductImages)
                {
                    string filePath = _env.GenerateFilePath("img/product", item.Name);

                    filePath.DeleteFile();
                }
                _context.ProductImages.RemoveRange(product.ProductImages);

                List<ProductImage> productImages = new();

                foreach (var item in request.UploadImages)
                {
                    string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;

                    string filePath = _env.GenerateFilePath("img/product", fileName);

                    using (FileStream stream = new(filePath, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                    productImages.Add(new ProductImage { Name = fileName, IsMain = false, ProductId = product.Id });
                }

                productImages.FirstOrDefault().IsMain = true;
                await _context.ProductImages.AddRangeAsync(productImages);

            }
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Size = request.Size;
            product.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
