using CloudRecommendationApi.Models;

namespace CloudRecommendationApi.Services;

public interface ITokenService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
