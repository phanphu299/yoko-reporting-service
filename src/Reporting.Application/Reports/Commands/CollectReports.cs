using MediatR;
using System;
using System.Collections.Generic;

namespace Reporting.Application.Command
{
    public class CollectReports : IRequest
    {
        public string RetryJobId { get; set; }
        public Guid ExecutionId { get; set; }
    }

    public class CollectReportsParameters
    {
        public List<int> ReportGenerationScheduleIds { get; set; }
        public string TimeZoneName { get; set; }
        public DateTime ExecutionTimeUtc { get; set; }
        public string Period { get; set; }
    }
}