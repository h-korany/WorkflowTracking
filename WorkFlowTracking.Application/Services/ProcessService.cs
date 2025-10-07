using WorkFlowTracking.Application.DTOs;
using WorkFlowTracking.Application.Interfaces;
using WorkFlowTracking.Domain.Entities;
using WorkFlowTracking.Domain.Enums;
using WorkFlowTracking.Domain.Interfaces;

namespace WorkFlowTracking.Application.Services;

public class ProcessService : IProcessService
{
    private readonly IRepository<Process> _processRepository;
    private readonly IRepository<ProcessStep> _processStepRepository;
    private readonly IRepository<Workflow> _workflowRepository;
    private readonly IRepository<WorkflowStep> _workflowStepRepository;
    private readonly IValidationService _validationService;

    public ProcessService(
        IRepository<Process> processRepository,
        IRepository<ProcessStep> processStepRepository,
        IRepository<Workflow> workflowRepository,
        IRepository<WorkflowStep> workflowStepRepository,
        IValidationService validationService)
    {
        _processRepository = processRepository;
        _processStepRepository = processStepRepository;
        _workflowRepository = workflowRepository;
        _workflowStepRepository = workflowStepRepository;
        _validationService = validationService;
    }

    public async Task<Process> StartProcessAsync(StartProcessRequest request)
    {
        var workflow = await _workflowRepository.GetByIdAsync(request.WorkflowId);
        if (workflow == null)
            throw new ArgumentException("Workflow not found");

        var firstStep = (await _workflowStepRepository
            .FindAsync(ws => ws.WorkflowId == request.WorkflowId))
            .OrderBy(ws => ws.Order)
            .FirstOrDefault();

        if (firstStep == null)
            throw new InvalidOperationException("Workflow has no steps");

        var process = new Process
        {
            Id = Guid.NewGuid(),
            WorkflowId = request.WorkflowId,
            Initiator = request.Initiator,
            Status = ProcessStatus.Active, // Use enum
            CurrentStep = firstStep.StepName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _processRepository.AddAsync(process);
        await _processRepository.SaveChangesAsync();

        return process;
    }

    public async Task<ProcessStep> ExecuteStepAsync(ExecuteStepRequest request)
    {
        var process = await _processRepository.GetByIdAsync(request.ProcessId);
        if (process == null)
            throw new ArgumentException("Process not found");

        var currentWorkflowStep = (await _workflowStepRepository
            .FindAsync(ws => ws.WorkflowId == process.WorkflowId && ws.StepName == request.StepName))
            .FirstOrDefault();

        if (currentWorkflowStep == null)
            throw new ArgumentException("Step not found in workflow");

        // Validate step execution
        if (currentWorkflowStep.AssignedTo != request.PerformedBy)
            throw new UnauthorizedAccessException("User not authorized to perform this step");

        // Perform validation if required
        if (currentWorkflowStep.RequiresValidation)
        {
            var isValid = await _validationService.ValidateStepAsync(
                request.StepName, request.ProcessId, request.PerformedBy);

            if (!isValid)
            {
                await _validationService.LogValidationAsync(
                    request.ProcessId, request.StepName, false, "Validation failed");
                throw new InvalidOperationException("Step validation failed");
            }

            await _validationService.LogValidationAsync(
                request.ProcessId, request.StepName, true, "Validation successful");
        }

        // Create process step
        var processStep = new ProcessStep
        {
            Id = Guid.NewGuid(),
            ProcessId = request.ProcessId,
            StepName = request.StepName,
            PerformedBy = request.PerformedBy,
            Action = request.Action,
            Comments = request.Comments,
            ExecutedAt = DateTime.UtcNow,
            Status = ProcessStatus.Completed, // Use enum
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _processStepRepository.AddAsync(processStep);

        // Update process current step and status
        var nextWorkflowStep = await GetNextWorkflowStepAsync(process.WorkflowId, request.StepName);

        if (nextWorkflowStep != null)
        {
            process.CurrentStep = nextWorkflowStep.StepName;
            process.Status = ProcessStatus.Active; // Use enum
        }
        else
        {
            process.CurrentStep = "Completed";
            process.Status = ProcessStatus.Completed; // Use enum
            process.CompletedAt = DateTime.UtcNow;
        }

        process.UpdatedAt = DateTime.UtcNow;
        _processRepository.Update(process);

        await _processRepository.SaveChangesAsync();

        return processStep;
    }

    public async Task<IEnumerable<Process>> GetProcessesAsync(ProcessQuery query)
    {
        var processes = await _processRepository.GetAllAsync();
        var filteredProcesses = processes.AsQueryable();

        if (query.WorkflowId.HasValue)
            filteredProcesses = filteredProcesses.Where(p => p.WorkflowId == query.WorkflowId);

        if (query.Status.HasValue)
            filteredProcesses = filteredProcesses.Where(p => p.Status == query.Status); // Use enum

        if (!string.IsNullOrEmpty(query.AssignedTo))
        {
            var workflowSteps = await _workflowStepRepository.FindAsync(ws => ws.AssignedTo == query.AssignedTo);
            var workflowIds = workflowSteps.Select(ws => ws.WorkflowId).Distinct();

            filteredProcesses = filteredProcesses.Where(p =>
                workflowIds.Contains(p.WorkflowId) &&
                workflowSteps.Any(ws => ws.StepName == p.CurrentStep && ws.WorkflowId == p.WorkflowId));
        }

        return filteredProcesses.ToList();
    }

    private async Task<WorkflowStep?> GetNextWorkflowStepAsync(Guid workflowId, string currentStepName)
    {
        var currentStep = (await _workflowStepRepository
            .FindAsync(ws => ws.WorkflowId == workflowId && ws.StepName == currentStepName))
            .FirstOrDefault();

        if (currentStep?.NextStep == null || currentStep.NextStep == "Completed")
            return null;

        return (await _workflowStepRepository
            .FindAsync(ws => ws.WorkflowId == workflowId && ws.StepName == currentStep.NextStep))
            .FirstOrDefault();
    }

}