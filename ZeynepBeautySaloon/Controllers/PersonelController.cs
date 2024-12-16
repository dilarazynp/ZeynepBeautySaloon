using Microsoft.AspNetCore.Mvc;
using ZeynepBeautySaloon.Migrations; 
using ZeynepBeautySaloon.Models; 
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            
            var personeller = await _context.Personeller.ToListAsync();
            return View(personeller); 
        }


        public IActionResult Create()
        {
            
            var personelListesi = _context.Personeller
                .Where(p => p.Durum == true) 
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Ad} {p.Soyad}" 
                }).ToList();

            
            ViewData["PersonelId"] = new SelectList(personelListesi, "Value", "Text");

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Personel personel)
        {

            _context.Add(personel);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));


        }

        public IActionResult PersonelDetay(int? id)
        {

            if (id is null)
            {
                TempData["msj"] = "Lütfen verileri düzgün giriniz.";
                return RedirectToAction("Index");
            }

            var personel = _context.Personeller.FirstOrDefault(i => i.Id == id);

            if (personel == null)
            {
                TempData["msj"] = "Personel bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(personel);
        }


        private bool PersonelExists(int id)
        {
            return _context.Personeller.Any(e => e.Id == id);
        }


        


       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            
            var personel = await _context.Personeller.FindAsync(id);

           
            if (personel == null) return NotFound();

           
            ViewData["PersonelId"] = new SelectList(_context.Personeller, "Id", "Ad", personel.Id);

            
            return View(personel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Personel personel)
        {
          
            if (id != personel.Id)
                return NotFound();

           
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
                        return NotFound();
                    else
                        throw;
                }
                
                return RedirectToAction(nameof(Index));
            }

            ViewData["PersonelId"] = new SelectList(_context.Personeller, "Id", "Ad", personel.Id);

            return View(personel);
        }


        public IActionResult Delete(int id)
        {
            var personel = _context.Personeller.FirstOrDefault(i => i.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

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
            }

            return RedirectToAction(nameof(Index));
        }


    }
}