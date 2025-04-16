using Microsoft.AspNetCore.Mvc;

namespace ProjectManagerMvc.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
            }
            
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }

}