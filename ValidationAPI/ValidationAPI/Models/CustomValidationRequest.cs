namespace ValidationAPI.Models
{
    public class CustomValidationRequest
    {
        public Guid ProcessId { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}
