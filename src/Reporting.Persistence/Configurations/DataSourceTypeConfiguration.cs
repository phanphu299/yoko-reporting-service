using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class DataSourceTypeConfiguration : IEntityTypeConfiguration<Domain.Entity.DataSourceType>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.DataSourceType> builder)
        {
            builder.ToTable("data_source_types");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
        }
    }
}