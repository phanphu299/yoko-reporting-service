using System;
using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class TriggerGenerateReport : IRequest<BaseResponse>
    {
        public Guid ScheduleExecutionId { get; set; }
        public string RetryJobId { get; set; }

        public TriggerGenerateReport(Guid scheduleExecutionId, string retryJobId)
        {
            ScheduleExecutionId = scheduleExecutionId;
            RetryJobId = retryJobId;
        }
    }
}