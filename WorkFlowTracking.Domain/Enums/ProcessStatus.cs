using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowTracking.Domain.Enums
{
    public enum ProcessStatus
    {
        Pending = 1,
        Active = 2,
        Completed = 3,
        Rejected = 4,
        Cancelled = 5
    }
}
