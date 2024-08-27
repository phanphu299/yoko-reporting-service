using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class TemplateSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private static Func<Domain.Entity.Template, TemplateSimpleDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.Template, TemplateSimpleDto>> Projection
        {
            get
            {
                return entity => new TemplateSimpleDto
                {
                    Id = entity.Id,
                    Name = entity.Name
                };
            }
        }

        public static TemplateSimpleDto Create(Domain.Entity.Template entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}