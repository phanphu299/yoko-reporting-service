using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class Report : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? TemplateId { get; set; }
        public int? StorageId { get; set; }
        public string Overridden { get; set; }
        public string OutputTypeId { get; set; }
        public DateTime FromDateUtc { get; set; }
        public DateTime ToDateUtc { get; set; }
        public string StorageUrl { get; set; }
        public string FileName { get; set; }
        public int? ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public string TemplateName { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }
        public Guid? ScheduleExecutionId { get; set; }

        public OutputType OutputType { get; set; }
        public Template Template { get; set; }
    }
}