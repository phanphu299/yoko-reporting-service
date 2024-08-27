using Reporting.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface IReportNotifyContentBuilder
    {
        public Task<string> CreateZipReportsAsync(ScheduleExecution execution, string timeZoneName, List<Report> reports);
    }
}