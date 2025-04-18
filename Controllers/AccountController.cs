using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;

namespace ProjectManagerMvc.Controllers
{
    public class AccountController : Controller
    {

        private readonly ProjectManagerContext _context;

        public AccountController(ProjectManagerContext context)
        {
            _context = context;
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
            
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                return RedirectToAction("Index", "Home");
            }

            return View();
        
        }
    
    }

}