using MediatR;
using System;

namespace Reporting.Application.Command
{
    public class TriggerRetryCollectReports : IRequest
    {
        public string JobId { get; set; }
        public Guid ScheduleExecutionId { get; set; }
    }
}