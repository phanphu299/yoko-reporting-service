using System;
using AHI.Infrastructure.Repository.Model.Generic;
using Reporting.Domain.EnumType;

namespace Reporting.Domain.Entity
{
    public class SchedulerContact : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public int ScheduleId { get; set; }
        public Guid ObjectId { get; set; }
        public ContactType ObjectType { get; set; }
        public int SequentialNumber { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { set; get; }

        public Schedule Schedule { get; set; }
    }
}