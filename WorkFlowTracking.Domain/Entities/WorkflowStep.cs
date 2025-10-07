using System;
using System.Collections.Generic;
using WorkFlowTracking.Domain.Enums;

namespace WorkFlowTracking.Domain.Entities;

public partial class WorkflowStep
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string StepName { get; set; } = null!;
    public string AssignedTo { get; set; } = null!;
    public ActionType ActionType { get; set; }
    public string? NextStep { get; set; }
    public int Order { get; set; }
    public bool RequiresValidation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual Workflow Workflow { get; set; } = null!;
}