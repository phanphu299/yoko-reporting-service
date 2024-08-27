using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class Storage : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeId { get; set; }
        public string Content { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }

        public StorageType Type { get; set; }
    }
}