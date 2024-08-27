using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class ScheduleJobConfiguration : IEntityTypeConfiguration<Domain.Entity.ScheduleJob>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.ScheduleJob> builder)
        {
            builder.ToTable("schedule_jobs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.JobId).HasColumnName("job_id");
            builder.Property(x => x.ScheduleId).HasColumnName("schedule_id");
        }
    }
}