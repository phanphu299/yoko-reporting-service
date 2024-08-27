using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class FailedScheduleConfiguration : IEntityTypeConfiguration<Domain.Entity.FailedSchedule>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.FailedSchedule> builder)
        {
            builder.ToTable("failed_schedules");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.ScheduleName).HasColumnName("schedule_name");
            builder.Property(x => x.ScheduleId).HasColumnName("schedule_id");
            builder.Property(x => x.JobId).HasColumnName("job_id");
            builder.Property(x => x.TimeZoneName).HasColumnName("timezone_name");
            builder.Property(x => x.ExecutionTime).HasColumnName("execution_time");
            builder.Property(x => x.NextExecutionTime).HasColumnName("next_execution_time");
            builder.Property(x => x.PreviousExecutionTime).HasColumnName("previous_execution_time");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
        }
    }
}