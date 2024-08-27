using System.Threading.Tasks;
using Reporting.Application.Command.Model;
using Reporting.Domain.Entity;

namespace Reporting.Application.Service.Abstraction
{
    public interface IReportProcessor
    {
        Task ProcessAsync(ScheduleExecution execution, IExecutionParameter executionParameter);
    }
}