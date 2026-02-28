using ClickGuard.Admin.Models;
using Microsoft.AspNetCore.Identity;

namespace ClickGuard.Admin.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, string fcsAdminEmail)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure roles exist
        var roles = new[] { "FcsAdmin", "Admin", "ReadOnly" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Ensure FCS admin user has FcsAdmin role
        var user = await userManager.FindByEmailAsync(fcsAdminEmail);

        if (user != null && !await userManager.IsInRoleAsync(user, "FcsAdmin"))
        {
            await userManager.AddToRoleAsync(user, "FcsAdmin");
        }
    }
}