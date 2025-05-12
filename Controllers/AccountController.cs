using Microsoft.AspNetCore.Mvc;
using FSSA.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ProjectManagerMvc.Controllers
{
    public class AccountController : Controller
    {

        private readonly ProjectManagerContext _context;

        public AccountController(ProjectManagerContext context)
        {
            _context = context;
        }


        // Display login page from get request
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handle login form submission
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Find exist user with matching email and password
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {   
                // Create a list of user identity info
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("Role", user.Role ?? "")
                };

                // Create the identity and principal for cookie
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign the user in using cookie schme and redirect to home page
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }

            // Display error message if login fails
            ViewBag.Email = email;
            ViewBag.ErrorMessage = "Your password is incorrect";
            return View();
        
        }
    
    }

}