using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface INotificationHandler
    {
        Task SendGeneratedReportsAsync(ScheduleExecution execution, ReportAndSendParameter parameters);
        Task SendCollectedReportsAsync(ScheduleExecution execution, CollectReportsParameters parameters,
            List<Report> reports, List<string> failedSchedules, List<string> partialSuccessSchedules);
    }
}