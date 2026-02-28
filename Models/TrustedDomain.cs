using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClickGuard.Admin.Models
{
    public class TrustedDomain
    {
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string DomainPattern { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    }
}