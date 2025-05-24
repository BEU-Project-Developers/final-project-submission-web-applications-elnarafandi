using Microsoft.AspNetCore.Mvc;

namespace Fashion.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
