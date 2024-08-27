using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.Service;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.UserContext.Abstraction;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Template.Command;
using Reporting.Application.Template.Command.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Security.Extension;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using Device.Application.Constant;
using Reporting.Domain.Entity;

namespace Reporting.Application.Service
{
    public class TemplateService : BaseSearchService<Domain.Entity.Template, int, SearchTemplate, TemplateDto>, ITemplateService
    {
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly ISchemaValidator _schemaValidator;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContext _userContext;
        private readonly IDictionary<string, IDataSourceHandler> _dataSourceHandler;
        private readonly ILoggerAdapter<TemplateService> _logger;
        private readonly IValidator<TemplateDto> _templateVerificationValidator;
        private readonly IStorageService _storageService;
        private readonly ITenantContext _tenantContext;
        private readonly ITagService _tagService;

        public TemplateService(IServiceProvider serviceProvider,
                                IReportingUnitOfWork reportingUnitOfWork,
                                ISchemaValidator schemaValidator,
                                IAuditLogService auditLogService,
                                IUserContext userContext,
                                IDictionary<string, IDataSourceHandler> dataSourceHandler,
                                ILoggerAdapter<TemplateService> logger,
                                IValidator<TemplateDto> templateVerificationValidator,
                                IStorageService storageService,
                                ITenantContext tenantContext,
                                ITagService tagService)
            : base(TemplateDto.Create, serviceProvider)
        {
            _reportingUnitOfWork = reportingUnitOfWork;
            _schemaValidator = schemaValidator;
            _auditLogService = auditLogService;
            _dataSourceHandler = dataSourceHandler;
            _userContext = userContext;
            _logger = logger;
            _templateVerificationValidator = templateVerificationValidator;
            _storageService = storageService;
            _tenantContext = tenantContext;
            _tagService = tagService;
        }

        public async Task<TemplateByIdDto> GetTemplateByIdAsync(GetTemplateById command)
        {
            var template = await _reportingUnitOfWork.TemplateRepository.FindAsync(command.Id);
            if (template == null)
            {
                throw new EntityNotFoundException();
            }

            var result = TemplateByIdDto.Create(template);
            result.DataSets = await MarkDataSourceIsDeletedAsync(result.DataSets);

            return await _tagService.FetchTagsAsync(result);
        }

        public async Task<TemplateDto> AddTemplateAsync(AddTemplate command)
        {
            try
            {
                await ValidateOutputTypeAsync(command.OutputTypeId);
                await ValidateStorageAsync(command.StorageId);
                await ValidateTemplateNameAsync(command.Name);
                await _schemaValidator.ValidateAsync(command.DataSets);
                await ValidateDataSourceAsync(command.DataSets);

                command.SetDefaultValue();
                var entity = AddTemplate.Create(command);

                entity.CreatedBy = _userContext.Upn;
                var entityResult = await _reportingUnitOfWork.TemplateRepository.AddAsync(entity);
                await _reportingUnitOfWork.CommitAsync();

                var tagIds = Array.Empty<long>();
                if (command.Tags != null && command.Tags.Any())
                {
                    command.Upn = _userContext.Upn;
                    command.ApplicationId = Guid.Parse(_userContext.ApplicationId ?? ApplicationInformation.ASSET_DASHBOARD_APPLICATION_ID);
                    tagIds = await _tagService.UpsertTagsAsync(command);
                }

                if (tagIds.Any())
                {
                    var entitiesTags = tagIds.Distinct().Select(x => new EntityTagDb
                    {
                        EntityType = Privileges.ReportTemplate.ENTITY_NAME,
                        EntityIdInt = entityResult.Id,
                        TagId = x
                    }).ToArray();

                    await _reportingUnitOfWork.EntityTagRepository.AddRangeWithSaveChangeAsync(entitiesTags);
                    await _reportingUnitOfWork.CommitAsync();
                }

                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.TEMPLATE, ActionType.Add, ActionStatus.Success, entity.Id, entity.Name, command);

                return await _tagService.FetchTagsAsync(TemplateDto.Create(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.TEMPLATE, ActionType.Add, ex, payload: command);
                throw;
            }
        }

