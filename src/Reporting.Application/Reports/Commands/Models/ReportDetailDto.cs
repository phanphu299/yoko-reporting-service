using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class ReportDetailDto
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; }
        public string StorageUrl { get; set; }
        public string StorageToken { get; set; }
        public string FileName { get; set; }
        public string Overridden { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime CreatedUtc { get; set; }

        static Func<Domain.Entity.Report, ReportDetailDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.Report, ReportDetailDto>> Projection
        {
            get
            {
                return entity => new ReportDetailDto
                {
                    Id = entity.Id,
                    TemplateId = entity.TemplateId,
                    StorageUrl = entity.StorageUrl,
                    FileName = entity.FileName,
                    Overridden = entity.Overridden,
                    CreatedUtc = entity.CreatedUtc,
                    CreatedBy = entity.CreatedBy,
                    ResourcePath = entity.ResourcePath,
                };
            }
        }

        public static ReportDetailDto Create(Domain.Entity.Report entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}