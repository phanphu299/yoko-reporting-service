using System;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class StorageType : IEntity<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool CanRead { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }

        public ICollection<Storage> Storages { get; set; }

        public StorageType()
        {
            Storages = new List<Storage>();
        }
    }
}