using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class ScheduleTypeConfiguration : IEntityTypeConfiguration<Domain.Entity.ScheduleType>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.ScheduleType> builder)
        {
            builder.ToTable("schedule_types");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.LocalizationKey).HasColumnName("localization_key");
            builder.Property(x => x.Deleted).HasColumnName("deleted");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
        }
    }
}