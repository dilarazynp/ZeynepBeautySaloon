using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZeynepBeautySaloon.Models
{
    public class Islemler
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İşlem adı gereklidir.")]
        [StringLength(100, ErrorMessage = "İşlem adı en fazla 100 karakter olabilir.")]
        public string IslemAdi { get; set; }

        [Required(ErrorMessage = "Süre gereklidir.")]
        [Range(1, 480, ErrorMessage = "Süre 1 ile 480 dakika arasında olmalıdır.")]
        public int Sure { get; set; }

        [Required(ErrorMessage = "Ücret gereklidir.")]
        [Range(1, 10000, ErrorMessage = "Ücret 1 ile 10,000 arasında olmalıdır.")]
        public decimal Ucret { get; set; }

        [ForeignKey("Personel")]
        public int? PersonelId { get; set; } // Nullable yapılabilir, işlem oluşturulurken personel atanmamış olabilir

        public virtual Personel? Personel { get; set; } // İşlemi yapan personel

        public virtual ICollection<Appointment>? Appointments { get; set; } // İşlemin yapıldığı randevular, nullable
    }
}