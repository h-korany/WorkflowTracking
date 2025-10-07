using System;
using System.Collections.Generic;
using WorkFlowTracking.Domain.Enums;

namespace WorkFlowTracking.Domain.Entities;

public partial class ProcessStep
{
    public Guid Id { get; set; }
    public Guid ProcessId { get; set; }
    public string StepName { get; set; } = null!;
    public string PerformedBy { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string? Comments { get; set; }
    public DateTime ExecutedAt { get; set; }
    public ProcessStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual Process Process { get; set; } = null!;
}