using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowTracking.Domain.Enums;

namespace WorkFlowTracking.Application.DTOs
{
    public class WorkflowDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public WorkflowStatus Status { get; set; } = WorkflowStatus.Active;
        public List<WorkflowStepDto> Steps { get; set; } = new();
    }
}
