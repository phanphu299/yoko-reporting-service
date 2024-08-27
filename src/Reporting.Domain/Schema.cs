using System;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class Schema : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }

        public ICollection<SchemaDetail> Details { get; private set; }

        public Schema()
        {
            Details = new List<SchemaDetail>();
        }
    }
}