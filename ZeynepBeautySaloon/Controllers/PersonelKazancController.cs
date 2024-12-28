using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Data;
using ZeynepBeautySaloon.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class PersonelKazancController : Controller
{
    private readonly AppDbContext _context;

    public PersonelKazancController(AppDbContext context)
    {
        _context = context;
    }

    // MVC: Admin Paneli için view döndürme
    public async Task<IActionResult> Index()
    {
        try
        {
            var now = DateTime.Now; // Şu anki zaman

            // LINQ sorgusu
            var kazancListesi = await _context.Appointments
                .Include(a => a.Personel)
                .Include(a => a.Islem)
                .Where(a => a.OnayDurumu == true &&
                            a.Tarih <= now.Date &&
                            a.Saat <= now.TimeOfDay) // Tarih ve saat kontrolü
                .GroupBy(a => a.PersonelId)
                .ToListAsync();

            var result = kazancListesi
                .Select(g =>
                {
                    var firstAppointment = g.FirstOrDefault();
                    return new PersonelKazancDto
                    {
                        PersonelId = g.Key,
                        PersonelAdi = firstAppointment?.Personel != null
                            ? $"{firstAppointment.Personel.Ad} {firstAppointment.Personel.Soyad}"
                            : "Bilinmeyen Personel",
                        ToplamKazanc = g.Sum(a => a.Ucret)
                    };
                })
                .ToList();

            return View(result);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
            return View("Error"); // Hata görünümüne yönlendir
        }
    }


    // API: Personel Kazançlarını almak için endpoint
    [HttpGet("api/personelkazanc")]
    public async Task<ActionResult<IEnumerable<PersonelKazancDto>>> GetPersonelKazanc()
    {
        try
        {
            var now = DateTime.Now; // Şu anki zaman

            // Tarih ve saat karşılaştırması için ayrı bir sütun kullanmayı düşünün
            var kazancListesi = await _context.Appointments
                .Include(a => a.Personel)
                .Include(a => a.Islem)
                .Where(a => a.OnayDurumu == true &&
                            a.Tarih <= now.Date &&
                            a.Saat <= now.TimeOfDay) // Tarih ve saat kontrolü
                .GroupBy(a => a.PersonelId)
                .ToListAsync();

            var result = kazancListesi
                .Select(g =>
                {
                    var firstAppointment = g.FirstOrDefault();
                    return new PersonelKazancDto
                    {
                        PersonelId = g.Key,
                        PersonelAdi = firstAppointment?.Personel != null
                            ? $"{firstAppointment.Personel.Ad} {firstAppointment.Personel.Soyad}"
                            : "Bilinmeyen Personel",
                        ToplamKazanc = g.Sum(a => a.Ucret)
                    };
                })
                .ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
        }
    }

}
