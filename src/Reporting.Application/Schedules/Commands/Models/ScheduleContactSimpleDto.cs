using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reporting.Domain.EnumType;

namespace Reporting.Application.Command.Model
{
    public class ScheduleContactSimpleDto
    {
        public Guid ObjectId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ContactType ObjectType{ get; set; }

        private static Func<Domain.Entity.SchedulerContact, ScheduleContactSimpleDto> DtoConverter = DtoProjection.Compile();
        private static Expression<Func<Domain.Entity.SchedulerContact, ScheduleContactSimpleDto>> DtoProjection
        {
            get
            {
                return entity => new ScheduleContactSimpleDto
                {
                    ObjectId = entity.ObjectId,
                    ObjectType = entity.ObjectType,
                };
            }
        }

        public static ScheduleContactSimpleDto CreateDto(Domain.Entity.SchedulerContact entity)
        {
            if (entity == null)
                return null;
            return DtoConverter(entity);
        }
    }
}