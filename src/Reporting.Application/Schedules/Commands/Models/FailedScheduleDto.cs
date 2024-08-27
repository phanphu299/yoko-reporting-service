using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class FailedScheduleDto
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public string JobId { get; set; }
        public string TimeZoneName { get; set; }
        public long ExecutionTime { get; set; }
        public long NextExecutionTime { get; set; }
        public long PreviousExecutionTime { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public DateTime? UpdatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }

        private static Func<Domain.Entity.FailedSchedule, FailedScheduleDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.FailedSchedule, FailedScheduleDto>> Projection
        {
            get
            {
                return entity => new FailedScheduleDto
                {
                    Id = entity.Id,
                    ScheduleId = entity.ScheduleId,
                    ScheduleName = entity.ScheduleName,
                    JobId = entity.JobId,
                    TimeZoneName = entity.TimeZoneName,
                    ExecutionTime = entity.ExecutionTime,
                    NextExecutionTime = entity.NextExecutionTime,
                    PreviousExecutionTime = entity.PreviousExecutionTime,
                    CreatedUtc = entity.CreatedUtc,
                    UpdatedUtc = entity.UpdatedUtc,
                };
            }
        }

        public static FailedScheduleDto Create(Domain.Entity.FailedSchedule entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}