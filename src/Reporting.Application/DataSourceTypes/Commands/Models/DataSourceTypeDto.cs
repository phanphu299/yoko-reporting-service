using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class DataSourceTypeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        private static Func<Domain.Entity.DataSourceType, DataSourceTypeDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.DataSourceType, DataSourceTypeDto>> Projection
        {
            get
            {
                return entity => new DataSourceTypeDto
                {
                    Id = entity.Id,
                    Name = entity.Name
                };
            }
        }

        public static DataSourceTypeDto Create(Domain.Entity.DataSourceType entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}