using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;
using Reporting.Persistence.Repository;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.Service.Tag.Extension;

namespace Reporting.Persistence.Extension
{
    public static class PersistenceExtensions
    {
        public static void AddPersistenceService(this IServiceCollection serviceCollection, bool isDevelopment = true)
        {
            serviceCollection.AddDbContext<ReportingDbContext>((service, option) =>
            {
                var configuration = service.GetService<IConfiguration>();
                var tenantContext = service.GetRequiredService<ITenantContext>();
                var connectionString = configuration["ConnectionStrings:Default"].BuildConnectionString(configuration, tenantContext.ProjectId);
                option.UseSqlServer(connectionString);
            });

            serviceCollection.AddMemoryCache();

            serviceCollection.AddScoped<ISchemaRepository, SchemaPersistenceRepository>();
            serviceCollection.AddScoped<IStorageTypeRepository, StorageTypePersistenceRepository>();
            serviceCollection.AddScoped<IOutputTypeRepository, OutputTypePersistenceRepository>();
            serviceCollection.AddScoped<IDataSourceTypeRepository, DataSourceTypePersistenceRepository>();
            serviceCollection.AddScoped<IStorageRepository, StoragePersistenceRepository>();
            serviceCollection.AddScoped<IReportTemplateRepository, ReportTemplatePersistenceRepository>();
            serviceCollection.AddScoped<ITemplateRepository, TemplatePersistenceRepository>();
            serviceCollection.AddScoped<IReportRepository, ReportPersistenceRepository>();
            serviceCollection.AddScoped<IReportScheduleRepository, ReportSchedulePersistenceRepository>();
            serviceCollection.AddScoped<IScheduleRepository, SchedulePersistenceRepository>();
            serviceCollection.AddScoped<IScheduleTypeRepository, ScheduleTypePersistenceRepository>();
            serviceCollection.AddScoped<IFailedScheduleRepository, FailedSchedulePersistenceRepository>();
            serviceCollection.AddScoped<IScheduleTemplateRepository, ScheduleTemplatePersistenceRepository>();
            serviceCollection.AddScoped<IScheduleContactRepository, ScheduleContactRepository>();
            serviceCollection.AddScoped<IScheduleExecutionRepository, ScheduleExecutionRepository>();
            serviceCollection.AddScoped<IScheduleJobRepository, ScheduleJobPersistenceRepository>();
            serviceCollection.AddScoped<IReportingUnitOfWork, ReportingUnitOfWork>();

            serviceCollection.AddEntityTagRepository(typeof(ReportingDbContext));
        }
    }
}