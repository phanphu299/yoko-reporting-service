using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class StorageTypeConfiguration : IEntityTypeConfiguration<Domain.Entity.StorageType>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.StorageType> builder)
        {
            builder.ToTable("storage_types");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.CanRead).HasColumnName("can_read");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.HasMany(x => x.Storages).WithOne(x => x.Type).HasForeignKey(x => x.TypeId);
        }
    }
}