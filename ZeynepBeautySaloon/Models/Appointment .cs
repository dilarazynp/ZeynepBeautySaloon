using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZeynepBeautySaloon.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Randevu tarihi gereklidir.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Randevu saati gereklidir.")]
        public TimeSpan Saat { get; set; }

        [ForeignKey("Personel")]
        [Required(ErrorMessage = "Personel seçimi gereklidir.")]
        public int PersonelId { get; set; }

        public virtual Personel Personel { get; set; }

        [ForeignKey("Islemler")]
        [Required(ErrorMessage = "İşlem seçimi gereklidir.")]
        public int IslemId { get; set; }

        public virtual Islemler Islem { get; set; }

        [ForeignKey("Uye")]
        [Required(ErrorMessage = "Üye bilgisi gereklidir.")]
        public int UyeId { get; set; }

        public virtual Uye Uye { get; set; }

        public decimal Ucret { get; set; } // İşlem ücreti, Islem.Ucret'ten alınacak

        public bool OnayDurumu { get; set; }
    }
}