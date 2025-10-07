using Microsoft.AspNetCore.Mvc;
using WorkFlowTracking.Application.DTOs;
using WorkFlowTracking.Application.Interfaces;

namespace WorkFlowTracking.Presentation.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class WorkflowsController : ControllerBase
{
    private readonly IWorkflowService _workflowService;

    public WorkflowsController(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    [HttpPost]
    public async Task<ActionResult<WorkflowDto>> CreateWorkflow(WorkflowDto workflowDto)
    {
        try
        {
            var result = await _workflowService.CreateWorkflowAsync(workflowDto);
            return CreatedAtAction(nameof(GetWorkflow), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkflowDto>> GetWorkflow(Guid id)
    {
        var workflow = await _workflowService.GetWorkflowAsync(id);
        if (workflow == null)
            return NotFound();

        return Ok(workflow);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkflowDto>>> GetAllWorkflows()
    {
        var workflows = await _workflowService.GetAllWorkflowsAsync();
        return Ok(workflows);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WorkflowDto>> UpdateWorkflow(Guid id, WorkflowDto workflowDto)
    {
        try
        {
            var result = await _workflowService.UpdateWorkflowAsync(id, workflowDto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteWorkflow(Guid id)
    {
        try
        {
            var result = await _workflowService.DeleteWorkflowAsync(id);
            return Ok(new { message = "Workflow deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}