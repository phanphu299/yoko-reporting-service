using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public int? TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string StorageUrl { get; set; }
        public string FileName { get; set; }
        public string Overridden { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime CreatedUtc { get; set; }

        public OutputTypeDto OutputType { get; set; }

        private static Func<Domain.Entity.Report, ReportDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.Report, ReportDto>> Projection
        {
            get
            {
                return entity => new ReportDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    TemplateId = entity.TemplateId,
                    TemplateName = entity.TemplateName,
                    StorageUrl = entity.StorageUrl,
                    FileName = entity.FileName,
                    Overridden = entity.Overridden,
                    CreatedUtc = entity.CreatedUtc,
                    ScheduleId = entity.ScheduleId,
                    ScheduleName = entity.ScheduleName,
                    CreatedBy = entity.CreatedBy,
                    ResourcePath = entity.ResourcePath,
                    OutputType = OutputTypeDto.Create(entity.OutputType)
                };
            }
        }

        public static ReportDto Create(Domain.Entity.Report entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}