using Fashion.Data;
using Fashion.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userProducts = await _context.AppUserProducts
                .Where(up => up.AppUserId == userId)
                .Include(up => up.Product)
                    .ThenInclude(p => p.ProductImages) // ProductImages'i de yüklüyoruz
                .ToListAsync();

            var products = userProducts.Select(up => up.Product).ToList();

            var totalPrice = products.Sum(p => p.Price);

            var model = new CartVM
            {
                Products = products,
                TotalPrice = totalPrice
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var appUserProduct = await _context.AppUserProducts
                .FirstOrDefaultAsync(up => up.AppUserId == userId && up.ProductId == id);

            if (appUserProduct == null)
                return NotFound();

            _context.AppUserProducts.Remove(appUserProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
