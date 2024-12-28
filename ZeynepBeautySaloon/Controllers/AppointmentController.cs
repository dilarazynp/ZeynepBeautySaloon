using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZeynepBeautySaloon.Data;

namespace ZeynepBeautySaloon.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var currentUserId = HttpContext.Session.GetString("UserId");
            IQueryable<Appointment> randevular;
            bool isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                randevular = _context.Appointments
                    .Include(r => r.Islem)
                    .Include(r => r.Islem.Personel)
                    .Include(r => r.Uye);
            }
            else
            {
                randevular = _context.Appointments
                    .Include(r => r.Islem)
                    .Include(r => r.Islem.Personel)
                    .Where(r => r.UyeId == int.Parse(currentUserId));
            }

            return View(await randevular.ToListAsync());
        }


        
        public IActionResult Randevu()
        {

            ViewData["Personeller"] = new SelectList(_context.Personeller.Where(p => p.MusaitlikDurumu), "Id", "Ad");
            return View();
        }

        // Personel seçildiğinde o personelin işlemlerini getirir
        [Authorize(Roles = "User")]
        public JsonResult CalisandanServisleriAl(int calisanId)
        {
            var servisler = _context.Islemler
                                    .Where(s => s.PersonelId == calisanId)
                                    .Select(s => new { s.Id, s.IslemAdi, s.Ucret })
                                    .ToList();
            return Json(servisler);
        }

        // Personel ve tarih seçildiğinde o personelin o tarihteki uygun saatlerini getirir
        [Authorize(Roles = "User")]
        public JsonResult RandevuSaatleriniAl(int calisanId, DateTime tarih)
        {
            var calisan = _context.Personeller.FirstOrDefault(c => c.Id == calisanId);
            if (calisan == null) return Json(new List<string>());

            // O tarihteki randevuları al, sadece onaylı veya onaysız tüm randevular
            var doluSaatler = _context.Appointments
                                      .Where(r => r.PersonelId == calisanId && r.Tarih.Date == tarih.Date)
                                      .Select(r => r.Saat)
                                      .ToList();

            var randevuSaatleri = new List<string>();
            // 08:00 - 20:00 arası, 1 saatlik aralıklarla
            for (var saat = new TimeSpan(8, 0, 0); saat < new TimeSpan(20, 0, 0); saat = saat.Add(TimeSpan.FromHours(1)))
            {
                if (!doluSaatler.Contains(saat))
                {
                    randevuSaatleri.Add(saat.ToString(@"hh\:mm"));
                }
            }

            return Json(randevuSaatleri);
        }

        // POST: Randevu alma işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> RandevuAl(int PersonelId, int IslemId, DateTime Tarih, TimeSpan Saat)
        {
            // Session'dan UserId'yi al
            if (!int.TryParse(HttpContext.Session.GetString("UserId"), out int uyeId) && !User.IsInRole("Admin"))
            {
                // UserId alınamazsa, giriş yapmaya yönlendir
                TempData["msj"] = "Randevu alabilmek için giriş yapmalısınız.";
                return RedirectToAction("Login", "Uye");
            }

            // Geçmiş bir tarih ve saat kontrolü
            var selectedDateTime = Tarih.Add(Saat);
            if (selectedDateTime < DateTime.Now)
            {
                TempData["msj"] = "Geçmiş bir tarihe randevu alamazsınız.";
                return RedirectToAction("Randevu");
            }

            // Aynı gün ve saatte farklı personelden randevu kontrolü
            var existingAppointment = await _context.Appointments
                .AnyAsync(a => a.UyeId == uyeId && a.Tarih.Date == Tarih.Date && a.Saat == Saat);
            if (existingAppointment)
            {
                TempData["msj"] = "Aynı gün ve saatte farklı bir personelden randevu alamazsınız.";
                return RedirectToAction("Randevu");
            }

            // Seçilen işlem ve personel bilgisini al
            var islem = await _context.Islemler.Include(i => i.Personel).FirstOrDefaultAsync(i => i.Id == IslemId);

            if (islem == null || islem.Personel == null || islem.PersonelId != PersonelId)
            {
                TempData["msj"] = "İşlem veya personel bilgisi hatalı.";
                return RedirectToAction("Randevu");
            }

            // Randevu saati kontrolü (çakışma var mı?)
            var mevcutRandevu = await _context.Appointments.AnyAsync(a => a.PersonelId == PersonelId && a.Tarih.Date == Tarih.Date && a.Saat == Saat);
            if (mevcutRandevu)
            {
                TempData["msj"] = "Seçilen saatte başka bir randevu var.";
                return RedirectToAction("Randevu");
            }

            // Yeni randevu oluştur
            var yeniRandevu = new Appointment
            {
                PersonelId = PersonelId,
                IslemId = IslemId,
                Tarih = Tarih,
                Saat = Saat,
                UyeId = User.IsInRole("Admin") ? 0 : uyeId, // Admin için UyeId 0 olabilir
                Ucret = islem.Ucret,
                OnayDurumu = false
            };

            _context.Add(yeniRandevu);
            await _context.SaveChangesAsync();

            TempData["msj"] = "Randevu talebiniz alındı. Onay için bekleyiniz.";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Appointments
                .Include(r => r.Uye)
                .Include(r => r.Islem.Personel)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (randevu == null)
            {
                return NotFound();
            }

            // Kullanıcı sadece kendi randevusunu görebilir
            if (!User.IsInRole("Admin") && randevu.UyeId.ToString() != HttpContext.Session.GetString("UserId"))
            {
                TempData["msj"] = "Bu randevuyu görüntüleme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            return View(randevu);
        }

        // POST: Randevu/Onayla/5 (Admin için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Appointments.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            randevu.OnayDurumu = true;
            _context.Update(randevu);
            await _context.SaveChangesAsync();

            TempData["msj"] = "Randevu onaylandı.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Randevu/IptalEt/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var randevu = await _context.Appointments.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            // Kullanıcı sadece kendi randevusunu iptal edebilir
            if (!User.IsInRole("Admin") && randevu.UyeId.ToString() != HttpContext.Session.GetString("UserId"))
            {
                TempData["msj"] = "Bu randevuyu iptal etme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            _context.Appointments.Remove(randevu);
            await _context.SaveChangesAsync();

            TempData["msj"] = "Randevu iptal edildi.";
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}