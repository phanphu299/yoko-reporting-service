using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class OutputTypeConfiguration : IEntityTypeConfiguration<Domain.Entity.OutputType>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.OutputType> builder)
        {
            builder.ToTable("output_types");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
        }
    }
}