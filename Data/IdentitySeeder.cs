using Microsoft.AspNetCore.Identity;

namespace ClickGuard.Admin.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, string fcsAdminEmail)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Models.ApplicationUser>>();

        const string roleName = "FcsAdmin";

        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName));

        var user = await userManager.FindByEmailAsync(fcsAdminEmail);
        if (user != null && !await userManager.IsInRoleAsync(user, roleName))
            await userManager.AddToRoleAsync(user, roleName);
    }
}