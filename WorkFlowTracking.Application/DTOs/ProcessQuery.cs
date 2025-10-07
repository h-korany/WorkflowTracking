using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowTracking.Domain.Enums;

namespace WorkFlowTracking.Application.DTOs
{
    public class ProcessQuery
    {
        public Guid? WorkflowId { get; set; }
        public ProcessStatus? Status { get; set; }
        public string? AssignedTo { get; set; }
    }
}
