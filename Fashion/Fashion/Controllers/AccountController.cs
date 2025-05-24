using Fashion.Helpers.Enums;
using Fashion.Models;
using Fashion.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Fashion.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            AppUser appUser = new AppUser()
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.UserName
            };
            var result = await _userManager.CreateAsync(appUser, request.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(request);
            }
            await _userManager.AddToRoleAsync(appUser, Roles.Member.ToString());
            return RedirectToAction(nameof(Login));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            var existUser = await _userManager.FindByEmailAsync(request.EmailOrUsername);
            if (existUser == null)
            {
                existUser = await _userManager.FindByNameAsync(request.EmailOrUsername);
            }
            if (existUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(request);
            }
            var result = await _userManager.CheckPasswordAsync(existUser, request.Password);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(request);
            }
            var res = await _signInManager.PasswordSignInAsync(existUser, request.Password, false, false);
            

            return RedirectToAction("Index", "Home");
        }
    }
}
