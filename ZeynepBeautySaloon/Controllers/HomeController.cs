using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ZeynepBeautySaloon.Models;

namespace ZeynepBeautySaloon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View(); 
            }

            
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole == "Admin")
            {
                return RedirectToAction("AdminPanel", "Home");
            }
            else if (userRole == "User")
            {
                return RedirectToAction("KullaniciPanel", "Home");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AdminPanel()
        {
           
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                TempData["msj"] = "Bu iþlemi yalnýzca adminler yapabilir.";
                return RedirectToAction("Index", "Home");  
            }

           
            return View(); 
        }


        public IActionResult KullaniciPanel()
        {
           
            if (HttpContext.Session.GetString("Role") != "User")
            {
                TempData["msj"] = "Bu iþlem sadece kullanýcýlar içindir.";
                return RedirectToAction("Index", "Home"); 
            }

           
            return View(); 
        }

    }
}