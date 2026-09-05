using CloudRecommendation.Api.Data;
using CloudRecommendation.API.Services;
using CloudRecommendationApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudRecommendation.Api.Services;

public class RecommendationService : IRecommendationService
{
    private readonly AppDbContext _context;
    private readonly IOpenAiRecommendationService _aiService;
    private readonly IAuditService _auditService; // Assuming you have this
    private readonly ILogger<RecommendationService> _logger;

    public RecommendationService(
        AppDbContext context,
        IOpenAiRecommendationService aiService,
        IAuditService auditService,
        ILogger<RecommendationService> logger)
    {
        _context = context;
        _aiService = aiService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<RecommendationResultDto> ProcessAndGenerateAsync(Requirement requirement, int userId)
    {
        // 1. Assign User and Save Requirement (Search History)
        requirement.UserId = userId;
        requirement.CreatedAt = DateTime.UtcNow;

        _context.Requirements.Add(requirement);
        await _context.SaveChangesAsync(); // Get the generated Requirement.Id

        try
        {
            // 2. Call Gen AI Service
            var aiResults = await _aiService.GenerateRecommendationsAsync(requirement);

            // 3. Save Recommendations to DB
            foreach (var rec in aiResults)
            {
                rec.RequirementId = requirement.Id;
                rec.CreatedAt = DateTime.UtcNow;
            }

            _context.Recommendations.AddRange(aiResults);
            await _context.SaveChangesAsync();

            // 4. Audit Log
            await _auditService.LogActionAsync(userId, "GenerateRecommendations", $"Successfully generated {aiResults.Count} recommendations for Req ID: {requirement.Id}");

            return new RecommendationResultDto
            {
                RequirementId = requirement.Id,
                Recommendations = aiResults
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate recommendations for Requirement ID: {RequirementId}", requirement.Id);

            // Optional: Log failure to audit
            await _auditService.LogActionAsync(userId, "GenerateRecommendations_Failed", $"Error: {ex.Message}");

            throw; // Let the controller handle the 500 response
        }
    }
}