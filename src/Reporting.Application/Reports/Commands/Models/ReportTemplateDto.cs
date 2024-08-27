using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class ReportTemplateDto
    {
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime CreatedUtc { get; set; }
        public bool HasDeletedTemplate { get; set; }

        private static Func<Domain.Entity.ReportTemplate, ReportTemplateDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.ReportTemplate, ReportTemplateDto>> Projection
        {
            get
            {
                return entity => new ReportTemplateDto
                {
                    Name = entity.Name,
                    TemplateId = entity.TemplateId,
                    CreatedBy = entity.CreatedBy,
                    ResourcePath = entity.ResourcePath,
                    CreatedUtc = entity.CreatedUtc,
                    HasDeletedTemplate = entity.HasDeletedTemplate
                };
            }
        }

        public static ReportTemplateDto Create(Domain.Entity.ReportTemplate entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}