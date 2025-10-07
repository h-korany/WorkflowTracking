namespace ValidationAPI.Models
{
    public class FinanceValidationRequest
    {
        public Guid ProcessId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Department { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
    }
}
