using MediatR;
using System;

namespace Reporting.Application.Command
{
    public class TriggerCollectReports : IRequest
    {
        public string JobId { get; set; }
        public string TimeZoneName { get; set; }
        public DateTime ExecutionTimeUtc { get; set; }
    }
}