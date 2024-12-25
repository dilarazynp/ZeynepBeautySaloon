using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabaný baðlantýsý ekleniyor
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Oturum hizmetini ekleyin
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true; // Güvenlik için çerez sadece HTTP üzerinden okunabilir
    options.Cookie.IsEssential = true; // GDPR uyumluluðu için gerekli
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Çerezler yalnýzca HTTPS üzerinden gönderilir
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Uye/Login";
        options.LogoutPath = "/Uye/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS zorunluluðu
    });

// IHttpContextAccessor servisini ekleyin
builder.Services.AddHttpContextAccessor();

// Admin yetkilendirme politikasý ekleniyor
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin", "true"));
});

// Loglama servisi (isteðe baðlý dosya tabanlý loglama eklenebilir)
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    // Eðer dosya tabanlý loglama istersen:
    // config.AddFile("Logs/myapp-{Date}.txt");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HTTPS için zorunlu
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();
app.UseAuthentication(); // Authentication middleware
app.UseAuthorization();

// Varsayýlan rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();