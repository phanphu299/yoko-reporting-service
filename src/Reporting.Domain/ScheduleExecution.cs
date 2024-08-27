using System;
using AHI.Infrastructure.Repository.Model.Generic;
using Reporting.Domain.EnumType;

namespace Reporting.Domain.Entity
{
    public class ScheduleExecution : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public int ScheduleId { get; set; }
        public ExecutionState State { get; set; } = ExecutionState.INIT;
        public int MaxRetryCount { get; set; }
        public int RetryCount { get; set; } = 0;
        public string RetryJobId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public string ExecutionParam { get; set; }

        public Schedule Schedule { get; set; }

        public ScheduleExecution()
        {
            CreatedUtc = UpdatedUtc = DateTime.UtcNow;
        }
    }
}