        public async Task<TemplateDto> PartialUpdateTemplateAsync(PartialUpdateTemplate command)
        {
            string entityName = null;
            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                var targetEntity = await _reportingUnitOfWork.TemplateRepository.FindAsync(command.Id);
                if (targetEntity == null)
                {
                    throw new EntityNotFoundException();
                }
                entityName = targetEntity.Name;
                var commandUpdate = new UpdateTemplate();
                var document = command.JsonPatchDocument;
                if (document == null || !document.Operations.Any())
                    throw EntityValidationExceptionHelper.GenerateException("Payload", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

                document.ApplyTo(commandUpdate);
                commandUpdate.Id = command.Id;
                if (!string.IsNullOrEmpty(commandUpdate.Name))
                {
                    await UpdateTemplateNameInReportsAsync(command.Id, commandUpdate.Name);
                    commandUpdate.SetDefaultValue();
                }
                else
                    commandUpdate.Name = entityName;

                await ValidateOutputTypeAsync(commandUpdate.OutputTypeId);
                await ValidateStorageAsync(commandUpdate.StorageId);
                await ValidateTemplateNameAsync(commandUpdate.Name, commandUpdate.Id);
                await _schemaValidator.ValidateAsync(commandUpdate.DataSets);
                await ValidateDataSourceAsync(commandUpdate.DataSets);

                var requestedEntity = UpdateTemplate.Create(commandUpdate);
                await _reportingUnitOfWork.TemplateRepository.PartialUpdateTemplateAsync(UpdateTemplate.Create(commandUpdate), targetEntity);

                if (commandUpdate.Tags != null)
                {
                    var tagIds = Array.Empty<long>();
                    if (commandUpdate.Tags.Any())
                    {
                        commandUpdate.Upn = _userContext.Upn;
                        commandUpdate.ApplicationId = Guid.Parse(_userContext.ApplicationId ?? ApplicationInformation.ASSET_DASHBOARD_APPLICATION_ID);
                        tagIds = await _tagService.UpsertTagsAsync(commandUpdate);
                    }

                    var entityTags = await _reportingUnitOfWork.EntityTagRepository.AsQueryable()
                                                                .Where(x => x.EntityIdInt == commandUpdate.Id && x.EntityType == Privileges.ReportTemplate.ENTITY_NAME)
                                                                .ToArrayAsync();
                    if (entityTags.Any())
                    {
                        _reportingUnitOfWork.EntityTagRepository.RemoveRange(entityTags);
                    }

                    if (tagIds.Any())
                    {
                        var entitiesTags = tagIds.Distinct().Select(x => new EntityTagDb
                        {
                            EntityType = Privileges.ReportTemplate.ENTITY_NAME,
                            EntityIdInt = commandUpdate.Id,
                            TagId = x
                        }).ToArray();
                        await _reportingUnitOfWork.EntityTagRepository.AddRangeWithSaveChangeAsync(entitiesTags);
                    }
                }

                await _reportingUnitOfWork.CommitAsync();
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.TEMPLATE, ActionType.Update, ActionStatus.Success, requestedEntity.Id, requestedEntity.Name, command);

                return TemplateDto.Create(targetEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _reportingUnitOfWork.RollbackAsync();
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.TEMPLATE, ActionType.Update, ex, command.Id, entityName, payload: command);
                throw;
            }
        }

        private async Task UpdateTemplateNameInReportsAsync(int templateId, string templateName)
        {
            if (templateId == 0 || string.IsNullOrWhiteSpace(templateName))
            {
                return;
            }

            var reportsOfTemplate = _reportingUnitOfWork.ReportRepository.AsQueryable().Where(x => x.TemplateId == templateId);
            await _reportingUnitOfWork.ReportRepository.UpdateTemplateNameOfReportListAsync(reportsOfTemplate, templateName);
        }

