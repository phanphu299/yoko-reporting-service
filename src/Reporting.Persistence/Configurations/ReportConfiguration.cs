using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reporting.Domain.Entity;

namespace Reporting.Persistence.Configuration
{
    public class ReportConfiguration : IEntityTypeConfiguration<Domain.Entity.Report>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.Report> builder)
        {
            builder.ToTable("reports");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.FromDateUtc).HasColumnName("from_date_utc");
            builder.Property(x => x.ToDateUtc).HasColumnName("to_date_utc");
            builder.Property(x => x.StorageUrl).HasColumnName("storage_url");
            builder.Property(x => x.FileName).HasColumnName("file_name");
            builder.Property(x => x.TemplateId).HasColumnName("template_id");
            builder.Property(x => x.StorageId).HasColumnName("storage_id");
            builder.Property(x => x.TemplateName).HasColumnName("template_name");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by");
            builder.Property(x => x.ResourcePath).HasColumnName("resource_path"); 
            builder.Property(x => x.OutputTypeId).HasColumnName("output_type_id");
            builder.Property(x => x.ScheduleId).HasColumnName("schedule_id");
            builder.Property(x => x.ScheduleName).HasColumnName("schedule_name");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.ScheduleExecutionId).HasColumnName("schedule_execution_id");
            builder.HasOne<ScheduleExecution>().WithMany().HasForeignKey(x => x.ScheduleExecutionId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}