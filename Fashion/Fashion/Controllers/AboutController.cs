using Microsoft.AspNetCore.Mvc;

namespace Fashion.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
