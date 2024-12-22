using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tabloya dönüştürülecek model sınıfları
    public DbSet<Personel> Personeller { get; set; }
    public DbSet<Uye> Uyeler { get; set; }
    public DbSet<Islemler> Islemler { get; set; }
    public DbSet<Randevu> Randevular { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Islemler>()
            .Property(i => i.Ucret)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Randevu>()
            .HasOne(r => r.Personel)
            .WithMany(p => p.Randevular)
            .HasForeignKey(r => r.PersonelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Randevu>()
            .HasOne(r => r.Islem)
            .WithMany(i => i.Randevular)
            .HasForeignKey(r => r.IslemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}