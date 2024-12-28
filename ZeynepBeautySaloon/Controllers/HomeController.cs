using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ZeynepBeautySaloon.Data;
using ZeynepBeautySaloon.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ZeynepBeautySaloon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> AdminPanel()
        //{
        //    if (HttpContext.Session.GetString("Role") != "Admin")
        //    {
        //        TempData["msj"] = "Bu iþlemi yalnýzca adminler yapabilir.";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}