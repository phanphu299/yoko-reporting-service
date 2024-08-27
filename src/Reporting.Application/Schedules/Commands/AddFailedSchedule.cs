using MediatR;
using Reporting.Application.Command.Model;
using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command
{
    public class AddFailedSchedule : IRequest<FailedScheduleDto>
    {
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

        private static Func<GenerateReport, Domain.Entity.FailedSchedule> Converter = Projection.Compile();

        private static Expression<Func<GenerateReport, Domain.Entity.FailedSchedule>> Projection
        {
            get
            {
                return command => new Domain.Entity.FailedSchedule
                {
                    JobId = command.JobId,
                    TimeZoneName = command.TimeZoneName,
                    ExecutionTime = command.ExecutionTime,
                    NextExecutionTime = command.NextExecutionTime,
                    PreviousExecutionTime = command.PreviousExecutionTime,
                };
            }
        }

        public static Domain.Entity.FailedSchedule Create(GenerateReport command)
        {
            if (command == null)
                return null;
            return Converter(command);
        }
    }
}