using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class SchemaDetail : IEntity<int>
    {
        public int Id { get; set; }
        public int SchemaId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
        public bool IsReadonly { get; set; }
        public string PlaceHolder { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { get; set; }

        public Schema Schema { get; set; }
    }
}