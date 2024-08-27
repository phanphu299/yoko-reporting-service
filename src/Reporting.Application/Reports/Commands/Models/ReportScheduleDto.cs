using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class ReportScheduleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public string CronDescription { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime? LastRunUtc { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public bool HasDeletedSchedule { get; set; }

        private static Func<Domain.Entity.ReportSchedule, ReportScheduleDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.ReportSchedule, ReportScheduleDto>> Projection
        {
            get
            {
                return entity => new ReportScheduleDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    TemplateId = entity.TemplateId,
                    CronDescription = entity.CronDescription,
                    CreatedBy = entity.CreatedBy,
                    ResourcePath = entity.ResourcePath,
                    CreatedUtc = entity.CreatedUtc,
                    LastRunUtc = entity.LastRunUtc,
                    HasDeletedSchedule = entity.HasDeletedSchedule
                };
            }
        }

        public static ReportScheduleDto Create(Domain.Entity.ReportSchedule entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}