using System.ComponentModel.DataAnnotations;

namespace ZeynepBeautySaloon.Models
{
    public class Uye
    {

        public int Id { get; set; }

        [Display(Name = "Adınız")]
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [MaxLength(30, ErrorMessage = "Ad en fazla 30 karakter olabilir.")]
        public string Ad { get; set; }

        [Display(Name = "Soyadınız")]
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [MaxLength(30, ErrorMessage = "Soyad en fazla 30 karakter olabilir.")]
        public string Soyad { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı adı alanı zorunludur.")]
        [MaxLength(20, ErrorMessage = "Kullanıcı adı en fazla 30 karakter olabilir.")]
        public string UserName { get; set; }
        
        [Display(Name = "Cinsiyet")]
        [Required(ErrorMessage = "Cinsiyet alanı zorunludur.")]
        public string Cinsiyet { get; set; }

        [Display(Name = "E-posta Adresi")]
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Display(Name = "Telefon No")]
        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string Telefon { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W).{8,}$", ErrorMessage = "Şifre en az 8 karakter uzunluğunda olmalı, bir büyük harf ve bir özel karakter içermelidir.")]
        public string PasswordHash { get; set; }

        public string Rol { get; set; } = "User"; 


    }
}
