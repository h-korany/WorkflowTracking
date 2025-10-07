using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowTracking.Domain.Enums;

namespace WorkFlowTracking.Application.DTOs
{
    public class WorkflowStepDto
    {
        public string StepName { get; set; } = null!;
        public string AssignedTo { get; set; } = null!;
        public ActionType ActionType { get; set; }
        public string? NextStep { get; set; }
        public int Order { get; set; }
        public bool RequiresValidation { get; set; }
    }
}
