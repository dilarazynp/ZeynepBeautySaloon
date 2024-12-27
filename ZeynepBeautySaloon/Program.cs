using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ZeynepBeautySaloon.Data;

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
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // �erezler yaln�zca HTTPS �zerinden g�nderilir
});

// Kimlik do�rulama hizmetini ekleyin
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Uye/Login";
        options.LogoutPath = "/Uye/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS zorunlulu�u
    });

// IHttpContextAccessor servisini ekleyin
builder.Services.AddHttpContextAccessor();

// Admin yetkilendirme politikas� ekleniyor
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin", "true"));
});

// Loglama servisi (iste�e ba�l� dosya tabanl� loglama eklenebilir)
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    // E�er dosya tabanl� loglama istersen:
    // config.AddFile("Logs/myapp-{Date}.txt");
});

builder.Services.AddHttpClient("HairstyleClient", client =>
{
    client.BaseAddress = new Uri("https://hairstyle-changer.p.rapidapi.com/");
    client.DefaultRequestHeaders.Add("x-rapidapi-key", "1891f2f6bdmshf4a939ef30e048bp15aa29jsncb5b3698e673"); // G�NCEL API ANAHTARINIZI BURAYA YAZIN
    client.DefaultRequestHeaders.Add("x-rapidapi-host", "hairstyle-changer.p.rapidapi.com");
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
    app.UseHsts(); // HTTPS i�in zorunlu
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy(); // Cookie policy middleware

app.UseSession();

app.UseRouting();
app.UseAuthentication(); // Authentication middleware
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();