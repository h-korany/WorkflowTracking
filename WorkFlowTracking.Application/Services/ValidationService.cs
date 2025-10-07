using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using WorkFlowTracking.Application.Interfaces;
using WorkFlowTracking.Domain.Entities;
using WorkFlowTracking.Domain.Interfaces;

namespace WorkFlowTracking.Application.Services;

public class ValidationService : IValidationService
{
    private readonly IRepository<ValidationLog> _validationLogRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ValidationService> _logger;
    private readonly string _validationApiBaseUrl;

    public ValidationService(
        IRepository<ValidationLog> validationLogRepository,
        IHttpClientFactory httpClientFactory,
        ILogger<ValidationService> logger,
        IConfiguration configuration)
    {
        _validationLogRepository = validationLogRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _validationApiBaseUrl = configuration["ValidationApi:BaseUrl"] ?? "https://localhost:7001";
    }

    public async Task<bool> ValidateStepAsync(string stepName, Guid processId, string performedBy)
    {
        try
        {
            _logger.LogInformation("Starting validation for step: {StepName}, process: {ProcessId}, user: {PerformedBy}",
                stepName, processId, performedBy);

            // Determine which validation endpoint to call based on step name
            var validationResult = stepName.ToLower() switch
            {
                var name when name.Contains("finance") => await ValidateFinanceStep(processId, performedBy),
                var name when name.Contains("manager") => await ValidateManagerStep(processId, performedBy),
                var name when name.Contains("compliance") => await ValidateComplianceStep(processId, stepName),
                var name when name.Contains("legal") => await ValidateComplianceStep(processId, stepName),
                _ => await ValidateGenericStep(processId, stepName, performedBy)
            };

            _logger.LogInformation("Validation result for step {StepName}: {IsValid}", stepName, validationResult);
            return validationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validation failed for step: {StepName}, process: {ProcessId}", stepName, processId);
            await LogValidationAsync(processId, stepName, false, $"Validation error: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> ValidateFinanceStep(Guid processId, string performedBy)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(230);

        var request = new
        {
            ProcessId = processId,
            UserId = performedBy,
            Amount = new Random().Next(100, 10000), // Simulated amount
            Department = "Finance",
            RequestType = "Approval"
        };

        var response = await client.PostAsJsonAsync($"{_validationApiBaseUrl}/api/validation/finance", request);

        if (response.IsSuccessStatusCode)
        {
            var validationResponse = await response.Content.ReadFromJsonAsync<ExternalValidationResponse>();
            return validationResponse?.IsValid == true;
        }

        _logger.LogWarning("Finance validation API returned status: {StatusCode}", response.StatusCode);
        return false;
    }

    private async Task<bool> ValidateManagerStep(Guid processId, string performedBy)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(160);

        var request = new
        {
            ProcessId = processId,
            ManagerId = performedBy,
            RequesterId = "employee123", // This would come from process context
            RequesterDepartment = "Operations",
            ManagerDepartment = "Operations",
            ActionType = "Approval"
        };

        var response = await client.PostAsJsonAsync($"{_validationApiBaseUrl}/api/validation/manager", request);

        if (response.IsSuccessStatusCode)
        {
            var validationResponse = await response.Content.ReadFromJsonAsync<ExternalValidationResponse>();
            return validationResponse?.IsValid == true;
        }

        _logger.LogWarning("Manager validation API returned status: {StatusCode}", response.StatusCode);
        return false;
    }

    private async Task<bool> ValidateComplianceStep(Guid processId, string stepName)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);

        var request = new
        {
            ProcessId = processId,
            StepName = stepName,
            DocumentCount = new Random().Next(1, 5),
            RequiredDocuments = 2,
            Description = "Standard compliance check"
        };

        var response = await client.PostAsJsonAsync($"{_validationApiBaseUrl}/api/validation/compliance", request);

        if (response.IsSuccessStatusCode)
        {
            var validationResponse = await response.Content.ReadFromJsonAsync<ExternalValidationResponse>();
            return validationResponse?.IsValid == true;
        }

        _logger.LogWarning("Compliance validation API returned status: {StatusCode}", response.StatusCode);
        return false;
    }

    private async Task<bool> ValidateGenericStep(Guid processId, string stepName, string performedBy)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);

        var request = new
        {
            ProcessId = processId,
            StepName = stepName,
            PerformedBy = performedBy,
            AdditionalData = new Dictionary<string, object>
            {
                { "timestamp", DateTime.UtcNow },
                { "validationType", "generic" }
            }
        };

        var response = await client.PostAsJsonAsync($"{_validationApiBaseUrl}/api/validation/custom", request);

        if (response.IsSuccessStatusCode)
        {
            var validationResponse = await response.Content.ReadFromJsonAsync<ExternalValidationResponse>();
            return validationResponse?.IsValid == true;
        }

        _logger.LogWarning("Custom validation API returned status: {StatusCode}", response.StatusCode);
        return false;
    }

    public async Task LogValidationAsync(Guid processId, string stepName, bool isValid, string? message)
    {
        var validationLog = new ValidationLog
        {
            Id = Guid.NewGuid(),
            ProcessId = processId,
            StepName = stepName,
            IsValid = isValid,
            ValidationMessage = message,
            ValidatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _validationLogRepository.AddAsync(validationLog);
        await _validationLogRepository.SaveChangesAsync();

        _logger.LogInformation("Validation logged for process {ProcessId}, step {StepName}: {IsValid}",
            processId, stepName, isValid);
    }

    // Helper method to check if validation service is available
    public async Task<bool> IsValidationServiceAvailable()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var response = await client.GetAsync($"{_validationApiBaseUrl}/api/validation/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

// Response DTO for external validation API
public class ExternalValidationResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ValidationType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
}