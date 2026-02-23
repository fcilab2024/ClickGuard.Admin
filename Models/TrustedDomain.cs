using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClickGuard.Admin.Models
{
    public class TrustedDomain
    {
        public int Id { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        [Required]
        [MaxLength(500)]
        public string DomainPattern { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}