using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class TemplateConfiguration : IEntityTypeConfiguration<Domain.Entity.Template>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.Template> builder)
        {
            builder.ToTable("templates");
            builder.Property(x => x.TemplateFileUrl).HasColumnName("template_file_url");
            builder.Property(x => x.StorageId).HasColumnName("storage_id");
            builder.Property(x => x.OutputTypeId).HasColumnName("output_type_id");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by");
            builder.Property(x => x.ResourcePath).HasColumnName("resource_path");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.HasMany(x => x.Details).WithOne(x => x.Template).HasForeignKey(x => x.TemplateId);
            builder.HasMany(x => x.ScheduleTemplates).WithOne(x => x.Template).HasForeignKey(x => x.TemplateId);
            builder.HasMany(x => x.EntityTags).WithOne(x => x.Template).HasForeignKey(x => x.EntityIdInt).OnDelete(DeleteBehavior.Cascade);
        }
    }
}