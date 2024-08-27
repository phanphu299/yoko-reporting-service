using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.Service;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.SharedKernel.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class StorageService : BaseSearchService<Domain.Entity.Storage, int, SearchStorage, StorageDto>, IStorageService
    {
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly ISchemaValidator _schemaValidator;
        private readonly IAuditLogService _auditLogService;
        private readonly ILoggerAdapter<StorageService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantContext _tenantContext;

        public StorageService(IServiceProvider serviceProvider,
                                IReportingUnitOfWork reportingUnitOfWork,
                                IAuditLogService auditLogService,
                                ISchemaValidator schemaValidator,
                                ILoggerAdapter<StorageService> logger,
                                IHttpClientFactory httpClientFactory,
                                ITenantContext tenantContext)
            : base(StorageDto.Create, serviceProvider)
        {
            _reportingUnitOfWork = reportingUnitOfWork;
            _schemaValidator = schemaValidator;
            _auditLogService = auditLogService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tenantContext = tenantContext;
        }

        public async Task<StorageDto> GetStorageByIdAsync(GetStorage command)
        {
            var storage = await _reportingUnitOfWork.StorageRepository.FindAsync(command.Id);
            if (storage == null)
            {
                throw new EntityNotFoundException();
            }
            return StorageDto.Create(storage);
        }

        public async Task<StorageDto> AddStorageAsync(AddStorage command)
        {
            try
            {
                await ValidateStorageTypeAsync(command.TypeId);
                await ValidateStorageNameAsync(command.Name);
                await _schemaValidator.ValidateAsync(command.TypeId, command.Content);

                var entity = AddStorage.Create(command);

                await _reportingUnitOfWork.StorageRepository.AddAsync(entity);
                await _reportingUnitOfWork.CommitAsync();
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.STORAGE, ActionType.Add, ActionStatus.Success, entity.Id, entity.Name, command);

                return StorageDto.Create(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.STORAGE, ActionType.Add, ex, payload: command);
                throw;
            }
        }

        public async Task<StorageDto> UpdateStorageAsync(UpdateStorage command)
        {
            string entityName = null;
            try
            {
                await ValidateStorageAsync(command.Id);
                entityName = await _reportingUnitOfWork.StorageRepository.AsFetchable().Where(x => x.Id == command.Id).Select(x => x.Name).FirstOrDefaultAsync();
                await ValidateStorageTypeAsync(command.TypeId);
                await ValidateStorageNameAsync(command.Name, command.Id);

                await _schemaValidator.ValidateAsync(command.TypeId, command.Content);

                var entity = UpdateStorage.Create(command);

                await _reportingUnitOfWork.StorageRepository.UpdateAsync(command.Id, entity);
                await _reportingUnitOfWork.CommitAsync();
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.STORAGE, ActionType.Update, ActionStatus.Success, entity.Id, entity.Name, command);

                return StorageDto.Create(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.STORAGE, ActionType.Update, ex, command.Id, entityName, command);
                throw;
            }
        }

        public async Task<BaseResponse> DeleteStorageAsync(DeleteStorage command)
        {
            var listEntityName = new List<string>();
            try
            {
                listEntityName.AddRange(await _reportingUnitOfWork.StorageRepository.AsFetchable().Where(x => command.Ids.Contains(x.Id)).Select(x => x.Name).ToListAsync());
                var isBeingUsed = await _reportingUnitOfWork.TemplateRepository.AsQueryable().AsNoTracking().AnyAsync(x => command.Ids.Contains(x.StorageId));
                if (isBeingUsed)
                {
                    throw new EntityInvalidException(detailCode: Message.STORAGE_BEING_USED);
                }

                await _reportingUnitOfWork.StorageRepository.RemoveStoragesAsync(command.Ids);
                await _reportingUnitOfWork.CommitAsync();
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.STORAGE, ActionType.Delete, ActionStatus.Success, command.Ids, listEntityName, command);

                return BaseResponse.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.STORAGE, ActionType.Delete, ex, command.Ids, listEntityName, command);
                throw;
            }
        }

        public async Task ValidateStorageTypeAsync(string storageTypeId)
        {
            var exist = await _reportingUnitOfWork.StorageTypeRepository.AsQueryable().AnyAsync(x => x.Id == storageTypeId);
            if (!exist)
            {
                throw EntityValidationExceptionHelper.GenerateException("StorageType", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_NOT_FOUND);
            }
        }

        public async Task ValidateStorageAsync(int storageId)
        {
            var exist = await _reportingUnitOfWork.StorageRepository.AsQueryable().AnyAsync(x => x.Id == storageId);
            if (!exist)
            {
                throw new EntityNotFoundException();
            }
        }

        public async Task ValidateStorageNameAsync(string name, int? storageId = null)
        {
            var exist = storageId != null
                ? await _reportingUnitOfWork.StorageRepository.AsQueryable().AnyAsync(x => x.Id != storageId && x.Name == name)
                : await _reportingUnitOfWork.StorageRepository.AsQueryable().AnyAsync(x => x.Name == name);
            if (exist)
            {
                throw EntityValidationExceptionHelper.GenerateException("Name", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_DUPLICATED);
            }
        }

        protected override Type GetDbType()
        {
            return typeof(IStorageRepository);
        }

        public async Task<string> ExtractZipEntryAsync(string filePath, string contentPath, string folderPath)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientName.STORAGE_SERVICE, _tenantContext);
            var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    filePath = filePath,
                    targetPath = $"report_template/{folderPath}",
                    contentPath = contentPath,
                    cleanAfterExtract = false
                }), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("sta/zip/entry/extract", content);
            var responseContent = await response.Content.ReadAsByteArrayAsync();
            response.EnsureSuccessStatusCode();
            var path = responseContent.Deserialize<JObject>()["filePath"].ToString();
            return path;
        }
    }
}