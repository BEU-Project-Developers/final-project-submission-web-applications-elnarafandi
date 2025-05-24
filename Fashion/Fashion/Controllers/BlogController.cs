using Fashion.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        public BlogController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs.ToListAsync();
            return View(blogs);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var blog= await _context.Blogs.FirstOrDefaultAsync(m=>m.Id==id);
            return View(blog);
        }
    }
}
