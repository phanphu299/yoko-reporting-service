using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Internal;
using AHI.Infrastructure.Service;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.UserContext.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using Reporting.Application.Models;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class ReportService : BaseSearchService<Domain.Entity.Report, int, SearchReport, ReportDto>, IReportService
    {
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly IFileEventService _fileEventService;
        private readonly ITemplateService _templateService;
        private readonly IAuditLogService _auditLogService;
        private readonly INativeStorageService _storageService;
        private readonly IWorkerService _workerService;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IDictionary<string, IDataSourceHandler> _dataSourceHandler;
        private readonly IDictionary<string, IStorageSpaceHandler> _storageSpaceHandler;
        private readonly ILoggerAdapter<ReportService> _logger;
        private readonly IExportNotificationService _exportNotificationService;
        private readonly RetryOptions _retryOptions;
        private readonly IJobService _jobService;
        private readonly string _endpoint;

        public ReportService(IServiceProvider serviceProvider,
                            ITenantContext tenantContext,
                            IUserContext userContext,
                            IReportingUnitOfWork reportingUnitOfWOrk,
                            ITemplateService templateService,
                            IFileEventService fileEventService,
                            IAuditLogService auditLogService,
                            INativeStorageService storageService,
                            IWorkerService workerService,
                            IBackgroundTaskQueue taskQueue,
                            IDictionary<string, IDataSourceHandler> dataSourceHandler,
                            IDictionary<string, IStorageSpaceHandler> storageSpaceHandler,
                            ILoggerAdapter<ReportService> logger,
                            IExportNotificationService exportNotificationService,
                            RetryOptions retryOptions,
                            IJobService jobService,
                            IConfiguration configuration)
             : base(ReportDto.Create, serviceProvider)
        {
            _tenantContext = tenantContext;
            _userContext = userContext;
            _reportingUnitOfWork = reportingUnitOfWOrk;
            _templateService = templateService;
            _fileEventService = fileEventService;
            _auditLogService = auditLogService;
            _workerService = workerService;
            _storageService = storageService;
            _dataSourceHandler = dataSourceHandler;
            _storageSpaceHandler = storageSpaceHandler;
            _taskQueue = taskQueue;
            _logger = logger;
            _exportNotificationService = exportNotificationService;
            _retryOptions = retryOptions;
            _jobService = jobService;
            _endpoint = configuration["Endpoint:ReportService"]?.TrimEnd('/');
        }

        public async Task<PreviewReportFileDto> PreviewReportAsync(PreviewReport command)
        {
            command.SetDefaultOutput();

            if (!OutputType.CheckOutputType(command.OutputTypeId))
            {
                throw new EntityNotFoundException();
            }

            var commandGetTemplate = new GetTemplateById(command.TemplateId);
            var template = await _templateService.GetTemplateByIdAsync(commandGetTemplate);
            var timeZoneName = _userContext?.Timezone?.Name ?? DateTimeExtension.DEFAULT_TIMEZONE_NAME;

            var templateFile = new MemoryStream();
            await _storageService.DownloadFileToStreamAsync(template.TemplateFileUrl, templateFile);

            templateFile.Seek(0, SeekOrigin.Begin);
            command.ExtractFromDateToDate(timeZoneName, _userContext.DateTimeFormat);

            var commandGetData = new GetDataSource(templateFile, _userContext.Timezone.Name, _userContext.DateTimeFormat, command.FromDateInMilliseconds, command.ToDateInMilliseconds, template.DataSets);
            var data = await GetDataSourceAsync(commandGetData);

            // override for preview purpose
            template.OutputTypeId = command.OutputTypeId;
            template.OutputType.Id = command.OutputTypeId;
            template.OutputType.Extension = OutputType.GetOutputExtension(command.OutputTypeId);

            var simpleTemplate = template.CreateSimpleTemplate();
            var commandPreviewReport = new PreviewReportFile(command.ActivityId, _userContext.Upn, timeZoneName, (TenantContext)_tenantContext, command.FromDate.ToLocalDateTime(_userContext.Timezone.Name), command.ToDate.ToLocalDateTime(_userContext.Timezone.Name), simpleTemplate, data);
            var previewReportFileDto = await _workerService.PreviewReportFileAsync(commandPreviewReport);

            return previewReportFileDto;
        }

        public async Task<string> CreatePreviewReportAsync(PreviewReportBase command, TemplateByIdDto template, string timeZoneName)
        {
            if (string.IsNullOrEmpty(command.OutputTypeId))
            {
                command.OutputTypeId = OutputType.PDF;
            }

            if (!OutputType.CheckOutputType(command.OutputTypeId))
            {
                throw new EntityNotFoundException();
            }

            var templateFile = new MemoryStream();
            await _storageService.DownloadFileToStreamAsync(template.TemplateFileUrl, templateFile);
            templateFile.Seek(0, SeekOrigin.Begin);
            command.ExtractFromDateToDate(timeZoneName, _userContext.DateTimeFormat);
            var commandGetData = new GetDataSource(templateFile, _userContext.Timezone.Name, _userContext.DateTimeFormat, command.FromDateInMilliseconds, command.ToDateInMilliseconds, template.DataSets);
            var data = await GetDataSourceAsync(commandGetData);

            // override for preview purpose
            template.OutputTypeId = command.OutputTypeId;
            template.OutputType.Id = command.OutputTypeId;
            template.OutputType.Extension = OutputType.GetOutputExtension(command.OutputTypeId);

            var simpleTemplate = template.CreateSimpleTemplate();
            var commandBuildReport = new BuildReportFile(command.FromDate.ToLocalDateTime(_userContext.Timezone.Name), command.ToDate.ToLocalDateTime(_userContext.Timezone.Name), simpleTemplate, data);
            var reportFile = await _workerService.BuildReportFileAsync(commandBuildReport);
            reportFile.Position = 0;

            var uploadFileTime = DateTime.UtcNow;
            var commandUploadStorage = new UploadStorageSpace(simpleTemplate, reportFile, timeZoneName, uploadFileTime);

            // override for preview purpose
            commandUploadStorage.StorageTypeId = StorageSpace.NATIVE_STORAGE_SPACE;

            var uploadedInfo = await UploadStorageSpaceAsync(commandUploadStorage);
            return uploadedInfo?.FileUrl;
        }

        public async Task InitGenerateReportAsync(GenerateReport command)
        {
            if (command.TemplateId != null)
                command.Templates = new List<int>(command.Templates) { command.TemplateId.Value };
            var schedule = await _reportingUnitOfWork.ScheduleRepository.AsQueryable().FirstOrDefaultAsync(x => x.JobId == command.JobId);

            Domain.Entity.ScheduleExecution scheduleExecution;
            switch (schedule.Type)
            {
                case ScheduleType.REPORT_AND_SEND:
                    {
                        var executionParam = ReportAndSendParameter.Create(command);
                        scheduleExecution = new Domain.Entity.ScheduleExecution
                        {
                            ScheduleId = schedule.Id,
                            MaxRetryCount = _retryOptions.ReportAndSend.RetryCount,
                            CreatedUtc = StringExtensions.UnixTimeStampToDateTime(command.ExecutionTime.ToString()),
                            ExecutionParam = executionParam.ToJson()
                        };
                        break;
                    }
                default:
                    throw new InvalidOperationException();
            }

            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                scheduleExecution = await _reportingUnitOfWork.ScheduleExecutionRepository.AddAsync(scheduleExecution);
                await _reportingUnitOfWork.CommitAsync();
            }
            catch
            {
                await _reportingUnitOfWork.RollbackAsync();
                throw;
            }
            var executionParameter = scheduleExecution.ExecutionParam.FromJson<ReportAndSendParameter>();
            var retryJobId = await CreateRetryJobAsync(scheduleExecution.Id, executionParameter.ExecutionTimestamp);

            await Task.WhenAll(
                _taskQueue.QueueAsync(new TriggerGenerateReport(scheduleExecution.Id, retryJobId), _tenantContext, _userContext)
            );
        }

        private async Task<string> CreateRetryJobAsync(Guid scheduleExecutionId, long executionTime)
        {
            var jobModel = BuildReportAndSendRetryJob(scheduleExecutionId, executionTime);
            jobModel.AdditionalParams = new Dictionary<string, object>()
            {
                [FieldName.TENANT_ID] = _tenantContext.TenantId,
                [FieldName.SUBSCRIPTION_ID] = _tenantContext.SubscriptionId,
                [FieldName.PROJECT_ID] = _tenantContext.ProjectId,
            };
            try
            {
                var response = await _jobService.AddJobAsync(jobModel);
                return response.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create job. Continue to generate report, but this execution won't be able to retry.");
                return null;
            }
        }

        private JobDto BuildReportAndSendRetryJob(Guid scheduleExecutionId, long executionTime)
        {
            var options = _retryOptions.ReportAndSend;
            var retryJobId = Guid.NewGuid().ToString();
            var retryPath = Endpoint.RETRY_GENERATE_REPORT.Replace("{{executionId}}", scheduleExecutionId.ToString());
            var endpoint = $"{_endpoint}/{retryPath}";
            var retryDateTime = StringExtensions.UnixTimeStampToDateTime(executionTime.ToString());
            var cron = $"* {retryDateTime.Minute % options.RetryDelay}/{options.RetryDelay} * * * ?";
            var start = retryDateTime.AddMinutes(options.RetryDelay);
            return new JobDto(retryJobId, cron, endpoint, HttpClientMethod.POST, "UTC", start, null, null);
        }


        // Mark as obsoleted, will be removed later after being fully refactored and confirmed to have no side effect
        [Obsolete("Should be replaced with ReportGenerator.GenerateReportAsync", error: false)]
        public async Task<IEnumerable<GenerateReportDto>> GenerateReportAsync(GenerateReport command)
        {
            if (command.TemplateId != null)
            {
                command.Templates = new List<int>(command.Templates) { command.TemplateId.Value };
            }

            var entities = new List<Domain.Entity.Report>();
            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                var generateTime = command.ExtractFromDateToDate(command.TimeZoneName, command.DateTimeFormat);
                var schedule = await _reportingUnitOfWork.ScheduleRepository.AsQueryable().FirstOrDefaultAsync(x => x.JobId == command.JobId);

                var distinctTemplates = command.Templates.Distinct();
                foreach (var templateId in distinctTemplates)
                {
                    var entity = await GenerateReportEntityAsync(templateId, command, generateTime);
                    if (schedule != null)
                    {
                        schedule.LastRunUtc = generateTime.ExecutionTimeUtc;
                        entity.ScheduleId = schedule.Id;
                        entity.ScheduleName = schedule.Name;
                    }
                    entities.Add(entity);
                    await _reportingUnitOfWork.ReportRepository.AddAsync(entity);
                }
                await _reportingUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _reportingUnitOfWork.RollbackAsync();
                await AddFailedScheduleAsync(command);
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.REPORT, ActionType.Add, ex, payload: command);
                throw;
            }

            var result = new List<GenerateReportDto>();
            foreach (var entity in entities)
            {
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.REPORT, ActionType.Add, ActionStatus.Success, entity.Id, entity.Name, command);
                var report = GenerateReportDto.Create(entity);
                if (report != null)
                    report.JobId = command.JobId;
                result.Add(report);
            }

            return result;
        }

        private async Task<Domain.Entity.Report> GenerateReportEntityAsync(
            int templateId,
            GenerateReport command,
            GenerateReport.GenerateTime generateTime)
        {
            var commandGetTemplate = new GetTemplateById(templateId);
            var template = await _templateService.GetTemplateByIdAsync(commandGetTemplate);

            var templateFile = new MemoryStream();
            await _storageService.DownloadFileToStreamAsync(template.TemplateFileUrl, templateFile);
            templateFile.Seek(0, SeekOrigin.Begin);
            var commandGetData = new GetDataSource(templateFile, command.TimeZoneName, command.DateTimeFormat, generateTime.FromDateUtcInMilliseconds, generateTime.ToDateInUtcMilliseconds, template.DataSets);
            var data = await GetDataSourceAsync(commandGetData);

            var simpleTemplate = template.CreateSimpleTemplate();
            var commandBuildReport = new BuildReportFile(generateTime.FromDateLocal, generateTime.ToDateLocal, simpleTemplate, data);
            var reportFile = await _workerService.BuildReportFileAsync(commandBuildReport);

            reportFile.Position = 0;
            var uploadFileTimeUTC = DateTime.UtcNow;
            var commandUploadStorage = new UploadStorageSpace(simpleTemplate, reportFile, command.TimeZoneName, uploadFileTimeUTC);
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
            return entity;
        }

        public async Task<IEnumerable<DataTableResult>> GetDataSourceAsync(GetDataSource command)
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

        public Task<List<string>> GetReportIdsByTemplateIdsAsync(IEnumerable<string> templateIds)
        {
            var ids = templateIds.Select(x => Convert.ToInt32(x));
            return _reportingUnitOfWork.ReportRepository.AsQueryable()
                                                        .AsNoTracking()
                                                        .Where(x => ids.Any(id => id == x.TemplateId.Value))
                                                        .Select(x => x.Id.ToString())
                                                        .ToListAsync();
        }

        public Task<List<string>> GetReportIdsByScheduleIdsAsync(IEnumerable<string> scheduleIds)
        {
            var ids = scheduleIds.Select(x => Convert.ToInt32(x));
            return _reportingUnitOfWork.ReportRepository.AsQueryable()
                                                        .AsNoTracking()
                                                        .Where(x => x.ScheduleId.HasValue && ids.Any(id => id == x.ScheduleId.Value))
                                                        .Select(x => x.Id.ToString())
                                                        .ToListAsync();
        }

        public async Task<UploadStorageSpaceDto> UploadStorageSpaceAsync(UploadStorageSpace command)
        {
            return await _storageSpaceHandler[command.StorageTypeId].HandleAsync(command);
        }

        public async Task<DownloadFileUrlDto> GetDownloadFileUrlAsync(GetDownloadFileUrl command)
        {
            var report = await _reportingUnitOfWork.ReportRepository.FindAsync(command.Id);
            if (report == null)
            {
                throw new EntityNotFoundException();
            }

            command.StorageUrl = report.StorageUrl;
            command.StorageSpaceContent = report.Template.Storage.Content;

            var downloadFileUrl = await _storageSpaceHandler[report.Template.Storage.TypeId].GetDownloadFileUrlAsync(command);

            return new DownloadFileUrlDto(downloadFileUrl);
        }

        public async Task<ReportDetailDto> GetReportByIdAsync(GetReport command)
        {
            var report = await _reportingUnitOfWork.ReportRepository.FindAsync(command.Id);
            if (report == null)
            {
                throw new EntityNotFoundException();
            }
            return ReportDetailDto.Create(report);
        }

        public async Task<ActivityResponse> DownloadReportsAsync(ExportReport command)
        {
            try
            {
                var ids = command.Ids.Select(x => Convert.ToInt32(x));
                var exist = await _reportingUnitOfWork.ReportRepository.AsQueryable().AnyAsync(x => ids.Any(id => id == x.Id));
                if (!exist)
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.Ids), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_SOME_ITEMS_DELETED);
                }
                await _fileEventService.SendExportEventAsync(command.ActivityId, command.ObjectType, command.Ids);
                return new ActivityResponse(command.ActivityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.REPORT, ActionType.Export, ex, payload: command);
                throw;
            }
        }

        public async Task<ActivityResponse> DownLoadReportsByTemplateAsync(ExportReportByTemplate command)
        {
            try
            {
                var ids = command.TemplateIds.Select(x => Convert.ToInt32(x));
                var exist = await _reportingUnitOfWork.ReportTemplateRepository.AsQueryable().AnyAsync(x => ids.Any(id => id == x.TemplateId));
                if (!exist)
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.TemplateIds), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_SOME_ITEMS_DELETED);
                }

                await _fileEventService.SendExportEventAsync(command.ActivityId, IOEntityType.TEMPLATE, command.TemplateIds);
                return new ActivityResponse(command.ActivityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.REPORT, ActionType.Export, ex, payload: command);
                throw;
            }
        }

        public async Task<ActivityResponse> DownLoadReportsByScheduleAsync(ExportReportBySchedule command)
        {
            try
            {
                if (command.ScheduleNames == null || !command.ScheduleNames.Any())
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.ScheduleNames), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

                if (string.IsNullOrWhiteSpace(command.TemplateId))
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.TemplateId), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

                if (command.ScheduleIds == null || !command.ScheduleIds.Any())
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.ScheduleIds), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

                var scheduleIds = command.ScheduleIds.Select(x => Convert.ToInt32(x));
                var templateId = Convert.ToInt32(command.TemplateId);
                var exist = await _reportingUnitOfWork.ReportScheduleRepository.AsQueryable().AnyAsync(x => command.ScheduleNames.Any(name => name == x.Name) && scheduleIds.Any(id => id == x.Id) && x.TemplateId == templateId);
                if (!exist)
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.ScheduleNames), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_SOME_ITEMS_DELETED);
                }

                await _fileEventService.SendExportReportScheduleEventAsync(command.ActivityId, IOEntityType.SCHEDULE, command.ScheduleNames, command.TemplateId, command.ScheduleIds);
                return new ActivityResponse(command.ActivityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.REPORT, ActionType.Export, ex, payload: command);
                throw;
            }
        }

        public async Task<ActivityResponse> DownloadPreviewReportsAsync(ExportPreviewReport command)
        {
            try
            {
                var template = await _templateService.GetTemplateByIdAsync(new GetTemplateById(command.TemplateId));
                if (template.DataSets.Any(dataSet => dataSet.Deleted))
                {
                    throw new SystemCallServiceException(detailCode: Message.DATASOURCE_IS_DELETED);
                }
                _exportNotificationService.Upn = _userContext.Upn;
                _exportNotificationService.ActivityId = command.ActivityId;
                _exportNotificationService.ObjectType = ObjectType.REPORT_OBJECT_TYPE;
                _exportNotificationService.NotificationType = ActionType.Export;

                await _exportNotificationService.SendStartExportNotifyAsync(1);

                if (string.IsNullOrEmpty(command.OutputTypeId))
                {
                    command.OutputTypeId = OutputType.PDF;
                }

                if (!OutputType.CheckOutputType(command.OutputTypeId))
                {
                    throw new EntityNotFoundException();
                }

                await _taskQueue.QueueAsync(command, _tenantContext, _userContext/*, this*/);
                return new ActivityResponse(command.ActivityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.REPORT, ActionType.Export, ex, payload: command);
                throw;
            }
        }

        private async Task AddFailedScheduleAsync(GenerateReport generateReportCommand)
        {
            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                var entity = AddFailedSchedule.Create(generateReportCommand);
                var schedule = await _reportingUnitOfWork.ScheduleRepository.AsQueryable().AsNoTracking().FirstOrDefaultAsync(x => x.JobId == generateReportCommand.JobId);
                if (schedule == null)
                {
                    throw new EntityNotFoundException();
                }

                entity.ScheduleId = schedule.Id;
                entity.ScheduleName = schedule.Name;
                entity.CreatedUtc = DateTime.UtcNow;
                entity.UpdatedUtc = DateTime.UtcNow;

                await _reportingUnitOfWork.FailedScheduleRepository.AddAsync(entity);

                await _reportingUnitOfWork.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await _reportingUnitOfWork.RollbackAsync();
            }
        }

        protected override Type GetDbType()
        {
            return typeof(IReportRepository);
        }
    }
}