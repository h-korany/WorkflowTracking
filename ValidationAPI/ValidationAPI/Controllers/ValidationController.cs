using Microsoft.AspNetCore.Mvc;
using ValidationAPI.Models;

namespace ValidationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly ILogger<ValidationController> _logger;

    public ValidationController(ILogger<ValidationController> logger)
    {
        _logger = logger;
    }

    [HttpPost("finance")]
    public async Task<ActionResult<ValidationResponse>> ValidateFinanceApproval([FromBody] FinanceValidationRequest request)
    {
        _logger.LogInformation("Finance validation request for Process: {ProcessId}, User: {UserId}",
            request.ProcessId, request.UserId);

        // Simulate API processing time
        await Task.Delay(200);

        // Business logic for finance validation
        var isValid = ValidateFinanceRequest(request);

        var response = new ValidationResponse
        {
            IsValid = isValid,
            Message = isValid
                ? "Finance validation passed successfully"
                : "Finance validation failed: Amount exceeds limit or budget constraints",
            ValidationType = "Finance",
            Timestamp = DateTime.UtcNow,
            ReferenceId = Guid.NewGuid().ToString()
        };

        _logger.LogInformation("Finance validation result: {IsValid} for Process: {ProcessId}",
            isValid, request.ProcessId);

        return Ok(response);
    }

    [HttpPost("manager")]
    public async Task<ActionResult<ValidationResponse>> ValidateManagerApproval([FromBody] ManagerValidationRequest request)
    {
        _logger.LogInformation("Manager validation request for Process: {ProcessId}, Manager: {ManagerId}",
            request.ProcessId, request.ManagerId);

        await Task.Delay(150);

        // Business logic for manager validation
       var isValid = ValidateManagerRequest(request);

        var response = new ValidationResponse
        {
            IsValid = isValid,
            Message = isValid
                ? "Manager approval validated successfully"
                : "Manager validation failed: Insufficient permissions or department mismatch",
            ValidationType = "Manager",
            Timestamp = DateTime.UtcNow,
            ReferenceId = Guid.NewGuid().ToString()
        };

        return Ok(response);
    }

    [HttpPost("compliance")]
    public async Task<ActionResult<ValidationResponse>> ValidateCompliance([FromBody] ComplianceValidationRequest request)
    {
        _logger.LogInformation("Compliance validation request for Step: {StepName}, Process: {ProcessId}",
            request.StepName, request.ProcessId);

        await Task.Delay(300);

        var isValid = ValidateComplianceRequest(request);

        var response = new ValidationResponse
        {
            IsValid = isValid,
            Message = isValid
                ? "Compliance check passed"
                : "Compliance validation failed: Policy violation detected",
            ValidationType = "Compliance",
            Timestamp = DateTime.UtcNow,
            ReferenceId = Guid.NewGuid().ToString()
        };

        return Ok(response);
    }

    [HttpPost("custom")]
    public async Task<ActionResult<ValidationResponse>> ValidateCustomStep([FromBody] CustomValidationRequest request)
    {
        _logger.LogInformation("Custom validation for Step: {StepName}, Process: {ProcessId}",
            request.StepName, request.ProcessId);

        await Task.Delay(100);

        // Generic validation logic that can be used for any step
        var isValid = ValidateCustomStepSimulate(request);

        var response = new ValidationResponse
        {
            IsValid = isValid,
            Message = isValid
                ? $"Step '{request.StepName}' validation passed"
                : $"Step '{request.StepName}' validation failed",
            ValidationType = request.StepName,
            Timestamp = DateTime.UtcNow,
            ReferenceId = Guid.NewGuid().ToString()
        };

        return Ok(response);
    }

    [HttpGet("health")]
    public ActionResult HealthCheck()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }

    private bool ValidateFinanceRequest(FinanceValidationRequest request)
    {
        // Simulate finance validation rules
        var random = new Random();

        // 80% success rate for amounts under 5000, 50% for amounts over 5000
        var successRate = request.Amount <= 5000 ? 0.8 : 0.5;

        // Check if user has finance role
        var hasFinanceRole = request.UserId?.ToLower().Contains("finance") == true ||
                            request.UserId?.ToLower().Contains("account") == true;

        return random.NextDouble() < successRate && hasFinanceRole;
    }

    private bool ValidateManagerRequest(ManagerValidationRequest request)
    {
        // Simulate manager validation rules
        var isManager = request.ManagerId?.ToLower().StartsWith("manager") == true ||
                       request.ManagerId?.ToLower().Contains("lead") == true ||
                       request.ManagerId?.ToLower().Contains("director") == true;

        // Check if manager is in the same department as requester
        var sameDepartment = !string.IsNullOrEmpty(request.RequesterDepartment) &&
                            !string.IsNullOrEmpty(request.ManagerDepartment) &&
                            request.RequesterDepartment == request.ManagerDepartment;

        return isManager && sameDepartment;
    }

    private bool ValidateComplianceRequest(ComplianceValidationRequest request)
    {
        // Simulate compliance checks
        var hasRequiredDocuments = request.DocumentCount >= request.RequiredDocuments;
        var noRestrictedKeywords = !request.Description?.ToLower().Contains("confidential") == true;

        return hasRequiredDocuments && noRestrictedKeywords;
    }

    private bool ValidateCustomStepSimulate(CustomValidationRequest request)
    {
        // Generic validation that works for any step
        var random = new Random();

        // Higher success rate for simpler steps
        var successRate = request.StepName.ToLower() switch
        {
            var name when name.Contains("review") => 0.9,
            var name when name.Contains("approval") => 0.7,
            var name when name.Contains("validation") => 0.8,
            _ => 0.85
        };

        return random.NextDouble() < successRate;
    }
}