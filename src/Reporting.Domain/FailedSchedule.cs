using AHI.Infrastructure.Repository.Model.Generic;
using System;

namespace Reporting.Domain.Entity
{
    public class FailedSchedule : IEntity<int>
    {
        public int Id { get; set; }
        public string ScheduleName { get; set; }
        public int ScheduleId { get; set; }
        public string JobId { get; set; }
        public string TimeZoneName { get; set; }
        public long ExecutionTime { get; set; }
        public long NextExecutionTime { get; set; }
        public long PreviousExecutionTime { get; set; }
        public bool Deleted { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public DateTime? UpdatedUtc { get; set; }
    }
}