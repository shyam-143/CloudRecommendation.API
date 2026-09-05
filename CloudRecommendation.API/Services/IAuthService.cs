using CloudRecommendationApi.DTOs;

namespace CloudRecommendationApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserProfileDto?> GetProfileAsync(int userId);
}

public class AuthException : Exception
{
    public AuthException(string message) : base(message) { }
}
