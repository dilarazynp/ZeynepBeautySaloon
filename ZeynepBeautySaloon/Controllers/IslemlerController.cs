using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Models;

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

        public IActionResult Create()
        {
            ViewData["PersonelId"] = new SelectList(
                _context.Personeller.Where(p => p.Durum).ToList(),
                "Id",
                "Ad"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Islemler islemler)
        {
            if (ModelState.IsValid)
            {
                _context.Add(islemler);
                _context.SaveChanges();
                TempData["msj"] = "İşlem başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            TempData["msj"] = "Hata! İşlem eklenemedi.";
            return View(islemler);
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var islem = await _context.Islemler.FindAsync(id);
            if (islem == null) return NotFound();

            ViewData["PersonelId"] = new SelectList(_context.Personeller, "Id", "Ad", islem.PersonelId);
            return View(islem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Islemler islem)
        {
            if (id != islem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
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
            return View(islem);
        }

        public IActionResult Delete(int id)
        {
            var islem = _context.Islemler.Include(i => i.Personel).FirstOrDefault(i => i.Id == id);
            if (islem == null) return NotFound();

            return View(islem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var islem = _context.Islemler.Find(id);
            if (islem != null)
            {
                _context.Islemler.Remove(islem);
                _context.SaveChanges();
                TempData["msj"] = "İşlem başarıyla silindi.";
            }
            else
            {
                TempData["msj"] = "Silme işlemi başarısız.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool IslemExists(int id)
        {
            return _context.Islemler.Any(e => e.Id == id);
        }
    }
}
