using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Models;
using Microsoft.AspNetCore.Authorization;
using ZeynepBeautySaloon.Data;

namespace ZeynepBeautySaloon.Controllers
{
    [Authorize(Roles = "Admin")] // Tüm metodlar için yetkilendirme
    public class PersonelController : Controller
    {
        private readonly AppDbContext _context;

        public PersonelController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var personeller = await _context.Personeller.ToListAsync();
            return View(personeller);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Personel personel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personel);
                _context.SaveChanges();
                TempData["msj"] = "Personel başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            TempData["msj"] = "Hata! Personel eklenemedi.";
            return View(personel);
        }

        public IActionResult PersonelDetay(int? id)
        {
            if (id is null)
            {
                TempData["msj"] = "Lütfen geçerli bir personel seçin.";
                return RedirectToAction("Index");
            }

            var personel = _context.Personeller.FirstOrDefault(p => p.Id == id);
            if (personel == null)
            {
                TempData["msj"] = "Personel bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(personel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var personel = await _context.Personeller.FindAsync(id);
            if (personel == null) return NotFound();

            return View(personel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Personel personel)
        {
            if (id != personel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personel);
                    await _context.SaveChangesAsync();
                    TempData["msj"] = "Personel başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonelExists(personel.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["msj"] = "Hata! Güncelleme yapılamadı.";
            return View(personel);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var personel = _context.Personeller.Find(id);
            if (personel == null) return NotFound();

            return View(personel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var personel = _context.Personeller.Find(id);
            if (personel != null)
            {
                _context.Personeller.Remove(personel);
                _context.SaveChanges();
                TempData["msj"] = "Personel başarıyla silindi.";
            }
            else
            {
                TempData["msj"] = "Silme işlemi başarısız.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PersonelExists(int id)
        {
            return _context.Personeller.Any(e => e.Id == id);
        }
    }
}