using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reporting.Domain.Entity;

namespace Reporting.Persistence.Configuration
{
    public class ScheduleExecutionConfiguration : IEntityTypeConfiguration<ScheduleExecution>
    {
        public void Configure(EntityTypeBuilder<ScheduleExecution> builder)
        {
            builder.ToTable("schedule_executions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ScheduleId).HasColumnName("schedule_id");
            builder.Property(x => x.State).HasColumnName("state").HasConversion<string>();
            builder.Property(x => x.MaxRetryCount).HasColumnName("max_retry_count");
            builder.Property(x => x.RetryCount).HasColumnName("retry_count");
            builder.Property(x => x.RetryJobId).HasColumnName("retry_job_id");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.ExecutionParam).HasColumnName("execution_param");
            builder.HasOne(x => x.Schedule).WithMany().HasForeignKey(x => x.ScheduleId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}