        public async Task<BaseResponse> DeleteTemplateAsync(DeleteTemplate command)
        {
            var entityNames = await _reportingUnitOfWork.TemplateRepository.AsQueryable().AsNoTracking().Where(x => command.Ids.Contains(x.Id)).Select(x => x.Name).ToListAsync();
            try
            {
                if (entityNames.Count != command.Ids.Count())
                {
                    throw new EntityValidationException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED);
                }

                var isBeingUsed = await _reportingUnitOfWork.ScheduleTemplateRepository.IsBeingUsedAsync(command.Ids);
                if (isBeingUsed)
                {
                    throw new EntityInvalidException(detailCode: Message.TEMPLATE_BEING_USED);
                }

                await _reportingUnitOfWork.TemplateRepository.RemoveTemplatesAsync(command.Ids);
                await _reportingUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.TEMPLATE, ActionType.Delete, ex, command.Ids, entityNames, command);
                throw;
            }
            await _auditLogService.SendLogAsync(ActivityLogEntityConstants.TEMPLATE, ActionType.Delete, ActionStatus.Success, command.Ids, entityNames, command);
            return BaseResponse.Success;
        }

        public async Task ValidateOutputTypeAsync(string outputTypeId)
        {
            if (string.IsNullOrEmpty(outputTypeId))
                return;

            var exist = await _reportingUnitOfWork.OutputTypeRepository.AsQueryable().AnyAsync(x => x.Id == outputTypeId);
            if (!exist)
            {
                throw EntityValidationExceptionHelper.GenerateException("OutputType", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_NOT_FOUND);
            }
        }

        public async Task ValidateStorageAsync(int storageId)
        {
            if (storageId == 0)
                return;

            var exist = await _reportingUnitOfWork.StorageRepository.AsQueryable().AnyAsync(x => x.Id == storageId);
            if (!exist)
            {
                throw EntityValidationExceptionHelper.GenerateException("Storage", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_NOT_FOUND);
            }
        }

        public async Task ValidateTemplateNameAsync(string name, int? storageId = null)
        {
            var exist = storageId != null
                ? await _reportingUnitOfWork.TemplateRepository.AsQueryable().AnyAsync(x => x.Id != storageId && x.Name == name)
                : await _reportingUnitOfWork.TemplateRepository.AsQueryable().AnyAsync(x => x.Name == name);
            if (exist)
            {
                throw EntityValidationExceptionHelper.GenerateException("Name", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_DUPLICATED);
            }
        }

        private async Task ValidateDataSourceAsync(IEnumerable<BaseTemplateDetail> dataSets)
        {
            if (!dataSets.Any())
                return;

            var failures = new List<ValidationFailure>();
            foreach (var dataSet in dataSets)
            {
                var command = new CheckExistDataSource(JsonConvert.SerializeObject(dataSet.DataSourceContent));
                var deletedContents = await _dataSourceHandler[dataSet.DataSourceTypeId].CheckExistDataSourceAsync(command);
                if (deletedContents.Any())
                {
                    failures.Add(new ValidationFailure(dataSet.Name, ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED));
                }
            }

            if (failures.Any())
            {
                throw new EntityValidationException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED, failures: failures);
            }
        }

        private async Task<IEnumerable<DataSourceContentDto>> MarkDataSourceIsDeletedAsync(IEnumerable<DataSourceContentDto> dataSets)
        {
            if (!dataSets.Any())
                return dataSets;

            var result = new List<DataSourceContentDto>();
            foreach (var item in dataSets)
            {
                var deletedContents = await _dataSourceHandler[item.DataSourceTypeId].CheckExistDataSourceAsync(new CheckExistDataSource(item.DataSourceContent));

                if (deletedContents.Any())
                {
                    item.Deleted = true;
                    item.DeletedContent = JsonConvert.SerializeObject(deletedContents);
                }

                result.Add(item);
            }

            return result;
        }

        protected override Type GetDbType()
        {
            return typeof(ITemplateRepository);
        }

        public async Task<IEnumerable<TemplateBasicDto>> ArchiveAsync(ArchiveTemplate command, CancellationToken cancellationToken)
        {
            var result = await _reportingUnitOfWork.TemplateRepository.AsQueryable()
                                                                      .AsNoTracking()
                                                                      .Include(x => x.Details)
                                                                      .Where(x => x.UpdatedUtc <= command.ArchiveTime && !x.Deleted)
                                                                      .ToListAsync();
            return result.Select(x => TemplateDto.CreateArchiveDto(x));
        }

        public async Task<bool> VerifyArchiveAsync(VerifyArchivedTemplate command, CancellationToken cancellationToken)
        {
            var data = JsonConvert.DeserializeObject<IEnumerable<TemplateDto>>(command.Data, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting);
            foreach (var template in data)
            {
                var validation = await _templateVerificationValidator.ValidateAsync(template);
                if (!validation.IsValid)
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.Data), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
                }
            }
            return true;
        }

        public async Task<bool> RetrieveAsync(RetrieveTemplate command, CancellationToken cancellationToken)
        {
            var data = JsonConvert.DeserializeObject<IEnumerable<TemplateDto>>(command.Data, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting).OrderBy(x => x.UpdatedUtc);
            var templates = TemplateDto.Create(data, command.Upn, _tenantContext.SubscriptionId, _tenantContext.ProjectId).ToList();
            var templateDetails = data.SelectMany(x => x.DataSets.Select(x => TemplateDetailDto.Create(x))).ToList();
            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                await _reportingUnitOfWork.TemplateRepository.RetrieveTemplateAsync(templates);
                await _reportingUnitOfWork.TemplateRepository.RetrieveTemplateDetailAsync(templateDetails);

                await _reportingUnitOfWork.CommitAsync();
            }
            catch
            {
                await _reportingUnitOfWork.RollbackAsync();
                throw;
            }

            await UploadFilesAsync(templates, command.AdditionalData);
            return true;
        }

        private async Task UploadFilesAsync(IList<Domain.Entity.Template> templates, IDictionary<string, object> additionalData)
        {
            foreach (var template in templates)
            {
                var key = template.Id.ToString();
                if (!additionalData.ContainsKey(key))
                    throw new EntityNotFoundException();

                var jsonData = additionalData[key].ToString();
                var fileData = JsonConvert.DeserializeObject<AdditionalData>(jsonData);
                if (fileData == null)
                    continue;

                if (string.IsNullOrEmpty(fileData.FilePath) || string.IsNullOrEmpty(fileData.ContentPath))
                    continue;
                await ProcessUploadAsync(template, fileData);
            }
        }

        private async Task ProcessUploadAsync(Domain.Entity.Template template, AdditionalData additionalData)
        {
            if (string.IsNullOrEmpty(template.TemplateFileUrl))
                return;
            var fileInfo = GetFileInfo(template);
            template.TemplateFileUrl = await _storageService.ExtractZipEntryAsync(additionalData.FilePath.Base64Decode(), additionalData.ContentPath.Base64Decode(), fileInfo.FolderPath);
        }

        private FileInfo GetFileInfo(Domain.Entity.Template template)
        {
            var file = new FileInfo();
            var subPath = template.TemplateFileUrl.Substring(template.TemplateFileUrl.IndexOf("report_template"));
            var arrayPath = subPath.Split('/');
            file.FolderPath = arrayPath[1] ?? string.Empty;
            file.FileName = arrayPath[2] ?? string.Empty;
            return file;
        }
    }

    internal class FileInfo
    {
        public string FileName { get; set; }
        public string FolderPath { get; set; }
    }

    internal class AdditionalData
    {
        public string FilePath { get; set; }
        public string ContentPath { get; set; }
    }
}