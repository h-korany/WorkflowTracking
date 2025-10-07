using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowTracking.Application.Interfaces
{
    public interface IValidationService
    {
        Task<bool> ValidateStepAsync(string stepName, Guid processId, string performedBy);
        Task LogValidationAsync(Guid processId, string stepName, bool isValid, string? message);
    }
}
