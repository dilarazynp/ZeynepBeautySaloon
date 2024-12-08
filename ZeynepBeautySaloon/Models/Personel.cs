namespace ZeynepBeautySaloon.Models
{
    public class Personel
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Uzmanlik { get; set; }
        public Islemler Islemler { get; set; }

        public bool Durum { get; set; }
        public string FotografUrl { get; set; }

    }
}
