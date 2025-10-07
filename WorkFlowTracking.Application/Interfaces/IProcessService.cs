using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowTracking.Application.DTOs;
using WorkFlowTracking.Domain.Entities;

namespace WorkFlowTracking.Application.Interfaces
{
    public interface IProcessService
    {
        Task<Process> StartProcessAsync(StartProcessRequest request);
        Task<ProcessStep> ExecuteStepAsync(ExecuteStepRequest request);
        Task<IEnumerable<Process>> GetProcessesAsync(ProcessQuery query);
    }
}
