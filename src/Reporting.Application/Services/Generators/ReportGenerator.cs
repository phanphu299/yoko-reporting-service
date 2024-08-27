using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class ReportGenerator
    {
        private readonly ITemplateService _templateService;
        private readonly INativeStorageService _storageService;
        private readonly IAuditLogService _auditLogService;
        private readonly IWorkerService _workerService;
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly IDictionary<string, IDataSourceHandler> _dataSourceHandler;
        private readonly IDictionary<string, IStorageSpaceHandler> _storageSpaceHandler;
        private readonly ILoggerAdapter<ReportService> _logger;

        public ReportGenerator(
            ITemplateService templateService,
            INativeStorageService storageService,
            IAuditLogService auditLogService,
            IWorkerService workerService,
            IReportingUnitOfWork reportingUnitOfWork,
            IDictionary<string, IStorageSpaceHandler> storageSpaceHandler,
            IDictionary<string, IDataSourceHandler> dataSourceHandler,
            ILoggerAdapter<ReportService> logger)
        {
            _templateService = templateService;
            _storageService = storageService;
            _auditLogService = auditLogService;
            _workerService = workerService;
            _reportingUnitOfWork = reportingUnitOfWork;
            _storageSpaceHandler = storageSpaceHandler;
            _dataSourceHandler = dataSourceHandler;
            _logger = logger;
        }

        public async Task<GenerateReportDto> GenerateReportAsync(
            Guid executionId,
            int templateId,
            GenerateReport command,
            GenerateReport.GenerateTime generateTime,
            Domain.Entity.Schedule schedule)
        {
            var entity = await GenerateReportEntityAsync(executionId, templateId, command, generateTime);
            entity.ScheduleId = schedule?.Id;
            entity.ScheduleName = schedule?.Name;
            entity.ScheduleExecutionId = executionId;

            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                await _reportingUnitOfWork.ReportRepository.AddAsync(entity);
                await _reportingUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, command.ToJson());
                await _reportingUnitOfWork.RollbackAsync();
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.REPORT, ActionType.Add, ex, payload: command);
                throw;
            }

            await _auditLogService.SendLogAsync(ActivityLogEntityConstants.REPORT, ActionType.Add, ActionStatus.Success, entity.Id, entity.Name, command);
            var report = GenerateReportDto.Create(entity);
            report.JobId = command.JobId;
            return report;
        }

        private async Task<Domain.Entity.Report> GenerateReportEntityAsync(
            Guid executionId,
            int templateId,
            GenerateReport command,
            GenerateReport.GenerateTime generateTime)
        {
            var commandGetTemplate = new GetTemplateById(templateId);
            var template = await _templateService.GetTemplateByIdAsync(commandGetTemplate);

            IEnumerable<DataTableResult> data;
            using (var templateFile = new MemoryStream())
            {
                await _storageService.DownloadFileToStreamAsync(template.TemplateFileUrl, templateFile);
                templateFile.Seek(0, SeekOrigin.Begin);
                var commandGetData = new GetDataSource(templateFile, command.TimeZoneName, command.DateTimeFormat, generateTime.FromDateUtcInMilliseconds, generateTime.ToDateInUtcMilliseconds, template.DataSets);
                data = await GetDataSourceAsync(commandGetData);
            }

            var simpleTemplate = template.CreateSimpleTemplate();
            var commandBuildReport = new BuildReportFile(generateTime.FromDateLocal, generateTime.ToDateLocal, simpleTemplate, data);
            var reportFile = await _workerService.BuildReportFileAsync(commandBuildReport);

            reportFile.Position = 0;
            var uploadFileTimeUTC = DateTime.UtcNow;
            var commandUploadStorage = new UploadStorageSpace(simpleTemplate, reportFile, command.TimeZoneName, uploadFileTimeUTC, executionId);
            var uploadedInfo = await UploadStorageSpaceAsync(commandUploadStorage);

            var generatedFileInfo = new GenerateReport.GenerateFileInfo
            {
                Name = $"{template.Name}_{uploadFileTimeUTC.ToLocalDateTime(command.TimeZoneName).ToString(DateTimeExtension.LONG_TIMESTAMP_FORMAT)}",
                OutputTypeId = template.OutputTypeId,
                StorageUrl = uploadedInfo.FileUrl,
                FileName = uploadedInfo.FileName,
                StorageId = template.StorageId
            };

            var entity = GenerateReport.Create(generatedFileInfo, generateTime);
            entity.TemplateId = templateId;
            entity.TemplateName = template.Name;
            entity.CreatedBy = template.CreatedBy;
            entity.CreatedUtc = uploadFileTimeUTC;
            entity.UpdatedUtc = uploadFileTimeUTC;
            return entity;
        }

        private async Task<IEnumerable<DataTableResult>> GetDataSourceAsync(GetDataSource command)
        {
            var result = new List<DataTableResult>();
            foreach (var dataSet in command.DataSets)
            {
                command.CurrentDataSetName = dataSet.Name;
                command.CurrentDataSourceContent = dataSet.DataSourceContent;
                var deletedContents = await _dataSourceHandler[dataSet.DataSourceTypeId].CheckExistDataSourceAsync(new CheckExistDataSource(dataSet.DataSourceContent));
                if (deletedContents.Any())
                {
                    throw new SystemCallServiceException(detailCode: Message.DATASOURCE_IS_DELETED);
                }
                var dataTableResult = await _dataSourceHandler[dataSet.DataSourceTypeId].HandleAsync(command);
                result.AddRange(dataTableResult);
            }
            return result;
        }

        private async Task<UploadStorageSpaceDto> UploadStorageSpaceAsync(UploadStorageSpace command)
        {
            return await _storageSpaceHandler[command.StorageTypeId].HandleAsync(command);
        }
    }
}