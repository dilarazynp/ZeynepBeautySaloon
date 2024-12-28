using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZeynepBeautySaloon.Models;
using ZeynepBeautySaloon.Data;
using BCrypt.Net;

namespace ZeynepBeautySaloon.Controllers
{
    public class UyeController : Controller
    {
        private readonly AppDbContext _context;

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

                // Şifreyi hash'leyip kaydet
                uye.Password = BCrypt.Net.BCrypt.HashPassword(uye.Password);
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
            // KULLANICI GİRİŞİ KONTROLÜ
            var uye = _context.Uyeler.SingleOrDefault(u => u.Email == Email);
            // Şifreyi hash'leyip karşılaştır
            if (uye != null && BCrypt.Net.BCrypt.Verify(Password, uye.Password))
            {
                await SignInUser(uye.UserName, uye.Rol);
                HttpContext.Session.SetString("UserName", uye.UserName);
                HttpContext.Session.SetString("Role", uye.Rol);
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

        [HttpGet]
        public IActionResult KullaniciPanel()
        {
            if (HttpContext.Session.GetString("Role") != "User")
            {
                TempData["msj"] = "Bu işlem sadece kullanıcılar içindir.";
                return RedirectToAction("Index", "Home");
            }

            if (!int.TryParse(HttpContext.Session.GetString("UserId"), out int userId))
            {
                TempData["msj"] = "Geçersiz kullanıcı.";
                return RedirectToAction("Index", "Home");
            }

            var uye = _context.Uyeler.Find(userId);
            if (uye == null)
            {
                TempData["msj"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            return View(uye);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Uye uye)
        {
            if (!int.TryParse(HttpContext.Session.GetString("UserId"), out int userId))
            {
                TempData["ErrorMessage"] = "Güncelleme işlemi için giriş yapmalısınız.";
                return RedirectToAction("Login", "Uye");
            }

            if (userId != uye.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz kullanıcı.";
                return RedirectToAction("KullaniciPanel");
            }

            if (ModelState.IsValid)
            {
                // E-posta, kullanıcı adı ve telefon numarasının benzersizliğini kontrol et
                if (_context.Uyeler.Any(u => u.Email == uye.Email && u.Id != uye.Id))
                {
                    ModelState.AddModelError("Email", "E-posta zaten kayıtlı.");
                }

                if (_context.Uyeler.Any(u => u.UserName == uye.UserName && u.Id != uye.Id))
                {
                    ModelState.AddModelError("UserName", "Kullanıcı adı zaten kayıtlı.");
                }

                if (_context.Uyeler.Any(u => u.Telefon == uye.Telefon && u.Id != uye.Id))
                {
                    ModelState.AddModelError("Telefon", "Telefon numarası zaten kayıtlı.");
                }

                if (!ModelState.IsValid)
                {
                    // ModelState hatalarını TempData'ya ekleyelim
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                    TempData["ErrorMessage"] = "Güncelleme işlemi başarısız oldu. Lütfen bilgilerinizi kontrol edin.";
                    TempData["ModelStateErrors"] = errors;
                    return RedirectToAction("KullaniciPanel");
                }

                try
                {
                    // Şifreyi hash'leyip kaydet
                    var existingUser = await _context.Uyeler.AsNoTracking().FirstOrDefaultAsync(u => u.Id == uye.Id);
                    if (existingUser != null && existingUser.Password != uye.Password)
                    {
                        uye.Password = BCrypt.Net.BCrypt.HashPassword(uye.Password);
                    }

                    _context.Update(uye);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Bilgileriniz başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Uyeler.Any(e => e.Id == uye.Id))
                    {
                        TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                        return RedirectToAction("KullaniciPanel");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                // ModelState hatalarını TempData'ya ekleyelim
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                TempData["ErrorMessage"] = "Güncelleme işlemi başarısız oldu. Lütfen bilgilerinizi kontrol edin.";
                TempData["ModelStateErrors"] = errors;
            }

            return RedirectToAction("KullaniciPanel");
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