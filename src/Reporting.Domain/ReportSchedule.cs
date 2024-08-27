using AHI.Infrastructure.Repository.Model.Generic;
using System;

namespace Reporting.Domain.Entity
{
    public class ReportSchedule : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public string CronDescription { get; set; }
        public bool HasDeletedSchedule { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public DateTime? LastRunUtc { get; set; }
    }
}