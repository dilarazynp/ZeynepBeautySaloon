namespace ZeynepBeautySaloon.Models
{
    public class Personel
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Uzmanlik { get; set; }

        public bool Durum { get; set; }
        public string FotografUrl { get; set; }

        public virtual ICollection<Islemler>? Islemler { get; set; }

        public virtual ICollection<Randevu>? Randevular { get; set; }

    }
}