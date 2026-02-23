using System.ComponentModel.DataAnnotations;

namespace ClickGuard.Admin.Models
{
    public class Tenant
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public ICollection<TrustedDomain> TrustedDomains { get; set; }
    }
}