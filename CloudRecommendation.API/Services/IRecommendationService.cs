using CloudRecommendationApi.Models;

namespace CloudRecommendation.API.Services
{

    public interface IRecommendationService
    {
        Task<RecommendationResultDto> ProcessAndGenerateAsync(Requirement requirement, int userId);
    }

    // DTO to return both the saved requirement ID and the generated recommendations
    public class RecommendationResultDto
    {
        public int RequirementId { get; set; }
        public List<Recommendation> Recommendations { get; set; } = new();
    }
}
