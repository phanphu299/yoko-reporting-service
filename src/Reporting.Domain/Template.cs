using System;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class Template : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TemplateFileUrl { get; set; }
        public string Default { get; set; }
        public int StorageId { get; set; }
        public string OutputTypeId { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }
        public OutputType OutputType { get; set; }
        public Storage Storage { get; set; }

        public ICollection<TemplateDetail> Details { get; private set; }
        public ICollection<Report> Reports { get; private set; }
        public ICollection<ScheduleTemplate> ScheduleTemplates { get; set; }
        public ICollection<EntityTagDb> EntityTags { get; set; }

        public Template()
        {
            Details = new List<TemplateDetail>();
            Reports = new List<Report>();
            EntityTags ??= new List<EntityTagDb>();
        }

        public Template(ICollection<TemplateDetail> details)
        {
            Details = details;
        }
    }
}