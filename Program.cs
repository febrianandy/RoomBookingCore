using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data; // Sesuaikan dengan namespace DbContext Anda
using RoomBookingCore.Services; // Pastikan namespace Service Anda di-import di sini
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
// 1. Daftarkan layanan Authentication dengan Cookie Scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Otomatis menendang ke sini jika belum login
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// 2. Daftarkan Razor Pages sekaligus proteksi global [Authorize]
builder.Services.AddRazorPages(options =>
{
    // Mengunci seluruh halaman agar wajib login
    options.Conventions.AuthorizeFolder("/");

    // Pengecualian: halaman login dibebaskan agar bisa diakses publik
    options.Conventions.AllowAnonymousToPage("/Login");
});

// 3. Konfigurasi Database MySQL menggunakan Pomelo.EntityFrameworkCore.MySql
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 4. DAFTARKAN SERVICE ANDA DI SINI (Agar bisa di-inject ke BookingFormModel)
builder.Services.AddScoped<BookingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 5. Urutan Middleware Autentikasi dan Otorisasi (WAJIB SEBELUM MapRazorPages)
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();