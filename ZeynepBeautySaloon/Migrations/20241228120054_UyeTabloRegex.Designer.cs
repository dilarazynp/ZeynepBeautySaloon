﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZeynepBeautySaloon.Data;

#nullable disable

namespace ZeynepBeautySaloon.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241228120054_UyeTabloRegex")]
    partial class UyeTabloRegex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IslemId")
                        .HasColumnType("int");

                    b.Property<bool>("OnayDurumu")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<int>("PersonelId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Saat")
                        .HasColumnType("time");

                    b.Property<DateTime>("Tarih")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Ucret")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(18,2)")
                        .HasDefaultValue(0m);

                    b.Property<int>("UyeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IslemId");

                    b.HasIndex("PersonelId");

                    b.HasIndex("UyeId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Islemler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("IslemAdi")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("PersonelId")
                        .HasColumnType("int");

                    b.Property<int>("Sure")
                        .HasColumnType("int");

                    b.Property<decimal>("Ucret")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("PersonelId");

                    b.ToTable("Islemler");
                });

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Personel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cinsiyet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FotografUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("MusaitlikDurumu")
                        .HasColumnType("bit");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Uzmanlik")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Personeller");
                });

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Uye", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Cinsiyet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Telefon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("Uyeler");
                });

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Appointment", b =>
                {
                    b.HasOne("ZeynepBeautySaloon.Models.Islemler", "Islem")
                        .WithMany("Appointments")
                        .HasForeignKey("IslemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZeynepBeautySaloon.Models.Personel", "Personel")
                        .WithMany("Appointments")
                        .HasForeignKey("PersonelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZeynepBeautySaloon.Models.Uye", "Uye")
                        .WithMany("Appointments")
                        .HasForeignKey("UyeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Islem");

                    b.Navigation("Personel");

                    b.Navigation("Uye");
                });

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Islemler", b =>
                {
                    b.HasOne("ZeynepBeautySaloon.Models.Personel", "Personel")
                        .WithMany("Islemler")
                        .HasForeignKey("PersonelId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Personel");
                });

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Islemler", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Personel", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Islemler");
                });

            modelBuilder.Entity("ZeynepBeautySaloon.Models.Uye", b =>
                {
                    b.Navigation("Appointments");
                });
#pragma warning restore 612, 618
        }
    }
}
