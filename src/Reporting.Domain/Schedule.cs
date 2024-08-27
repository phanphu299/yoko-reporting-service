using System;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class Schedule : IEntity<int>
    {
        public Schedule()
        {
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = DateTime.UtcNow;
        }

        public Schedule(IEnumerable<SchedulerContact> contacts) : this()
        {
            Contacts ??= new List<SchedulerContact>();
            foreach (var contact in contacts)
            {
                Contacts.Add(contact);
            }

            EntityTags ??= new List<EntityTagDb>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Guid? CronExpressionId { get; set; }
        public bool IsSwitchedToCron { get; set; }
        public string Cron { get; set; }
        public string CronDescription { get; set; }
        public string TimeZoneName { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public string AdditionalParams { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string JobId { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime? LastRunUtc { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }
        public string Type { get; set; }
        public string Period { get; set; }

        public ICollection<ScheduleTemplate> ScheduleTemplates { get; set; }
        public ICollection<SchedulerContact> Contacts { get; private set; }
        public ICollection<ScheduleJob> ScheduleJobs { get; set; }
        public ICollection<EntityTagDb> EntityTags { get; set; }
    }
}