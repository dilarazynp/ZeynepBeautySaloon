using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Data;
using ZeynepBeautySaloon.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
// Bu sınıf hem API hem MVC işlemleri yapacak
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
        var kazancListesi = await _context.Appointments
            .Include(a => a.Personel)
            .Include(a => a.Islem)
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
                        ? firstAppointment.Personel.Ad + " " + firstAppointment.Personel.Soyad
                        : "Bilinmeyen Personel",
                    ToplamKazanc = g.Sum(a => a.Islem.Ucret)
                };
            })
            .ToList();

        return View(result);
    }

    // API: Personel Kazançlarını almak için endpoint
    [HttpGet("api/personelkazanc")]
    public async Task<ActionResult<IEnumerable<PersonelKazancDto>>> GetPersonelKazanc()
    {
        var kazancListesi = await _context.Appointments
            .Include(a => a.Personel)
            .Include(a => a.Islem)
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
                        ? firstAppointment.Personel.Ad + " " + firstAppointment.Personel.Soyad
                        : "Bilinmeyen Personel",
                    ToplamKazanc = g.Sum(a => a.Islem.Ucret)
                };
            })
            .ToList();

        return Ok(result);
    }
}
