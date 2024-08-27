using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reporting.Domain.EnumType;

namespace Reporting.Application.Command.Model
{
    public class ScheduleContactDto
    {
        public Guid Id { get; set; }
        public int ScheduleId { get; set; }
        public Guid ObjectId { get; set; }
        public int SequentialNumber { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ContactType ObjectType { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { set; get; }

        private static Func<Domain.Entity.SchedulerContact, ScheduleContactDto> DtoConverter = DtoProjection.Compile();
        private static Expression<Func<Domain.Entity.SchedulerContact, ScheduleContactDto>> DtoProjection
        {
            get
            {
                return entity => new ScheduleContactDto
                {
                    Id = entity.Id,
                    ScheduleId = entity.ScheduleId,
                    ObjectId = entity.ObjectId,
                    ObjectType = entity.ObjectType,
                    SequentialNumber = entity.SequentialNumber,
                    CreatedUtc = entity.CreatedUtc,
                    UpdatedUtc = entity.UpdatedUtc,
                    Deleted = entity.Deleted
                };
            }
        }

        public static ScheduleContactDto CreateDto(Domain.Entity.SchedulerContact entity)
        {
            if (entity == null)
                return null;
            return DtoConverter(entity);
        }

        private static Func<ScheduleContactDto, Domain.Entity.SchedulerContact> EntityConverter = EntityProjection.Compile();
        private static Expression<Func<ScheduleContactDto, Domain.Entity.SchedulerContact>> EntityProjection
        {
            get
            {
                return model => new Domain.Entity.SchedulerContact
                {
                    ScheduleId = model.ScheduleId,
                    ObjectId = model.ObjectId,
                    ObjectType = model.ObjectType,
                    SequentialNumber = model.SequentialNumber,
                    CreatedUtc = model.CreatedUtc,
                    UpdatedUtc = model.UpdatedUtc
                };
            }
        }

        public static Domain.Entity.SchedulerContact CreateEntity(ScheduleContactDto model)
        {
            if (model is null)
                return null;

            return EntityConverter(model);
        }
    }
}