using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZeynepBeautySaloon.Models;
using ZeynepBeautySaloon.Data;

namespace ZeynepBeautySaloon.Controllers
{
    public class UyeController : Controller
    {
        private readonly AppDbContext _context;

        // DİKKAT: AdminEmail ve AdminPassword'u Environment Variable yerine veritabanından almanız daha güvenli.
        private readonly string AdminEmail = Environment.GetEnvironmentVariable("AdminEmail") ?? "admin@admin.com";
        // DİKKAT: Güvenlik açığı! Admin şifresini bu şekilde tutmak çok tehlikeli!
        private readonly string AdminPassword = Environment.GetEnvironmentVariable("AdminPassword") ?? "sau";

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
                if (uye.Email == AdminEmail)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi kullanılamaz.");
                }

                if (_context.Uyeler.Any(u => u.UserName == uye.UserName))
                {
                    ModelState.AddModelError("UserName", "Kullanıcı adı zaten kayıtlı.");
                }

                if (_context.Uyeler.Any(u => u.Email == uye.Email))
                {
                    ModelState.AddModelError("Email", "E-posta zaten kayıtlı.");
                }

                if (_context.Uyeler.Any(u => u.Telefon == uye.Telefon))
                {
                    ModelState.AddModelError("Telefon", "Telefon numarası zaten kayıtlı.");
                }

                if (!ModelState.IsValid)
                {
                    return View(uye);
                }

                // DİKKAT: Şifre artık hash'lenmeden kaydediliyor!
                // uye.SetPassword(uye.PasswordHash); // Bu satırı kaldırıyoruz
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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            // ADMİN GİRİŞİ KONTROLÜ - BAŞLANGIÇ
            if (Email == AdminEmail)
            {
                // DİKKAT: AdminPassword değişkeni, HAM ŞİFREYİ DEĞİL, HASH'LENMİŞ ŞİFREYİ TUTMALI.
                if (Password == AdminPassword) // Güvenli Olmayan Karşılaştırma!
                {
                    await SignInUser(AdminEmail, "Admin");
                    return RedirectToAction("Index", "Home");
                }
            }
            // KULLANICI GİRİŞİ KONTROLÜ - GÜVENLİ DEĞİL!
            var uye = _context.Uyeler.SingleOrDefault(u => u.Email == Email);
            // DİKKAT: Şifre artık hash'lenmeden, doğrudan karşılaştırılıyor!
            if (uye != null && uye.Password == Password)
            {
                await SignInUser(uye.UserName, uye.Rol);
                HttpContext.Session.SetString("UserId", uye.Id.ToString());
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Cookies");
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

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        }
    }
}