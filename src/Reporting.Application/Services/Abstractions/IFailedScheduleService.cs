using AHI.Infrastructure.Service.Abstraction;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface IFailedScheduleService : ISearchService<Domain.Entity.FailedSchedule, int, SearchFailedSchedule, FailedScheduleDto>
    {
        Task<IEnumerable<GenerateReportDto>> RunFailedScheduleAsync(RunFailedSchedule command);
    }
}