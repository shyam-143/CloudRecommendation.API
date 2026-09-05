namespace CloudRecommendationApi.Models;

public class Requirement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public string WorkloadType { get; set; } = string.Empty;   // e.g. Web App, ML Training, Batch Processing
    public string ExpectedUsage { get; set; } = string.Empty;  // e.g. Low, Medium, High / requests-per-day
    public string? ComplianceNeeds { get; set; }                // e.g. HIPAA, GDPR, SOC2
    public int CpuCores { get; set; }
    public int RamGb { get; set; }
    public int StorageGb { get; set; }
    public string? PreferredRegion { get; set; }
    public decimal? MonthlyBudget { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
}

public class Recommendation
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public Requirement? Requirement { get; set; }

    public string Provider { get; set; } = string.Empty;       // AWS, Azure, GCP
    public string ServiceName { get; set; } = string.Empty;
    public decimal EstimatedMonthlyCost { get; set; }
    public string? Justification { get; set; }
    public string? SpecsJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
