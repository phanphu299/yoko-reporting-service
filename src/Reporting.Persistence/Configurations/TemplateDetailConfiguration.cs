using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class TemplateDetailConfiguration : IEntityTypeConfiguration<Domain.Entity.TemplateDetail>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.TemplateDetail> builder)
        {
            builder.ToTable("template_details");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.TemplateId).HasColumnName("template_id");
            builder.Property(x => x.DataSourceTypeId).HasColumnName("data_source_type_id");
            builder.Property(x => x.DataSourceContent).HasColumnName("data_source_content");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.HasOne(x => x.Template).WithMany(x => x.Details).HasForeignKey(x => x.TemplateId);
        }
    }
}