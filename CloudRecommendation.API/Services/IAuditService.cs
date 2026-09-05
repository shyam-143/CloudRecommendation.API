namespace CloudRecommendation.API.Services
{
    public interface IAuditService
    {
        Task LogActionAsync(int? userId, string action, string? details = null);

    }
}
