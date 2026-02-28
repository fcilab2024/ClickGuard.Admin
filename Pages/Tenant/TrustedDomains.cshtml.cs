using ClickGuard.Admin.Data;
using ClickGuard.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClickGuard.Admin.Pages.Tenant;

[Authorize(Roles = "Admin")]
public class TrustedDomainsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TrustedDomainsModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public List<TrustedDomain> Domains { get; set; } = new();

    [BindProperty]
    public string NewDomain { get; set; } = "";

    public Guid TenantId { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        TenantId = user.TenantId;

        Domains = await _db.TrustedDomains
            .Where(d => d.TenantId == TenantId)
            .OrderBy(d => d.DomainPattern)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Index");

        if (string.IsNullOrWhiteSpace(NewDomain))
        {
            ModelState.AddModelError("", "Domain is required.");
            await OnGetAsync();
            return Page();
        }

        var domain = new TrustedDomain
        {
            Id = Guid.NewGuid(),
            TenantId = user.TenantId,
            DomainPattern = NewDomain.Trim().ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        _db.TrustedDomains.Add(domain);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Index");

        var domain = await _db.TrustedDomains
            .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == user.TenantId);

        if (domain != null)
        {
            _db.TrustedDomains.Remove(domain);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}