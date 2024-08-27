using System;
using System.Linq.Expressions;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Handler.Command
{
    public class AddScheduleContact : BaseScheduleContact
    {
        static Func<AddScheduleContact, int, Domain.Entity.SchedulerContact> Converter = Projection.Compile();
        static Expression<Func<AddScheduleContact, int, Domain.Entity.SchedulerContact>> Projection
        {
            get
            {
                return (command, index) => new Domain.Entity.SchedulerContact
                {
                    ObjectId = command.ObjectId,
                    ObjectType = command.ObjectType,
                    SequentialNumber = index,
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.SchedulerContact Create(AddScheduleContact command, int index)
        {
            return Converter(command, index);
        }
    }
}