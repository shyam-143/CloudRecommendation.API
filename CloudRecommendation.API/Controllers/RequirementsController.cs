using System.Security.Claims;
using CloudRecommendation.Api.Data;
using CloudRecommendationApi.Models;
using CloudRecommendation.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudRecommendationApi.Controllers;

[ApiController]
[Authorize]
[Route("api/requirements")]
public class RequirementsController : ControllerBase
{
    private readonly AppDbContext _db;

    public RequirementsController(AppDbContext db)
    {
        _db = db;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    // POST api/requirements - save a new questionnaire submission
    [HttpPost]
    public async Task<ActionResult<RequirementResponseDto>> Create([FromBody] RequirementCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var requirement = new Requirement
        {
            UserId = CurrentUserId,
            WorkloadType = dto.WorkloadType,
            ExpectedUsage = dto.ExpectedUsage,
            ComplianceNeeds = dto.ComplianceNeeds,
            CpuCores = dto.CpuCores,
            RamGb = dto.RamGb,
            StorageGb = dto.StorageGb,
            PreferredRegion = dto.PreferredRegion,
            MonthlyBudget = dto.MonthlyBudget
        };

        _db.Requirements.Add(requirement);
        _db.AuditLogs.Add(new AuditLog { UserId = CurrentUserId, Action = "RequirementCreated", Details = $"Id={requirement.Id}" });
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = requirement.Id }, ToDto(requirement));
    }

    // GET api/requirements - list current user's saved searches (history)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RequirementResponseDto>>> GetAll()
    {
        var items = await _db.Requirements
            .Where(r => r.UserId == CurrentUserId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => ToDto(r))
            .ToListAsync();

        return Ok(items);
    }

    // GET api/requirements/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RequirementResponseDto>> GetById(int id)
    {
        var r = await _db.Requirements.FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
        if (r is null) return NotFound();
        return Ok(ToDto(r));
    }

    // DELETE api/requirements/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _db.Requirements.FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
        if (r is null) return NotFound();

        _db.Requirements.Remove(r);
        _db.AuditLogs.Add(new AuditLog { UserId = CurrentUserId, Action = "RequirementDeleted", Details = $"Id={id}" });
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static RequirementResponseDto ToDto(Requirement r) => new()
    {
        Id = r.Id,
        WorkloadType = r.WorkloadType,
        ExpectedUsage = r.ExpectedUsage,
        ComplianceNeeds = r.ComplianceNeeds,
        CpuCores = r.CpuCores,
        RamGb = r.RamGb,
        StorageGb = r.StorageGb,
        PreferredRegion = r.PreferredRegion,
        MonthlyBudget = r.MonthlyBudget,
        CreatedAt = r.CreatedAt
    };
}
