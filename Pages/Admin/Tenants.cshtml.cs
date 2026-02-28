using ClickGuard.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClickGuard.Admin.Pages.Admin;

[Authorize(Roles = "FcsAdmin")]
public class TenantsModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public TenantsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<ClickGuard.Admin.Models.Tenant> Tenants { get; set; } = new();

    [BindProperty]
    public ClickGuard.Admin.Models.Tenant NewTenant { get; set; } = new();

    public async Task OnGetAsync()
    {
        Tenants = await _db.Tenants
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTenant.Name))
        {
            ModelState.AddModelError("", "Tenant name is required.");
            await OnGetAsync();
            return Page();
        }

        var tenant = new ClickGuard.Admin.Models.Tenant
        {
            Id = Guid.NewGuid(),
            Name = NewTenant.Name.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }
}