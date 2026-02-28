using ClickGuard.Admin.Data;
using ClickGuard.Admin.Models;
using ClickGuard.Admin.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClickGuard.Admin.Pages;

public class InviteModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public InviteModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Token { get; set; } = "";

    public string? InviteEmail { get; set; }
    public bool Valid { get; set; }

    [BindProperty]
    public string Password { get; set; } = "";

    [BindProperty]
    public string ConfirmPassword { get; set; } = "";

    public async Task OnGetAsync()
    {
        var invite = await FindInviteAsync(Token);
        Valid = invite != null;
        InviteEmail = invite?.Email;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var invite = await FindInviteAsync(Token);
        if (invite == null)
        {
            ModelState.AddModelError("", "Invalid or already-used invite.");
            Valid = false;
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Password) || Password != ConfirmPassword)
        {
            ModelState.AddModelError("", "Passwords must match.");
            Valid = true;
            InviteEmail = invite.Email;
            return Page();
        }

        // Transaction prevents double-use under race conditions
        await using var tx = await _db.Database.BeginTransactionAsync();

        // Re-check inside transaction (important)
        invite = await _db.TenantInvites
            .Where(i => i.Id == invite.Id && i.UsedAt == null)
            .FirstOrDefaultAsync();

        if (invite == null)
        {
            await tx.RollbackAsync();
            ModelState.AddModelError("", "Invite was just used. Please request a new one.");
            Valid = false;
            return Page();
        }

        // Create user
        var user = new ApplicationUser
        {
            UserName = invite.Email,
            Email = invite.Email,
            EmailConfirmed = true,
            TenantId = invite.TenantId
        };

        var result = await _userManager.CreateAsync(user, Password);
        if (!result.Succeeded)
        {
            await tx.RollbackAsync();
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            Valid = true;
            InviteEmail = invite.Email;
            return Page();
        }

        // Assign role within tenant context (basic: app role)
        if (!string.IsNullOrWhiteSpace(invite.Role))
            await _userManager.AddToRoleAsync(user, invite.Role);

        // Mark used (one-time)
        invite.UsedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        await tx.CommitAsync();

        return RedirectToPage("/Account/Login", new { area = "Identity" });
    }

    private async Task<TenantInvite?> FindInviteAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        var tokenHash = TokenService.Sha256(token);

        return await _db.TenantInvites
            .AsNoTracking()
            .Where(i => i.TokenHash == tokenHash && i.UsedAt == null)
            .FirstOrDefaultAsync();
    }
}