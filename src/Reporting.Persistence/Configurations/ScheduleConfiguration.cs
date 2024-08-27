using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Domain.Entity.Schedule>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.Schedule> builder)
        {
            builder.ToTable("schedules");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.CronExpressionId).HasColumnName("cron_expression_id");
            builder.Property(x => x.IsSwitchedToCron).HasColumnName("is_switched_to_cron");
            builder.Property(x => x.Cron).HasColumnName("cron");
            builder.Property(x => x.CronDescription).HasColumnName("cron_description");
            builder.Property(x => x.TimeZoneName).HasColumnName("timezone_name");
            builder.Property(x => x.AdditionalParams).HasColumnName("additional_params");
            builder.Property(x => x.JobId).HasColumnName("job_id");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by");
            builder.Property(x => x.ResourcePath).HasColumnName("resource_path");
            builder.Property(x => x.LastRunUtc).HasColumnName("last_run_utc");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.Type).HasColumnName("type");
            builder.Property(x => x.Period).HasColumnName("period");

            builder.HasMany(x => x.ScheduleTemplates).WithOne(x => x.Schedule).HasForeignKey(x => x.ScheduleId);
            builder.HasMany(x => x.ScheduleJobs).WithOne(x => x.Schedule).HasForeignKey(x => x.ScheduleId);
            builder.HasMany(x => x.EntityTags).WithOne(x => x.Schedule).HasForeignKey(x => x.EntityIdInt).OnDelete(DeleteBehavior.Cascade);
        }
    }
}