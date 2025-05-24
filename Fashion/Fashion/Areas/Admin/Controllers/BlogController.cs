using Fashion.Areas.Admin.ViewModels.Blog;
using Fashion.Data;
using Fashion.Helpers.Extensions;
using Fashion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateVM request)
        {
            if (!request.UploadImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("UploadImage", "Input type must be only image");
                return View(request);
            }
            if (!request.UploadImage.CheckFileSize(500))
            {
                ModelState.AddModelError("UploadImage", "Image size must be smaller than 500KB");
                return View(request);
            }
            string fileName = Guid.NewGuid().ToString() + "-" + request.UploadImage.FileName;
            string filePath = _env.GenerateFilePath("img/blog", fileName);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await request.UploadImage.CopyToAsync(stream);
            }
            await _context.Blogs.AddAsync(new Blog { Image=fileName, Title=request.Title, Description=request.Description});
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) return NotFound();
            string filePath = _env.GenerateFilePath("img/blog", blog.Image);
            filePath.DeleteFile();
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            var blog= await _context.Blogs.FirstOrDefaultAsync(b=>b.Id==id);
            if(blog == null) return NotFound();
            return View(blog);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) return NotFound();
            return View(new BlogEditVM {Image=blog.Image,Title=blog.Title,Description=blog.Description});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, BlogEditVM request)
        {
            if (id == null) return BadRequest();
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) return NotFound();
            if (request.UploadImage != null)
            {
                if (!request.UploadImage.CheckFileType("image/"))
                {
                    ModelState.AddModelError("UploadImage", "Input type must be only image");
                    return View(request);
                }
                if (!request.UploadImage.CheckFileSize(500))
                {
                    ModelState.AddModelError("UploadImage", "Image size must be smaller than 500KB");
                    return View(request);
                }
                string oldFilePath = _env.GenerateFilePath("img/blog", blog.Image);
                oldFilePath.DeleteFile();
                string fileName = Guid.NewGuid().ToString() + "-" + request.UploadImage.FileName;
                string filePath = _env.GenerateFilePath("img/blog", fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.UploadImage.CopyToAsync(stream);
                }
                blog.Image = fileName;
            }
            blog.Title = request.Title;
            blog.Description = request.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
