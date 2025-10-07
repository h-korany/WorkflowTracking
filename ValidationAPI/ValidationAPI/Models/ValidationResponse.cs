
namespace ValidationAPI.Models;

public class ValidationResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ValidationType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
}