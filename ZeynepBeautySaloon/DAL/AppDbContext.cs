﻿using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tabloya dönüştürülecek model sınıfları
    public DbSet<Personel> Personeller { get; set; }
    public DbSet<Uye> Uyeler { get; set; }
    public DbSet<Islemler> Islemler { get; set; }
}
