using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZeynepBeautySaloon.Models
{
    public class Randevu
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Randevu tarihi gereklidir.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Randevu saati gereklidir.")]
        public TimeSpan Saat { get; set; }

        [Required(ErrorMessage = "Müşteri adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Müşteri adı en fazla 100 karakter olabilir.")]
        public string MusteriAdi { get; set; }

        [Required(ErrorMessage = "Müşteri telefon numarası gereklidir.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string MusteriTelefon { get; set; }

        [ForeignKey("Personel")]
        [Required(ErrorMessage = "Personel seçimi gereklidir.")]
        public int PersonelId { get; set; }
        public virtual Personel? Personel { get; set; }

        [ForeignKey("Islemler")]
        [Required(ErrorMessage = "İşlem seçimi gereklidir.")]
        public int IslemId { get; set; }
        public virtual Islemler? Islem { get; set; }

        [Required]
        public decimal Ucret { get; set; } // İşlem ücreti

        [Required]
        public TimeSpan Sure { get; set; } // İşlem süresi

        public bool OnayDurumu { get; set; }  // Admin tarafından false olarak başlayacak, onaylandığında true yapılacak.
    }
}
