using CloudRecommendationApi.Models;

namespace CloudRecommendation.API.Services
{
    public interface IOpenAiRecommendationService
    {
        Task<List<Recommendation>> GenerateRecommendationsAsync(Requirement requirement);

    }
}
