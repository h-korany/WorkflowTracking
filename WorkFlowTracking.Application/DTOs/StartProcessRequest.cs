using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowTracking.Application.DTOs
{
    public class StartProcessRequest
    {
        public Guid WorkflowId { get; set; }
        public string Initiator { get; set; } = null!;
    }
}
