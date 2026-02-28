using ClickGuard.Admin.Data;
using ClickGuard.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClickGuard.Admin.Pages;

[Authorize(Roles = "Admin,ReadOnly")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

	public ClickGuard.Admin.Models.Tenant Tenant { get; set; }
    public int UserCount { get; set; }
    public int TrustedDomainCount { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null || user.TenantId == Guid.Empty)
            return;

        Tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == user.TenantId);

        UserCount = await _db.Users
            .CountAsync(u => u.TenantId == user.TenantId);

        TrustedDomainCount = await _db.TrustedDomains
            .CountAsync(d => d.TenantId == user.TenantId);
    }
}