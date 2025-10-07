using System;
using System.Collections.Generic;
using WorkFlowTracking.Domain.Enums;

namespace WorkFlowTracking.Domain.Entities;

public partial class Workflow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public WorkflowStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<Process> Processes { get; set; } = new List<Process>();
    public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
}
