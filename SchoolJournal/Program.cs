using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal;
using SchoolJournal.Data;
using SchoolJournal.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Внедрение зависимости для доступа к БД с использованием EF Core
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена.");
builder.Services.AddDbContext<SchoolJournalContext>(options => options.UseSqlServer(connectionString));

// ASP.NET Core Identity: пользователи и роли хранятся в той же БД
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SchoolJournalContext>();

// Профиль кэширования: вариант 3 -> 2*3 + 240 = 246 секунд
const int cacheSeconds = 2 * 3 + 240;

builder.Services.AddControllersWithViews(options =>
{
    options.CacheProfiles.Add(AppConstants.CacheProfileName, new CacheProfile
    {
        Duration = cacheSeconds,
        Location = ResponseCacheLocation.Any
    });
});
builder.Services.AddRazorPages();   // страницы входа/регистрации (Identity UI)

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Собственный middleware: миграции, тестовые данные, роли и администратор при первом запросе
app.UseDbInitializer();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
