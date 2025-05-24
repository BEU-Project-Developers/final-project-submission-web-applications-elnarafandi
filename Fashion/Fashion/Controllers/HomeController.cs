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
        public async Task<IActionResult> Cart()
        {
            List<BasketVM> basketDatas = [];
            decimal total = 0;
            if (_accessor.HttpContext.Request.Cookies["basket"] != null)
            {
                basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
            List<Product> products = new();
            foreach (var item in basketDatas)
            {
                var product = await _context.Products.Include(m=>m.ProductImages).FirstOrDefaultAsync(m=>m.Id==item.ProductId);
                products.Add(product);
            }
            foreach (var product in products)
            {
                total += product.Price;
            }
            return View(new BasketDetailVM
            {
                Products = products,
                Total = total
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToBasket(int id)
        {
            List<BasketVM> basketDatas = [];
            decimal total = 0;
            if (_accessor.HttpContext.Request.Cookies["basket"] != null)
            {
                basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
            List<Product> products = new();
            foreach (var item in basketDatas)
            {
                var product = await _context.Products.Include(m => m.ProductImages).FirstOrDefaultAsync(m => m.Id == item.ProductId);
                products.Add(product);
            }
            basketDatas.Add(new BasketVM { ProductId = id });
            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));
            return RedirectToAction("Cart");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            List<BasketVM> basketDatas = [];
            decimal total = 0;
            if (_accessor.HttpContext.Request.Cookies["basket"] != null)
            {
                basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
            var existBasketData = basketDatas.FirstOrDefault(m => m.ProductId == id);
            basketDatas.Remove(existBasketData);
            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));
            return RedirectToAction("Cart");
        }

    }
}
