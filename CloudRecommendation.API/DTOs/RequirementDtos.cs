using System.ComponentModel.DataAnnotations;

namespace CloudRecommendation.Api.DTOs;

public class RequirementCreateDto
{
    [Required]
    public string WorkloadType { get; set; } = string.Empty;

    [Required]
    public string ExpectedUsage { get; set; } = string.Empty;

    public string? ComplianceNeeds { get; set; }

    [Range(1, 1024)]
    public int CpuCores { get; set; }

    [Range(1, 8192)]
    public int RamGb { get; set; }

    [Range(1, 1000000)]
    public int StorageGb { get; set; }

    public string? PreferredRegion { get; set; }

    public decimal? MonthlyBudget { get; set; }
}

// This is the specific response for the Gen AI endpoint
public class GenerateRecommendationResponseDto
{
    public int RequirementId { get; set; }
    public List<RecommendationDto> Recommendations { get; set; } = new();
}

// A safe, flat DTO for recommendations (NO navigation properties back to Requirement)
public class RecommendationDto
{
    public string Provider { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal EstimatedMonthlyCost { get; set; }
    public string Justification { get; set; } = string.Empty;
    public string? SpecsJson { get; set; }
}

public class RequirementResponseDto
{
    public int Id { get; set; }
    public string WorkloadType { get; set; } = string.Empty;
    public string ExpectedUsage { get; set; } = string.Empty;
    public string? ComplianceNeeds { get; set; }
    public int CpuCores { get; set; }
    public int RamGb { get; set; }
    public int StorageGb { get; set; }
    public string? PreferredRegion { get; set; }
    public decimal? MonthlyBudget { get; set; }
    public DateTime CreatedAt { get; set; }
}