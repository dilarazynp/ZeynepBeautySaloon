using System.ComponentModel.DataAnnotations;

namespace ZeynepBeautySaloon.Models
{
    public class Uye
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [MaxLength(30, ErrorMessage = "Ad en fazla 30 karakter olabilir.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [MaxLength(30, ErrorMessage = "Soyad en fazla 30 karakter olabilir.")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı alanı zorunludur.")]
        [MaxLength(20, ErrorMessage = "Kullanıcı adı en fazla 20 karakter olabilir.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Cinsiyet alanı zorunludur.")]
        public string Cinsiyet { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string Telefon { get; set; }

        [Required]
        public string Password { get; set; } // PasswordHash yerine Password

        public string Rol { get; set; } = "User";

        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}