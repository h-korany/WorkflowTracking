namespace ValidationAPI.Models
{
    public class ComplianceValidationRequest
    {
        public Guid ProcessId { get; set; }
        public string StepName { get; set; } = string.Empty;
        public int DocumentCount { get; set; }
        public int RequiredDocuments { get; set; } = 1;
        public string? Description { get; set; }
    }
}
