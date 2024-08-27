using System;
using MediatR;

namespace Reporting.Application.Command
{
    public class TriggerExecution : IRequest
    {
        public Guid ScheduleExecutionId { get; set; }
        public string RetryJobId { get; set; }


        public TriggerExecution(Guid scheduleExecutionId, string retryJobId = null)
        {
            ScheduleExecutionId = scheduleExecutionId;
            RetryJobId = retryJobId;
        }
    }
}