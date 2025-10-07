using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowTracking.Application.DTOs;

namespace WorkFlowTracking.Application.Interfaces
{
    public interface IWorkflowService
    {
        Task<WorkflowDto> CreateWorkflowAsync(WorkflowDto workflowDto);
        Task<WorkflowDto?> GetWorkflowAsync(Guid id);
        Task<IEnumerable<WorkflowDto>> GetAllWorkflowsAsync();
        Task<WorkflowDto> UpdateWorkflowAsync(Guid id, WorkflowDto workflowDto); // Added update method
        Task<bool> DeleteWorkflowAsync(Guid id);
    }
}
