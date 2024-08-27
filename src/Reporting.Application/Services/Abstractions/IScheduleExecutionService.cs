using System;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface IScheduleExecutionService
    {
        Task TriggerExecutionAsync(Guid executionId, string retryJobId);
    }
}