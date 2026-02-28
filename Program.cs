using ClickGuard.Admin.Data;
using ClickGuard.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // Optional hardening (recommended)
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 4;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllers(); // <-- for /api + invite endpoints

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // <-- MISSssING in your current file (must be before Authorization)
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();    // <-- enables /api/* and /invite endpoints

await ClickGuard.Admin.Data.IdentitySeeder.SeedAsync(app.Services, "fernandocuervo08@hotmail.com");

app.Run();