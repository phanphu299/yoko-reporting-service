using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reporting.Persistence.Configuration
{
    public class ScheduleContactConfiguration : IEntityTypeConfiguration<Domain.Entity.SchedulerContact>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.SchedulerContact> builder)
        {
            builder.ToTable("schedule_contacts");
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ScheduleId).HasColumnName("schedule_id");
            builder.Property(x => x.ObjectId).HasColumnName("object_id");
            builder.Property(x => x.ObjectType).HasColumnName("object_type").HasConversion<string>();
            builder.Property(x => x.SequentialNumber).HasColumnName("sequential_number");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.Deleted).HasColumnName("deleted");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.HasOne(x => x.Schedule).WithMany(x => x.Contacts).HasForeignKey(x => x.ScheduleId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}