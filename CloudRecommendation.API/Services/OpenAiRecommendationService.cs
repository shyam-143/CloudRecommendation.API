using Azure.AI.OpenAI;
using CloudRecommendation.API.Services;
using CloudRecommendationApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace CloudRecommendation.Api.Services;

public class OpenAiRecommendationService : IOpenAiRecommendationService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName;
    private readonly ILogger<OpenAiRecommendationService> _logger;

    public OpenAiRecommendationService(IConfiguration config, ILogger<OpenAiRecommendationService> logger)
    {
        var endpoint = config["AzureOpenAi:Endpoint"] ?? throw new ArgumentNullException("AzureOpenAi:Endpoint");
        var apiKey = config["AzureOpenAi:ApiKey"] ?? throw new ArgumentNullException("AzureOpenAi:ApiKey");
        _deploymentName = config["AzureOpenAi:DeploymentName"] ?? "gpt-4o";

        // NEW v2.x API - Use ApiKeyCredential
        var credential = new ApiKeyCredential(apiKey);
        _client = new AzureOpenAIClient(new Uri(endpoint), credential);
        _logger = logger;
    }

    public async Task<List<Recommendation>> GenerateRecommendationsAsync(Requirement requirement)
    {
        try
        {
            string prompt = $@"
You are an expert Cloud Architect. Based on the following user requirements, recommend the best-fit cloud services from AWS, Azure, and GCP. 

User Requirements:
- Workload Type: {requirement.WorkloadType}
- Expected Usage: {requirement.ExpectedUsage}
- Compliance Needs: {requirement.ComplianceNeeds ?? "None"}
- CPU Cores: {requirement.CpuCores}
- RAM (GB): {requirement.RamGb}
- Storage (GB): {requirement.StorageGb}
- Preferred Region: {requirement.PreferredRegion ?? "Any"}
- Monthly Budget: {(requirement.MonthlyBudget.HasValue ? $"${requirement.MonthlyBudget.Value}" : "No strict budget")}

Instructions:
1. Provide exactly ONE recommendation per provider (AWS, Azure, GCP).
2. Ensure the EstimatedMonthlyCost is a realistic number based on current public pricing.
3. Return ONLY a valid JSON array. No markdown, no explanations.

JSON Schema Required:
[
  {{
    ""Provider"": ""AWS"",
    ""ServiceName"": ""e.g., EC2 t3.medium"",
    ""EstimatedMonthlyCost"": 150.50,
    ""Justification"": ""Brief reason"",
    ""SpecsJson"": ""{{ \""vCpu\"": 2, \""RAM\"": 8 }}""
  }}
]";

            // NEW v2.x API - Use ChatClient
            var chatClient = _client.GetChatClient(_deploymentName);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful cloud architecture assistant that outputs strictly valid JSON."),
                new UserChatMessage(prompt)
            };

            var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                Temperature = 0.2f,
            });

            string aiResponse = response.Value.Content[0].Text;

            // Clean and parse JSON
            aiResponse = aiResponse.Replace("```json", "").Replace("```", "").Trim();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var aiRecommendations = JsonSerializer.Deserialize<List<AiRecommendationDto>>(aiResponse, options);

            var finalRecommendations = new List<Recommendation>();
            if (aiRecommendations != null)
            {
                foreach (var rec in aiRecommendations)
                {
                    finalRecommendations.Add(new Recommendation
                    {
                        RequirementId = requirement.Id,
                        Provider = rec.Provider,
                        ServiceName = rec.ServiceName,
                        EstimatedMonthlyCost = rec.EstimatedMonthlyCost,
                        Justification = rec.Justification,
                        SpecsJson = rec.SpecsJson,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            return finalRecommendations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI recommendations");
            throw new Exception("Failed to generate cloud recommendations. Please try again.");
        }
    }

    private class AiRecommendationDto
    {
        public string Provider { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public decimal EstimatedMonthlyCost { get; set; }
        public string Justification { get; set; } = string.Empty;
        public string SpecsJson { get; set; } = string.Empty;
    }
}