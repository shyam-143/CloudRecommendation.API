using CloudRecommendation.Api.Data;
using CloudRecommendation.API.Services;
using CloudRecommendationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudRecommendation.Api.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogActionAsync(int? userId, string action, string? details = null)
    {
        try
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            // Never throw from audit logging - it's non-critical
            // In production, you'd log this to a file or monitoring system
        }
    }
}