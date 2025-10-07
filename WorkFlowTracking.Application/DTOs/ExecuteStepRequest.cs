using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowTracking.Application.DTOs
{
    public class ExecuteStepRequest
    {
        public Guid ProcessId { get; set; }
        public string StepName { get; set; } = null!;
        public string PerformedBy { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? Comments { get; set; }
    }
}
