using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class DataSourceContentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DataSourceTypeId { get; set; }
        public string DataSourceContent { get; set; }
        public string DeletedContent { get; set; }
        public DataSourceTypeDto DataSourceType { get; set; }
        public bool Deleted { get; set; }

        private static Func<Domain.Entity.TemplateDetail, DataSourceContentDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.TemplateDetail, DataSourceContentDto>> Projection
        {
            get
            {
                return entity => new DataSourceContentDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    DataSourceTypeId = entity.DataSourceTypeId,
                    DataSourceType = DataSourceTypeDto.Create(entity.DataSourceType),
                    DataSourceContent = entity.DataSourceContent,
                    Deleted = entity.Deleted
                };
            }
        }

        public static DataSourceContentDto Create(Domain.Entity.TemplateDetail entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}