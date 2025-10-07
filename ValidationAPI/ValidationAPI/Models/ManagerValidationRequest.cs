namespace ValidationAPI.Models
{
    public class ManagerValidationRequest
    {
        public Guid ProcessId { get; set; }
        public string ManagerId { get; set; } = string.Empty;
        public string RequesterId { get; set; } = string.Empty;
        public string RequesterDepartment { get; set; } = string.Empty;
        public string ManagerDepartment { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
    }
}
