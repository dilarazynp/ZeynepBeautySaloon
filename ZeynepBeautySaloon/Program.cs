using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZeynepBeautySaloon.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritaban� ba�lant�s� ekleniyor
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Oturum hizmetini ekleyin
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
    options.Cookie.HttpOnly = true; // G�venlik i�in �erez sadece HTTP �zerinden okunabilir
    options.Cookie.IsEssential = true; // GDPR uyumlulu�u i�in gerekli
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();
app.UseAuthentication(); // E�er kimlik do�rulama varsa
app.UseAuthorization();

app.MapControllers();


// Yetkisiz giri�ler i�in y�nlendirme
app.UseStatusCodePagesWithRedirects("/Account/Login?returnUrl=/Admin");

// Varsay�lan rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
