using System;
using System.Linq.Expressions;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Handler.Command
{
    public class UpdateScheduleContact : BaseScheduleContact
    {
        public Guid Id { get; set; }

        static Func<int, UpdateScheduleContact, int, Domain.Entity.SchedulerContact> Converter = Projection.Compile();
        static Expression<Func<int, UpdateScheduleContact, int, Domain.Entity.SchedulerContact>> Projection
        {
            get
            {
                return (scheduleId, command, index) => new Domain.Entity.SchedulerContact
                {
                    Id = command.Id,
                    ScheduleId = scheduleId,
                    ObjectId = command.ObjectId,
                    ObjectType = command.ObjectType,
                    SequentialNumber = index,
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.SchedulerContact Create(int scheduleId, UpdateScheduleContact command, int index)
        {
            return Converter(scheduleId, command, index);
        }
    }
}