using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WorkFlowTracking.Domain.Entities;
using WorkFlowTracking.Domain.Enums;

namespace WorkFlowTracking.Infrastucture.Data;

public partial class WorkflowTrackingDbContext : DbContext
{
    public WorkflowTrackingDbContext()
    {
    }

    public WorkflowTrackingDbContext(DbContextOptions<WorkflowTrackingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Process> Processes { get; set; }

    public virtual DbSet<ProcessStep> ProcessSteps { get; set; }

    public virtual DbSet<ValidationLog> ValidationLogs { get; set; }

    public virtual DbSet<Workflow> Workflows { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Process>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Processe__3214EC078820860B");

            entity.HasIndex(e => e.CreatedAt, "IX_Processes_CreatedAt");

            entity.HasIndex(e => e.CurrentStep, "IX_Processes_CurrentStep");

            entity.HasIndex(e => e.Initiator, "IX_Processes_Initiator");

            entity.HasIndex(e => e.Status, "IX_Processes_Status");

            entity.HasIndex(e => e.WorkflowId, "IX_Processes_WorkflowId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CurrentStep).HasMaxLength(200);
            entity.Property(e => e.Initiator).HasMaxLength(100);
            entity.Property(e => e.Status)
        .HasConversion<int>();
            entity.Property(e => e.Status).HasDefaultValue(ProcessStatus.Pending) // Use enum instead of magic number
            .HasConversion<int>(); 
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Workflow).WithMany(p => p.Processes)
                .HasForeignKey(d => d.WorkflowId)
                .HasConstraintName("FK_Processes_Workflows");
        });

        modelBuilder.Entity<ProcessStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProcessS__3214EC07886B69CE");

            entity.HasIndex(e => e.ExecutedAt, "IX_ProcessSteps_ExecutedAt");

            entity.HasIndex(e => e.PerformedBy, "IX_ProcessSteps_PerformedBy");

            entity.HasIndex(e => e.ProcessId, "IX_ProcessSteps_ProcessId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.Comments).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ExecutedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
            entity.Property(e => e.Status)
        .HasConversion<int>();
            entity.Property(e => e.Status).HasDefaultValue(ProcessStatus.Completed) // Use enum instead of magic number
            .HasConversion<int>();
            entity.Property(e => e.StepName).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Process).WithMany(p => p.ProcessSteps)
                .HasForeignKey(d => d.ProcessId)
                .HasConstraintName("FK_ProcessSteps_Processes");
        });

        modelBuilder.Entity<ValidationLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Validati__3214EC074E096AA3");

            entity.HasIndex(e => e.IsValid, "IX_ValidationLogs_IsValid");

            entity.HasIndex(e => e.ProcessId, "IX_ValidationLogs_ProcessId");

            entity.HasIndex(e => e.ValidatedAt, "IX_ValidationLogs_ValidatedAt");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.StepName).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ValidatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ValidationMessage).HasMaxLength(1000);

            entity.HasOne(d => d.Process).WithMany(p => p.ValidationLogs)
                .HasForeignKey(d => d.ProcessId)
                .HasConstraintName("FK_ValidationLogs_Processes");
        });

        modelBuilder.Entity<Workflow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workflow__3214EC07D5770631");

            entity.HasIndex(e => e.CreatedAt, "IX_Workflows_CreatedAt");

            entity.HasIndex(e => e.Status, "IX_Workflows_Status");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Status).HasDefaultValue(WorkflowStatus.Active) // Use enum instead of magic number
            .HasConversion<int>();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workflow__3214EC078DD9EA3B");

            entity.HasIndex(e => e.AssignedTo, "IX_WorkflowSteps_AssignedTo");

            entity.HasIndex(e => new { e.WorkflowId, e.Order }, "IX_WorkflowSteps_Order");

            entity.HasIndex(e => e.WorkflowId, "IX_WorkflowSteps_WorkflowId");

            entity.Property(e => e.ActionType)
        .HasConversion<int>();
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.NextStep).HasMaxLength(200);
            entity.Property(e => e.StepName).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Workflow).WithMany(p => p.WorkflowSteps)
                .HasForeignKey(d => d.WorkflowId)
                .HasConstraintName("FK_WorkflowSteps_Workflows");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
