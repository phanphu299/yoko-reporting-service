using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class StorageConfiguration : IEntityTypeConfiguration<Domain.Entity.Storage>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.Storage> builder)
        {
            builder.ToTable("storages");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.TypeId).HasColumnName("type_id");
            builder.Property(x => x.CanEdit).HasColumnName("can_edit");
            builder.Property(x => x.CanDelete).HasColumnName("can_delete");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.HasOne(x => x.Type).WithMany(x => x.Storages).HasForeignKey(x => x.TypeId);
        }
    }
}