using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClickGuard.Admin.Models
{
    public class PolicySnapshot
    {
        public int Id { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        public int Version { get; set; }

        [Required]
        public string PolicyJson { get; set; }

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}