using AHI.Infrastructure.Service.Tag.SqlServer.Configuration;
using Microsoft.EntityFrameworkCore;
using Reporting.Persistence.Configuration;

namespace Reporting.Persistence.Context
{
    public class ReportingDbContext : DbContext
    {
        public DbSet<Domain.Entity.Schema> Schemas { get; set; }
        public DbSet<Domain.Entity.Storage> Storages { get; set; }
        public DbSet<Domain.Entity.Template> Templates { get; set; }
        public DbSet<Domain.Entity.Report> Reports { get; set; }
        public DbSet<Domain.Entity.StorageType> StorageTypes { get; set; }
        public DbSet<Domain.Entity.OutputType> OutputTypes { get; set; }
        public DbSet<Domain.Entity.DataSourceType> DataSourceTypes { get; set; }
        public DbSet<Domain.Entity.Schedule> Schedules { get; set; }
        public DbSet<Domain.Entity.ScheduleType> ScheduleTypes { get; set; }
        public DbSet<Domain.Entity.ScheduleTemplate> ScheduleTemplates { get; set; }
        public DbSet<Domain.Entity.ScheduleJob> ScheduleJobs { get; set; }
        public DbSet<Domain.Entity.ReportTemplate> ReportTemplates { get; set; }
        public DbSet<Domain.Entity.ReportSchedule> ReportSchedules { get; set; }
        public DbSet<Domain.Entity.FailedSchedule> FailedSchedules { get; set; }
        public DbSet<Domain.Entity.ScheduleExecution> ScheduleExecutions { get; set; }
        public DbSet<Domain.Entity.SchedulerContact> ScheduleContacts { get; set; }
        public DbSet<Domain.Entity.EntityTagDb> EntityTags { get; set; }

        public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScheduleConfiguration).Assembly);
            modelBuilder.ApplyConfiguration(new ScheduleContactConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduleExecutionConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduleContactConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduleExecutionConfiguration());
            modelBuilder.ApplyConfiguration(new EntityTagConfiguration<Domain.Entity.EntityTagDb>());
        }
    }
}