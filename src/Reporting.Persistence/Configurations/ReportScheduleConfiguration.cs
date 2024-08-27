using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class ReportScheduleConfiguration : IEntityTypeConfiguration<Domain.Entity.ReportSchedule>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.ReportSchedule> builder)
        {
            builder.ToView("v_report_schedules");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.TemplateId).HasColumnName("template_id");
            builder.Property(x => x.CronDescription).HasColumnName("cron_description");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by");
            builder.Property(x => x.ResourcePath).HasColumnName("resource_path");
            builder.Property(x => x.LastRunUtc).HasColumnName("last_run_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.HasDeletedSchedule).HasColumnName("has_deleted_schedule");
        }
    }
}