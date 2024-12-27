using System.ComponentModel.DataAnnotations;

namespace ZeynepBeautySaloon.Models
{
    public class Personel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        public string Uzmanlik { get; set; }

        [Required(ErrorMessage = "Müsaitlik durumu zorunludur.")]
        public bool MusaitlikDurumu { get; set; }

        [Required(ErrorMessage = "Cinsiyet alanı zorunludur.")]
        public string Cinsiyet { get; set; }

        public string? FotografUrl { get; set; } // İsteğe bağlı

        public virtual ICollection<Islemler>? Islemler { get; set; } // Personelin yapabildiği işlemler

        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}