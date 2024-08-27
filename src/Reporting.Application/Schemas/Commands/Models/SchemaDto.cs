using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Reporting.Application.Command.Model
{
    public class SchemaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime CreatedUtc { get; set; }

        public IEnumerable<SchemaDetailDto> Details { get; set; }

        private static Func<Domain.Entity.Schema, SchemaDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.Schema, SchemaDto>> Projection
        {
            get
            {
                return entity => new SchemaDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Type = entity.Type,
                    CreatedUtc = entity.CreatedUtc,
                    Details = entity.Details.Select(SchemaDetailDto.Create)
                };
            }
        }

        public static SchemaDto Create(Domain.Entity.Schema entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}