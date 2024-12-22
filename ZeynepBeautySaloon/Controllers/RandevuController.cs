using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ZeynepBeautySaloon.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ZeynepBeautySaloon.Controllers
{
    public class RandevuController : Controller
    {
        private readonly AppDbContext _context;

        public RandevuController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Admin kullanıcı giriş yapmışsa randevu oluşturma sayfasına erişim engellenir
            if (User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Admin kullanıcılar randevu oluşturamaz.";
                return RedirectToAction("AdminList", "Randevu"); // Admin kullanıcıları admin randevu listesine yönlendirilir
            }

            // Normal kullanıcı giriş yapmışsa randevu oluşturma sayfası gösterilir
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.PersonelListesi = _context.Personeller.ToList();
                ViewBag.IslemListesi = _context.Islemler.ToList();
                return View();
            }
            else
            {
                TempData["ErrorMessage"] = "Randevu alabilmek için giriş yapmanız veya üye olmanız gerekmektedir.";
                return RedirectToAction("Login", "Uye"); // Burada giriş yapılmadığında login sayfasına yönlendirme yapılabilir
            }
        }
        [HttpPost]
        public IActionResult Create(Randevu randevu)
        {
            if (ModelState.IsValid)
            {
                // Randevu veritabanına ekleniyor
                randevu.OnayDurumu = false;  // Onay durumu varsayılan olarak false
                _context.Randevular.Add(randevu);
                _context.SaveChanges();

                // Başarı mesajı ayarlanıyor
                TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturulmuştur.";

                return RedirectToAction("Create");  // Sayfayı tekrar yönlendirerek mesajı gösterebiliriz
            }

            return View(randevu);  // Hata varsa aynı sayfayı tekrar göster
        }




        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult List()
        {
            var randevular = _context.Randevular
                .Where(r => r.MusteriTelefon == User.Identity.Name && r.OnayDurumu) // Kullanıcı sadece onaylanmış randevuları görsün
                .ToList();

            if (randevular.Count == 0)
            {
                TempData["InfoMessage"] = "Henüz onaylanmış bir randevunuz yok.";
            }

            return View(randevular);
        }


        // Randevu İptali
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Cancel(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null && randevu.MusteriTelefon == User.Identity.Name) // Kullanıcı sadece kendi randevusunu iptal edebilir
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevunuz başarıyla iptal edilmiştir.";
            }
            else
            {
                TempData["ErrorMessage"] = "Randevuyu iptal etme yetkiniz yok.";
            }
            return RedirectToAction("List"); // Listelenen randevulara geri yönlendir
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);

            if (randevu != null)
            {
                // Randevuyu onayla
                randevu.OnayDurumu = true; // Onay durumu true yapılır
                _context.Randevular.Update(randevu); // Veritabanı güncellenir

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevu talebiniz onaylanmıştır.";
            }
            else
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
            }

            return RedirectToAction("AdminList"); // Admin randevuları listesine yönlendirme
        }


        // Admin için randevu iptal etme
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelAdmin(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevu başarıyla iptal edilmiştir.";
            }
            else
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
            }
            return RedirectToAction("AdminList"); // Admin randevuları listesine yönlendirme
        }

        // Admin randevuları listele
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminList()
        {
            var randevular = _context.Randevular.ToList(); // Tüm randevuları listele
            return View(randevular);
        }
    }
}