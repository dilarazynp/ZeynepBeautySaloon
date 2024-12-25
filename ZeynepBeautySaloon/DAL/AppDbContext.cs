using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Models;

namespace ZeynepBeautySaloon.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Islemler> Islemler { get; set; }
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Uye> Uyeler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                .Property(a => a.OnayDurumu)
                .HasDefaultValue(false);

            // Appointment: Ücret'in varsayılan değeri 0, Islem seçildiğinde güncellenecek
            modelBuilder.Entity<Appointment>()
                .Property(a => a.Ucret)
                .HasDefaultValue(0);

            // Islemler: İşlem ile Personel ilişkisi
            modelBuilder.Entity<Islemler>()
                .HasOne(i => i.Personel)
                .WithMany(p => p.Islemler)
                .HasForeignKey(i => i.PersonelId)
                .OnDelete(DeleteBehavior.SetNull); // Personel silindiğinde, Islemler.PersonelId null olsun


            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Islem)
                .WithMany(i => i.Appointments)
                .HasForeignKey(a => a.IslemId);

            // Appointment: Üye ile ilişki
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Uye)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UyeId);

            modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Personel)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PersonelId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
            .HasOne(r => r.Islem)
            .WithMany(i => i.Appointments)
            .HasForeignKey(r => r.IslemId)
            .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}