using Microsoft.AspNetCore.Mvc;
using WorkFlowTracking.Application.DTOs;
using WorkFlowTracking.Application.Interfaces;

namespace WorkFlowTracking.Presentation.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class ProcessesController : ControllerBase
{
    private readonly IProcessService _processService;

    public ProcessesController(IProcessService processService)
    {
        _processService = processService;
    }

    [HttpPost("start")]
    public async Task<ActionResult> StartProcess(StartProcessRequest request)
    {
        try
        {
            var process = await _processService.StartProcessAsync(request);
            return Ok(new { process_id = process.Id, message = "Process started successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("execute")]
    public async Task<ActionResult> ExecuteStep(ExecuteStepRequest request)
    {
        try
        {
            var processStep = await _processService.ExecuteStepAsync(request);
            return Ok(new
            {
                step_id = processStep.Id,
                message = "Step executed successfully"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetProcesses([FromQuery] ProcessQuery query)
    {
        try
        {
            var processes = await _processService.GetProcessesAsync(query);
            return Ok(processes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}