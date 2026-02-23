using Microsoft.AspNetCore.Identity;

namespace ClickGuard.Admin.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid TenantId { get; set; }
    }
}