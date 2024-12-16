using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using ZeynepBeautySaloon.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
    public IActionResult Create(Islemler islemler)
    {

        _context.Add(islemler);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));


    }


    public IActionResult IslemDetay(int? id)
    {
        if (id is null)
        {
            TempData["msj"] = "lütfen dataları düzgün giriniz";
            return RedirectToAction("index");
        }
        var islem = _context.Islemler.Include(i => i.Personel).FirstOrDefault(i => i.Id == id);
        if (islem is null)
        {
            TempData["msj"] = "islem bulunamadı";
            return RedirectToAction("index");
        }
        return View();
    }




    private bool IslemExists(int id)
    {
        return _context.Islemler.Any(e => e.Id == id);
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

        if (id != islem.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(islem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IslemExists(islem.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["PersonelId"] = new SelectList(_context.Personeller, "Id", "Ad", islem.PersonelId);

        return View(islem);
    }


    public IActionResult Delete(int id)
    {
        var islem = _context.Islemler.Include(i => i.Personel).FirstOrDefault(i => i.Id == id);

        if (islem == null)
        {
            return NotFound();
        }

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
        }

        return RedirectToAction(nameof(Index));
    }

}