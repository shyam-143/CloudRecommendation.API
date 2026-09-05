using CloudRecommendation.Api.Services;
using CloudRecommendation.API.Services;
using CloudRecommendationApi.DTOs;
using CloudRecommendation.Api.DTOs;
using CloudRecommendationApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CloudRecommendation.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationsController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateRecommendations([FromBody] RequirementCreateDto dto)
    {
        try
        {
            // 1. Extract User ID from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token." });
            }

            // 2. Map the incoming DTO to the Entity model for the service layer
            var requirement = new Requirement
            {
                WorkloadType = dto.WorkloadType,
                ExpectedUsage = dto.ExpectedUsage,
                ComplianceNeeds = dto.ComplianceNeeds,
                CpuCores = dto.CpuCores,
                RamGb = dto.RamGb,
                StorageGb = dto.StorageGb,
                PreferredRegion = dto.PreferredRegion,
                MonthlyBudget = dto.MonthlyBudget
            };

            // 3. Call the service (This saves the requirement, calls Azure AI, and saves the results)
            var result = await _recommendationService.ProcessAndGenerateAsync(requirement, userId);

            // 4. Map the result to our SAFE DTOs (This prevents the circular reference error!)
            var response = new GenerateRecommendationResponseDto
            {
                RequirementId = result.RequirementId,
                Recommendations = result.Recommendations.Select(r => new RecommendationDto
                {
                    Provider = r.Provider,
                    ServiceName = r.ServiceName,
                    EstimatedMonthlyCost = r.EstimatedMonthlyCost,
                    Justification = r.Justification,
                    SpecsJson = r.SpecsJson
                }).ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
        }
    }
}