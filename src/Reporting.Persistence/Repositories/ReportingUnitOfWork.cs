using AHI.Infrastructure.Repository;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using Reporting.Application.Repository;
using Reporting.Domain.Entity;
using Reporting.Persistence.Context;

namespace Reporting.Persistence.Repository
{
    public class ReportingUnitOfWork : BaseUnitOfWork, IReportingUnitOfWork
    {
        public ITemplateRepository TemplateRepository { get; private set; }
        public IScheduleRepository ScheduleRepository { get; private set; }
        public ISchemaRepository SchemaRepository { get; private set; }
        public IStorageRepository StorageRepository { get; private set; }
        public IReportRepository ReportRepository { get; private set; }
        public IReportTemplateRepository ReportTemplateRepository { get; private set; }
        public IReportScheduleRepository ReportScheduleRepository { get; private set; }
        public IStorageTypeRepository StorageTypeRepository { get; private set; }
        public IOutputTypeRepository OutputTypeRepository { get; private set; }
        public IDataSourceTypeRepository DataSourceTypeRepository { get; private set; }
        public IFailedScheduleRepository FailedScheduleRepository { get; private set; }
        public IScheduleTemplateRepository ScheduleTemplateRepository { get; private set; }
        public IScheduleExecutionRepository ScheduleExecutionRepository { get; private set; }
        public IScheduleContactRepository ScheduleContactRepository { get; private set; }
        public IScheduleJobRepository ScheduleJobRepository { get; private set; }
        public IEntityTagRepository<EntityTagDb> EntityTagRepository { get; private set; }

        public ReportingUnitOfWork(
                ReportingDbContext context,
                ITemplateRepository templateRepository,
                IScheduleRepository scheduleRepository,
                ISchemaRepository schemaRepository,
                IStorageRepository storageRepository,
                IReportRepository reportRepository,
                IReportTemplateRepository reportTemplateRepository,
                IReportScheduleRepository reportScheduleRepository,
                IStorageTypeRepository storageTypeRepository,
                IOutputTypeRepository outputTypeRepository,
                IDataSourceTypeRepository dataSourceTypeRepository,
                IFailedScheduleRepository failedScheduleRepository,
                IScheduleTemplateRepository scheduleTemplateRepository,
                IScheduleExecutionRepository scheduleExecutionRepository,
                IScheduleContactRepository scheduleContactRepository,
                IScheduleJobRepository scheduleJobRepository,
                IEntityTagRepository<EntityTagDb> entityTagRepository
            )
             : base(context)
        {
            TemplateRepository = templateRepository;
            ScheduleRepository = scheduleRepository;
            SchemaRepository = schemaRepository;
            StorageRepository = storageRepository;
            ReportRepository = reportRepository;
            ReportTemplateRepository = reportTemplateRepository;
            ReportScheduleRepository = reportScheduleRepository;
            StorageTypeRepository = storageTypeRepository;
            OutputTypeRepository = outputTypeRepository;
            DataSourceTypeRepository = dataSourceTypeRepository;
            FailedScheduleRepository = failedScheduleRepository;
            ScheduleExecutionRepository = scheduleExecutionRepository;
            ScheduleTemplateRepository = scheduleTemplateRepository;
            ScheduleExecutionRepository = scheduleExecutionRepository;
            ScheduleContactRepository = scheduleContactRepository;
            ScheduleJobRepository = scheduleJobRepository;
            EntityTagRepository = entityTagRepository;
        }
    }
}