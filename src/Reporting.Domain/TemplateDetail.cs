using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class TemplateDetail : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public string DataSourceTypeId { get; set; }
        public string DataSourceContent { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }

        public DataSourceType DataSourceType { get; set; }
        public Template Template { get; set; }
    }
}