using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class ScheduleTypeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LocalizationKey { get; set; }

        private static Func<Domain.Entity.ScheduleType, ScheduleTypeDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.ScheduleType, ScheduleTypeDto>> Projection
        {
            get
            {
                return entity => new ScheduleTypeDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    LocalizationKey = entity.LocalizationKey
                };
            }
        }

        public static ScheduleTypeDto Create(Domain.Entity.ScheduleType entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}