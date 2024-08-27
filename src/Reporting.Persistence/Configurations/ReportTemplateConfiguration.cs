using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class ReportTemplateConfiguration : IEntityTypeConfiguration<Domain.Entity.ReportTemplate>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.ReportTemplate> builder)
        {
            builder.ToView("v_report_templates");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.TemplateId).HasColumnName("template_id");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by");
            builder.Property(x => x.ResourcePath).HasColumnName("resource_path");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.HasDeletedTemplate).HasColumnName("has_deleted_template");
        }
    }
}