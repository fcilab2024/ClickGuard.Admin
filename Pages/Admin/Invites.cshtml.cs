using ClickGuard.Admin.Data;
using ClickGuard.Admin.Models;
using ClickGuard.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClickGuard.Admin.Pages.Admin;

[Authorize(Roles = "FcsAdmin")]
public class InvitesModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public InvitesModel(ApplicationDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = default!;
    public List<TenantInvite> Invites { get; set; } = new();

    [BindProperty]
    public string Email { get; set; } = "";

    [BindProperty]
    public string Role { get; set; } = "Admin"; // Admin / ReadOnly

    public string? NewInviteLink { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (TenantId == Guid.Empty) return BadRequest("tenantId is required.");

        Tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == TenantId)
            ?? throw new InvalidOperationException("Tenant not found.");

        Invites = await _db.TenantInvites
            .Where(i => i.TenantId == TenantId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (TenantId == Guid.Empty) return BadRequest("tenantId is required.");

        if (string.IsNullOrWhiteSpace(Email))
        {
            ModelState.AddModelError("", "Email is required.");
            return await OnGetAsync();
        }

        // Generate token
        var rawToken = TokenService.NewToken();
        var tokenHash = TokenService.Sha256(rawToken);

        var invite = new TenantInvite
        {
            TenantId = TenantId,
            Email = Email.Trim().ToLowerInvariant(),
            Role = string.IsNullOrWhiteSpace(Role) ? "Admin" : Role.Trim(),
            TokenHash = tokenHash,
            UsedAt = null
        };

        _db.TenantInvites.Add(invite);
        await _db.SaveChangesAsync();

        // Build invite link (uses current host)
        var link = Url.PageLink("/Invite", values: new { token = rawToken }, protocol: Request.Scheme);
        NewInviteLink = link;

        // Reload list + tenant
        await OnGetAsync();
        return Page();
    }
}