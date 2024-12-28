using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Data;
using ZeynepBeautySaloon.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace ZeynepBeautySaloon.Controllers
{
    public class PersonelController : Controller
    {
        private readonly AppDbContext _context;

        public PersonelController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Personeller.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .FirstOrDefaultAsync(m => m.Id == id);//linq
            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ad,Soyad,Uzmanlik,MusaitlikDurumu,Cinsiyet,FotografUrl")] Personel personel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(personel.FotografUrl))
                {
                    personel.FotografUrl = await GetRandomPhotoUrl(personel.Cinsiyet);
                }

                _context.Add(personel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller.FindAsync(id);
            if (personel == null)
            {
                return NotFound();
            }
            return View(personel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,Uzmanlik,MusaitlikDurumu,Cinsiyet,FotografUrl")] Personel personel)
        {
            if (id != personel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonelExists(personel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(personel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personel = await _context.Personeller.FindAsync(id);
            if (personel == null)
            {
                return NotFound();
            }

            // Personel'e bağlı randevular var mı kontrol et // LINQ
            var hasAppointments = await _context.Appointments.AnyAsync(a => a.PersonelId == id);
            if (hasAppointments)
            {
                TempData["Error"] = "Bu personel silinemez çünkü bağlı randevuları var.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Personeller.Remove(personel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonelExists(int id)
        {
            return _context.Personeller.Any(e => e.Id == id);
        }

        private async Task<string> GetRandomPhotoUrl(string cinsiyet)
        {
            using (var client = new HttpClient())
            {
                var genderQuery = cinsiyet.ToLower() == "erkek" ? "male" : "female";
                var response = await client.GetAsync($"https://randomuser.me/api/?gender={genderQuery}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(json);
                    var photoUrl = data["results"]?[0]?["picture"]?["large"]?.ToString();
                    return photoUrl ?? "https://via.placeholder.com/150";
                }
                return "https://via.placeholder.com/150";
            }
        }
    }
}