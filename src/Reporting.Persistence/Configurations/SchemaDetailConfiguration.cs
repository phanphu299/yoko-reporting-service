using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class SchemaDetailConfiguration : IEntityTypeConfiguration<Domain.Entity.SchemaDetail>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.SchemaDetail> builder)
        {
            builder.ToTable("schema_details");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.IsRequired).HasColumnName("is_required");
            builder.Property(x => x.IsReadonly).HasColumnName("is_readonly");
            builder.Property(x => x.PlaceHolder).HasColumnName("place_holder");
            builder.Property(x => x.DataType).HasColumnName("data_type");
            builder.Property(x => x.SchemaId).HasColumnName("schema_id");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.HasOne(x => x.Schema).WithMany(x => x.Details).HasForeignKey(x => x.SchemaId);
        }
    }
}