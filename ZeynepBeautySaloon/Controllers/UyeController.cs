using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZeynepBeautySaloon.Models;
using BCrypt.Net;

namespace ZeynepBeautySaloon.Controllers
{
    public class UyeController : Controller
    {
        private readonly AppDbContext _context;

       
        private readonly string AdminEmail = Environment.GetEnvironmentVariable("AdminEmail") ?? "admin@admin.com";
        private readonly string AdminPassword = Environment.GetEnvironmentVariable("AdminPassword") ?? BCrypt.Net.BCrypt.HashPassword("sau");

        public UyeController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Uye uye)
        {
            if (ModelState.IsValid)
            {
               
                if (_context.Uyeler.Any(u =>
                    u.UserName == uye.UserName))
                {
                    ModelState.AddModelError("UserName", "Kullanıcı adı zaten kayıtlı.");
                }

                if (_context.Uyeler.Any(u =>
                    u.Email == uye.Email))
                {
                    ModelState.AddModelError("Email", "E-posta zaten kayıtlı.");
                }

                if (_context.Uyeler.Any(u =>
                    u.Telefon == uye.Telefon))
                {
                    ModelState.AddModelError("Telefon", "Telefon numarası zaten kayıtlı.");
                }

                
                if (!ModelState.IsValid)
                {
                    return View(uye);
                }

               
                uye.PasswordHash = BCrypt.Net.BCrypt.HashPassword(uye.PasswordHash);

                
                uye.Rol = "User";

                
                _context.Uyeler.Add(uye);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Üye olma işlemi başarılı! Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            return View(uye);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            
            if (Email == AdminEmail && BCrypt.Net.BCrypt.Verify(Password, AdminPassword))
            {
                await SignInUser("Admin", "Admin");
                return RedirectToAction("Index", "Home");
            }

           
            var uye = _context.Uyeler.SingleOrDefault(u => u.Email == Email);

            if (uye != null && BCrypt.Net.BCrypt.Verify(Password, uye.PasswordHash))
            {
                await SignInUser(uye.UserName, uye.Rol);
                return RedirectToAction("Index", "Home");
            }

          
            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(string userName, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}