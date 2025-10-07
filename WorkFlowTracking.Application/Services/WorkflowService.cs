using WorkFlowTracking.Application.DTOs;
using WorkFlowTracking.Application.Interfaces;
using WorkFlowTracking.Domain.Entities;
using WorkFlowTracking.Domain.Enums;
using WorkFlowTracking.Domain.Interfaces;

namespace WorkFlowTracking.Application.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IRepository<Workflow> _workflowRepository;
    private readonly IRepository<WorkflowStep> _workflowStepRepository;
    private readonly IRepository<Process> _processRepository;

    public WorkflowService(
    IRepository<Workflow> workflowRepository,
    IRepository<WorkflowStep> workflowStepRepository,
    IRepository<Process> processRepository) // Added
    {
        _workflowRepository = workflowRepository;
        _workflowStepRepository = workflowStepRepository;
        _processRepository = processRepository; // Added
    }

    public async Task<WorkflowDto> CreateWorkflowAsync(WorkflowDto workflowDto)
    {
        var workflow = new Workflow
        {
            Id = Guid.NewGuid(),
            Name = workflowDto.Name,
            Description = workflowDto.Description,
            Status = WorkflowStatus.Active, // Use enum
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _workflowRepository.AddAsync(workflow);

        var steps = workflowDto.Steps.Select((stepDto, index) => new WorkflowStep
        {
            Id = Guid.NewGuid(),
            WorkflowId = workflow.Id,
            StepName = stepDto.StepName,
            AssignedTo = stepDto.AssignedTo,
            ActionType = stepDto.ActionType, // Use enum
            NextStep = stepDto.NextStep,
            Order = stepDto.Order,
            RequiresValidation = stepDto.RequiresValidation,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        foreach (var step in steps)
        {
            await _workflowStepRepository.AddAsync(step);
        }

        await _workflowRepository.SaveChangesAsync();

        workflowDto.Id = workflow.Id;
        workflowDto.Status = workflow.Status;
        return workflowDto;
    }
    public async Task<WorkflowDto> UpdateWorkflowAsync(Guid id, WorkflowDto workflowDto)
    {
        var existingWorkflow = await _workflowRepository.GetByIdAsync(id);
        if (existingWorkflow == null)
            throw new KeyNotFoundException($"Workflow with ID {id} not found");

        // Check if workflow has active processes
        var hasActiveProcesses = await CheckWorkflowHasActiveProcesses(id);
        if (hasActiveProcesses)
            throw new InvalidOperationException("Cannot update workflow with active processes");

        // Update workflow properties
        existingWorkflow.Name = workflowDto.Name;
        existingWorkflow.Description = workflowDto.Description;
        existingWorkflow.UpdatedAt = DateTime.UtcNow;

        // Get existing steps
        var existingSteps = await _workflowStepRepository.FindAsync(ws => ws.WorkflowId == id);

        // Remove existing steps
        foreach (var existingStep in existingSteps)
        {
            _workflowStepRepository.Delete(existingStep);
        }

        // Add new steps
        var newSteps = workflowDto.Steps.Select((stepDto, index) => new WorkflowStep
        {
            Id = Guid.NewGuid(),
            WorkflowId = id,
            StepName = stepDto.StepName,
            AssignedTo = stepDto.AssignedTo,
            ActionType = stepDto.ActionType,
            NextStep = stepDto.NextStep,
            Order = stepDto.Order,
            RequiresValidation = stepDto.RequiresValidation,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        foreach (var step in newSteps)
        {
            await _workflowStepRepository.AddAsync(step);
        }

        _workflowRepository.Update(existingWorkflow);
        await _workflowRepository.SaveChangesAsync();

        workflowDto.Id = id;
        return workflowDto;
    }

    public async Task<bool> DeleteWorkflowAsync(Guid id)
    {
        var workflow = await _workflowRepository.GetByIdAsync(id);
        if (workflow == null)
            throw new KeyNotFoundException($"Workflow with ID {id} not found");

        // Check if workflow has active processes
        var hasActiveProcesses = await CheckWorkflowHasActiveProcesses(id);
        if (hasActiveProcesses)
            throw new InvalidOperationException("Cannot delete workflow with active processes");

        // Get and delete all steps first
        var steps = await _workflowStepRepository.FindAsync(ws => ws.WorkflowId == id);
        foreach (var step in steps)
        {
            _workflowStepRepository.Delete(step);
        }

        // Delete workflow
        _workflowRepository.Delete(workflow);
        await _workflowRepository.SaveChangesAsync();

        return true;
    }

    public async Task<WorkflowDto?> GetWorkflowAsync(Guid id)
    {
        var workflow = await _workflowRepository.GetByIdAsync(id);
        if (workflow == null) return null;

        var steps = await _workflowStepRepository.FindAsync(ws => ws.WorkflowId == id);

        return new WorkflowDto
        {
            Id = workflow.Id,
            Name = workflow.Name,
            Description = workflow.Description,
            Steps = steps.OrderBy(s => s.Order).Select(s => new WorkflowStepDto
            {
                StepName = s.StepName,
                AssignedTo = s.AssignedTo,
                ActionType = s.ActionType,
                NextStep = s.NextStep,
                Order = s.Order,
                RequiresValidation = s.RequiresValidation
            }).ToList()
        };
    }

    public async Task<IEnumerable<WorkflowDto>> GetAllWorkflowsAsync()
    {
        var workflows = await _workflowRepository.GetAllAsync();
        var result = new List<WorkflowDto>();

        foreach (var workflow in workflows)
        {
            var steps = await _workflowStepRepository.FindAsync(ws => ws.WorkflowId == workflow.Id);

            result.Add(new WorkflowDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                Steps = steps.OrderBy(s => s.Order).Select(s => new WorkflowStepDto
                {
                    StepName = s.StepName,
                    AssignedTo = s.AssignedTo,
                    ActionType = s.ActionType,
                    NextStep = s.NextStep,
                    Order = s.Order,
                    RequiresValidation = s.RequiresValidation
                }).ToList()
            });
        }

        return result;
    }
    //private async Task<bool> CheckWorkflowHasActiveProcesses(Guid workflowId)
    //{
    //    var activeProcesses = await _processRepository.FindAsync(p =>
    //        p.WorkflowId == workflowId && p.Status == 1); // Status 1 = Active

    //    return activeProcesses.Any();
    //}

    private async Task<bool> CheckWorkflowHasActiveProcesses(Guid workflowId)
    {
        var activeProcesses = await _processRepository.FindAsync(p =>
            p.WorkflowId == workflowId && p.Status == ProcessStatus.Active); // Use enum

        return activeProcesses.Any();
    }
}