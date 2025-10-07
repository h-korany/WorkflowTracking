using System;
using System.Collections.Generic;
using WorkFlowTracking.Domain.Enums;

namespace WorkFlowTracking.Domain.Entities;

public partial class Process
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string Initiator { get; set; } = null!;
    public ProcessStatus Status { get; set; }
    public string CurrentStep { get; set; } = null!;
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<ProcessStep> ProcessSteps { get; set; } = new List<ProcessStep>();
    public virtual ICollection<ValidationLog> ValidationLogs { get; set; } = new List<ValidationLog>();
    public virtual Workflow Workflow { get; set; } = null!;
}