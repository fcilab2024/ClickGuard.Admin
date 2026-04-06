using ClickGuard.Admin.Data;
using ClickGuard.Admin.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClickGuard.Admin.Controllers;

[ApiController]
[Route("api/agent")]
public class AgentController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public AgentController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("allowlist")]
    public async Task<ActionResult<AgentAllowlistResponse>> GetAllowlist([FromQuery] Guid tenantId)
    {
        var domains = await _db.TrustedDomains
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.DomainPattern)
            .Select(x => x.DomainPattern)
            .ToListAsync();

        return Ok(new AgentAllowlistResponse
        {
            Domains = domains
        });
    }
}