using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class ScheduleType : IEntity<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LocalizationKey { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }
    }
}