using Microsoft.AspNetCore.Identity;

namespace SchoolJournal.Data;

/// <summary>Создание ролей и учётной записи администратора (идемпотентно).</summary>
public static class IdentitySeeder
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";

    public const string AdminEmail = "admin@school.local";
    public const string AdminPassword = "Admin123!";

    public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { AdminRole, UserRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = AdminEmail, Email = AdminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, AdminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Не удалось создать администратора: " + string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }
}
