using System.Collections.Generic;

namespace Reporting.Function.Model
{
    public class DownloadReportRequest
    {
        public IEnumerable<string> Ids { get; set; }
        public string TemplateId { get; set; }
        public IEnumerable<string> ScheduleIds { get; set; }
        public IEnumerable<string> ScheduleNames { get; set; }
    }
}