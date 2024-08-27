using AHI.Infrastructure.Repository.Generic;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using Reporting.Domain.Entity;

namespace Reporting.Application.Repository
{
    public interface IReportingUnitOfWork : IUnitOfWork
    {
        public ISchemaRepository SchemaRepository { get; }
        public IStorageRepository StorageRepository { get; }
        public ITemplateRepository TemplateRepository { get; }
        public IReportRepository ReportRepository { get; }
        public IReportTemplateRepository ReportTemplateRepository { get; }
        public IReportScheduleRepository ReportScheduleRepository { get; }
        public IStorageTypeRepository StorageTypeRepository { get; }
        public IOutputTypeRepository OutputTypeRepository { get; }
        public IDataSourceTypeRepository DataSourceTypeRepository { get; }
        public IScheduleRepository ScheduleRepository { get; }
        public IFailedScheduleRepository FailedScheduleRepository { get; }
        public IScheduleExecutionRepository ScheduleExecutionRepository { get; }
        public IScheduleTemplateRepository ScheduleTemplateRepository { get; }
        public IScheduleContactRepository ScheduleContactRepository { get; }
        public IScheduleJobRepository ScheduleJobRepository { get; }
        public IEntityTagRepository<EntityTagDb> EntityTagRepository { get; }
    }
}