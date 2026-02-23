using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClickGuard.Admin.Models;

namespace ClickGuard.Admin.Data
{
    public class ApplicationDbContext 
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TenantInvite> TenantInvites { get; set; }
		public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TrustedDomain> TrustedDomains { get; set; }
        public DbSet<PolicySnapshot> PolicySnapshots { get; set; }
    }
}