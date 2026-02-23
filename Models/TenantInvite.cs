using System.ComponentModel.DataAnnotations;

namespace ClickGuard.Admin.Models;

public class TenantInvite
{
    public int Id { get; set; }

    [Required] public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = default!;

    [Required, MaxLength(256)] public string Email { get; set; } = "";
    [Required, MaxLength(128)] public string TokenHash { get; set; } = "";

    [Required, MaxLength(32)] public string Role { get; set; } = "Admin"; // Admin / ReadOnly

    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}