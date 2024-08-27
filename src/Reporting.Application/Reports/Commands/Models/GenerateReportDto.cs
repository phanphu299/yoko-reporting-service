using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class GenerateReportDto
    {
        public int Id { get; set; }
        public string JobId { get; set; }
        public string Name { get; set; }
        public int? TemplateId { get; set; }
        public string StorageUrl { get; set; }

        static Func<Domain.Entity.Report, GenerateReportDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.Report, GenerateReportDto>> Projection
        {
            get
            {
                return entity => new GenerateReportDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    TemplateId = entity.TemplateId,
                    StorageUrl = entity.StorageUrl
                };
            }
        }

        public static GenerateReportDto Create(Domain.Entity.Report entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}