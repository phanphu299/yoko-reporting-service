using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class OutputTypeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }

        private static Func<Domain.Entity.OutputType, OutputTypeDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.OutputType, OutputTypeDto>> Projection
        {
            get
            {
                return entity => new OutputTypeDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Extension = entity.Extension
                };
            }
        }

        public static OutputTypeDto Create(Domain.Entity.OutputType entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}