using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface IFileEventService
    {
        Task SendExportEventAsync(Guid activityId, string objectType, IEnumerable<string> data);
        Task SendExportReportScheduleEventAsync(Guid activityId, string objectType, IEnumerable<string> scheduleNames, string templateId, IEnumerable<string> scheduleIds);
    }
}