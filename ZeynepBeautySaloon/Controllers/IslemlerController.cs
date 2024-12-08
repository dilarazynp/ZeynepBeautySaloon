using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using ZeynepBeautySaloon.Models;
using Microsoft.EntityFrameworkCore;

public class IslemlerController : Controller
{
    private readonly AppDbContext _context;

    public IslemlerController(AppDbContext context)
    {
        _context = context;
    }

    // Kullanıcının işlemleri görüntülemesi
    public IActionResult Index()
    {
        var islemler = _context.Islemler.ToList();
        return View(islemler);
    }

    public IActionResult Create()
    {
        // Aktif personel listesi
        var personelListesi = _context.Personeller
            .Where(p => p.Durum == true) // Sadece aktif personel
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Ad} {p.Soyad}" // Personelin adı ve soyadı
            }).ToList();

        // Personel listesini ViewData'ya gönderiyoruz
        ViewData["PersonelId"] = new SelectList(personelListesi, "Value", "Text");

        return View();
    }

    // İşlem ekleme işlemi
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Islemler islemler)
    {
        
            _context.Add(islemler);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        
        
    }


    [HttpGet]
    public IActionResult Edit(int id)
    {
        var islemler = _context.Islemler.FirstOrDefault(i => i.Id == id);
        if (islemler == null)
        {
            return NotFound();
        }

        // Personel Listesi
        var personelListesi = _context.Personeller
            .Where(p => p.Durum == true) // Personel durumu aktif olmalı
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Ad} {p.Soyad}" // Personel adı soyadı ile görünecek
            }).ToList();

        ViewBag.PersonelId = new SelectList(personelListesi, "Value", "Text", islemler.PersonelId);

        return View(islemler); // Model ile View'a gönder
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Islemler islemler)
    {
        if (id != islemler.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(islemler); // Veritabanı güncelleniyor
                _context.SaveChanges();   // Değişiklikler kaydediliyor
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Islemler.Any(i => i.Id == islemler.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index)); // Güncelleme sonrası listeye yönlendir
        }

        // Eğer model geçerli değilse, personel listesini tekrar yükle
        var personelListesi = _context.Personeller
            .Where(p => p.Durum == true)
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Ad} {p.Soyad}"
            }).ToList();

        ViewBag.PersonelId = new SelectList(personelListesi, "Value", "Text", islemler.PersonelId);

        return View(islemler); // Modeli tekrar view'a gönder
    }


    // İşlem silme sayfası (GET)
    public IActionResult Delete(int id)
    {
        var islem = _context.Islemler.Include(i => i.Personel).FirstOrDefault(i => i.Id == id);

        if (islem == null)
        {
            return NotFound();
        }

        return View(islem);
    }

    // İşlem silme işlemi (POST)
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
