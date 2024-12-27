using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Models;
using Microsoft.AspNetCore.Authorization;
using ZeynepBeautySaloon.Data;

namespace ZeynepBeautySaloon.Controllers
{
    public class IslemlerController : Controller
    {
        private readonly AppDbContext _context;

        public IslemlerController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var islemler = _context.Islemler.Include(i => i.Personel).ToList();
            return View(islemler);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Personel listesini dropdown'a doldur, durumu uygun olanları getir
            ViewData["PersonelId"] = new SelectList(
                _context.Personeller.Where(p => p.MusaitlikDurumu).ToList(),
                "Id",
                "Ad" // Personel'in adı ve soyadı property'si varsa "AdSoyad" yapabilirsiniz
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Islemler islem)
        {
            if (ModelState.IsValid)
            {
                // İşlem oluşturulurken, seçilen PersonelId'ye göre Personel nesnesini ata
                if (islem.PersonelId.HasValue)
                {
                    islem.Personel = _context.Personeller.FirstOrDefault(p => p.Id == islem.PersonelId.Value);
                }

                _context.Add(islem);
                _context.SaveChanges();
                TempData["msj"] = "İşlem başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            // Hata durumunda Personel listesini tekrar doldur
            ViewData["PersonelId"] = new SelectList(
                _context.Personeller.Where(p => p.MusaitlikDurumu).ToList(),
                "Id",
                "Ad",
                islem.PersonelId
            );
            TempData["msj"] = "Hata! İşlem eklenemedi.";
            return View(islem);
        }

        public IActionResult IslemDetay(int? id)
        {
            if (id == null)
            {
                TempData["msj"] = "Lütfen geçerli bir işlem seçin.";
                return RedirectToAction(nameof(Index));
            }

            var islem = _context.Islemler.Include(i => i.Personel).FirstOrDefault(i => i.Id == id);
            if (islem == null)
            {
                TempData["msj"] = "İşlem bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(islem);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var islem = await _context.Islemler.FindAsync(id);
            if (islem == null) return NotFound();

            ViewData["PersonelId"] = new SelectList(_context.Personeller.Where(p => p.MusaitlikDurumu), "Id", "Ad", islem.PersonelId);
            return View(islem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Islemler islem)
        {
            if (id != islem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // İşlem güncellenirken, Personel nesnesini de güncelle
                    if (islem.PersonelId.HasValue)
                    {
                        islem.Personel = _context.Personeller.FirstOrDefault(p => p.Id == islem.PersonelId.Value);
                    }
                    else
                    {
                        islem.Personel = null; // Personel atanmamışsa, Personel nesnesini null yap
                    }

                    _context.Update(islem);
                    await _context.SaveChangesAsync();
                    TempData["msj"] = "İşlem başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IslemExists(islem.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["msj"] = "Hata! Güncelleme yapılamadı.";
            ViewData["PersonelId"] = new SelectList(_context.Personeller.Where(p => p.MusaitlikDurumu), "Id", "Ad", islem.PersonelId);
            return View(islem);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var islem = _context.Islemler.Include(i => i.Personel).FirstOrDefault(i => i.Id == id);
            if (islem == null) return NotFound();

            return View(islem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var islem = _context.Islemler
                .Include(i => i.Appointments) // Randevuları include et
                .FirstOrDefault(i => i.Id == id);

            if (islem == null)
            {
                TempData["msj"] = "İşlem bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (islem.Appointments.Any())
            {
                TempData["msj"] = "Bu işlem silinemez çünkü ilişkili randevuları var.";
                return RedirectToAction(nameof(Index));
            }

            _context.Islemler.Remove(islem);
            _context.SaveChanges();
            TempData["msj"] = "İşlem başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }


        private bool IslemExists(int id)
        {
            return _context.Islemler.Any(e => e.Id == id);
        }
    }
}