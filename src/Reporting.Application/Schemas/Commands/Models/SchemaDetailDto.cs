using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class SchemaDetailDto
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string DataType { get; set; }
        public string PlaceHolder { get; set; }
        public bool IsRequired { get; set; }
        public bool IsReadonly { get; set; }

        private static Func<Domain.Entity.SchemaDetail, SchemaDetailDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.SchemaDetail, SchemaDetailDto>> Projection
        {
            get
            {
                return entity => new SchemaDetailDto
                {
                    Name = entity.Name,
                    Key = entity.Key,
                    DataType = entity.DataType,
                    PlaceHolder = entity.PlaceHolder,
                    IsRequired = entity.IsRequired,
                    IsReadonly = entity.IsReadonly
                };
            }
        }

        public static SchemaDetailDto Create(Domain.Entity.SchemaDetail entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}