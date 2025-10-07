using System;
using System.Collections.Generic;

namespace WorkFlowTracking.Domain.Entities;

public partial class ValidationLog
{
    public Guid Id { get; set; }

    public Guid ProcessId { get; set; }

    public string StepName { get; set; } = null!;

    public bool IsValid { get; set; }

    public string? ValidationMessage { get; set; }

    public DateTime ValidatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Process Process { get; set; } = null!;
}
