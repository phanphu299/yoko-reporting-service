using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class DataSourceType : IEntity<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }
    }
}