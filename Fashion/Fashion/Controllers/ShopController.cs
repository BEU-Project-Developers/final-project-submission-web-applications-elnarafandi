using Fashion.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string category, string sortOrder, string sortCreatedTime)
        {
            var productsQuery = _context.Products.Include(p => p.ProductImages).Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category.Name == category);
            }

            if (sortOrder == "min-max")
            {
                productsQuery = productsQuery.OrderBy(p => p.Price);
            }
            else if (sortOrder == "max-min")
            {
                productsQuery = productsQuery.OrderByDescending(p => p.Price);
            }
            if (sortCreatedTime == "old-new")
                productsQuery = productsQuery.OrderBy(p => p.CreatedTime);
            else if (sortCreatedTime == "new-old")
                productsQuery = productsQuery.OrderByDescending(p => p.CreatedTime);

            var products = await productsQuery.ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var product= await _context.Products.Include(m=>m.Category).Include(m=>m.ProductImages).FirstOrDefaultAsync(m=>m.Id==id);
            return View(product);
        }
    }
}
