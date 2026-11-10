using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Data;

namespace SchoolJournal.Middleware;

/// <summary>
/// Компонент middleware для инициализации базы данных.
/// При первом обращении к приложению применяет миграции, заполняет таблицы тестовыми данными,
/// создаёт роли и администратора, после чего передаёт запрос дальше по конвейеру.
/// Сервисы получает через внедрение зависимостей (параметры InvokeAsync).
/// </summary>
public class DbInitializerMiddleware
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private static bool _initialized;

    private readonly RequestDelegate _next;

    public DbInitializerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        SchoolJournalContext db,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if (!_initialized)
        {
            await Lock.WaitAsync(context.RequestAborted);
            try
            {
                if (!_initialized)
                {
                    await db.Database.MigrateAsync();                       // создать/обновить схему по миграциям
                    await DbInitializer.InitializeAsync(db);                // тестовые данные предметной области
                    await IdentitySeeder.SeedAsync(userManager, roleManager); // роли и администратор
                    _initialized = true;
                }
            }
            finally
            {
                Lock.Release();
            }
        }

        await _next(context);
    }
}
