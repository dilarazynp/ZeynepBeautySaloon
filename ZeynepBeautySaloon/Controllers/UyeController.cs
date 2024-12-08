using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Models;
using BCrypt.Net;

namespace ZeynepBeautySaloon.Controllers
{
    public class UyeController : Controller
    {
        private readonly AppDbContext _context;

        // Constructor, veritabanı bağlamını alır
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
                // Kullanıcı adı, e-posta veya telefon numarası benzersiz mi kontrol et
                bool isExistingUser = _context.Uyeler.Any(u =>
                    u.UserName == uye.UserName ||
                    u.Email == uye.Email ||
                    u.Telefon == uye.Telefon);

                if (isExistingUser)
                {
                    ViewBag.Message = "Kullanıcı adı, e-posta veya telefon numarası zaten kayıtlı.";
                    return View(uye);
                }

                // Şifreyi hashle
                uye.PasswordHash = BCrypt.Net.BCrypt.HashPassword(uye.PasswordHash);

                // Veritabanına kaydet
                _context.Uyeler.Add(uye);
                _context.SaveChanges();

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
        public IActionResult Login(string Username, string Password)
        {
            var uye = _context.Uyeler.SingleOrDefault(u => u.UserName == Username);
            if (uye != null && BCrypt.Net.BCrypt.Verify(Password, uye.PasswordHash))
            {
                // Kullanıcı doğrulandı
                HttpContext.Session.SetString("UserName", uye.UserName);

                // Giriş yaptıktan sonra ana sayfaya yönlendir
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Kullanıcı adı veya şifre hatalı.";
            }
            return View();
        }


        [HttpGet]
        public IActionResult Logout()
        {
            // Kullanıcı oturumunu sonlandır
            HttpContext.Session.Remove("Username");

            // Ana sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }


    }
}
