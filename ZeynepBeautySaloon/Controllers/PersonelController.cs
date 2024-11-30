using Microsoft.AspNetCore.Mvc;
using ZeynepBeautySaloon.Migrations; // AppDbContext sınıfının bulunduğu namespace
using ZeynepBeautySaloon.Models; // Personel model sınıfının bulunduğu namespace
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ZeynepBeautySaloon.Controllers
{
    public class PersonelController : Controller
    {
        private readonly AppDbContext _context;

        // Constructor, veritabanı bağlamını alır
        public PersonelController(AppDbContext context)
        {
            _context = context;
        }

        // Personel listesini görüntülemek için GET aksiyonu
        public async Task<IActionResult> Index()
        {
            // Personeller tablosundaki tüm verileri çekiyoruz
            var personeller = await _context.Personeller.ToListAsync();
            return View(personeller); // View'e Personeller verisini gönderiyoruz
        }
    }
}
