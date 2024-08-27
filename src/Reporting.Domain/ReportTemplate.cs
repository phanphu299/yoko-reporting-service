using AHI.Infrastructure.Repository.Model.Generic;
using System;

namespace Reporting.Domain.Entity
{
    public class ReportTemplate : IEntity<long>
    {
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public bool HasDeletedTemplate { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime CreatedUtc { get; set; }
        public long Id { get; set; }

        public ReportTemplate()
        {
        }
    }
}