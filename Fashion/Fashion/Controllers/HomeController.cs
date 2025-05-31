using Fashion.Data;
using Fashion.Models;
using Fashion.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Fashion.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        public HomeController(AppDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }
        public async Task<IActionResult> Index()
        {
            var categories= await _context.Categories.Take(3).ToListAsync();
            var products= await _context.Products.Include(m=>m.Category).Include(m=>m.ProductImages).ToListAsync();
            var blogs= await _context.Blogs.OrderByDescending(m=>m.CreatedTime).Take(3).ToListAsync();
            HomeVM homeVM = new()
            {
                Categories = categories,
                Products = products,
                Blogs = blogs
            };
            return View(homeVM);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductToBasket(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var existingEntry = await _context.AppUserProducts
                .FirstOrDefaultAsync(up => up.AppUserId == userId && up.ProductId == id);

            if (existingEntry != null)
            {
                return BadRequest("Product is already in the basket.");
            }

            var appUserProduct = new AppUserProduct
            {
                AppUserId = userId,
                ProductId = id
            };

            _context.AppUserProducts.Add(appUserProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Cart");
        }



    }
}
