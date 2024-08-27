using AHI.Infrastructure.Repository.Generic;

namespace Reporting.Application.Repository
{
    public interface IReportScheduleRepository : ISearchRepository<Domain.Entity.ReportSchedule, int>
    {
    }
}