using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class ScheduleTemplateConfiguration : IEntityTypeConfiguration<Domain.Entity.ScheduleTemplate>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.ScheduleTemplate> builder)
        {
            builder.ToTable("schedule_templates");
            builder.HasKey(x => new { x.TemplateId, x.ScheduleId });
            builder.Property(x => x.TemplateId).HasColumnName("template_id");
            builder.Property(x => x.ScheduleId).HasColumnName("schedule_id");
        }
    }
}