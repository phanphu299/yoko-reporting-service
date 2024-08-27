using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.Service;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.UserContext.Abstraction;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using Reporting.Application.Repository;
using Reporting.Application.Schedule.Command;
using Reporting.Application.Service.Abstraction;
using Scheduler.Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using Device.Application.Constant;
using Reporting.Domain.Entity;

namespace Reporting.Application.Service
{
    public class ScheduleService : BaseSearchService<Domain.Entity.Schedule, int, SearchSchedule, ScheduleDto>, IScheduleService
    {
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly IJobService _jobService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserService _userService;
        private readonly IValidator<ScheduleDto> _scheduleVerificationValidator;
        private readonly IDictionary<string, IScheduleValidationHandler> _scheduleValidationHandler;
        private readonly ILoggerAdapter<ScheduleService> _logger;
        private readonly string _endpoint;
        private readonly ITagService _tagService;

        public ScheduleService(IServiceProvider serviceProvider,
                                ITenantContext tenantContext,
                                IUserContext userContext,
                                IReportingUnitOfWork reportingUnitOfWork,
                                IJobService jobService,
                                IAuditLogService auditLogService,
                                IUserService userService,
                                IValidator<ScheduleDto> scheduleVerificationValidator,
                                IDictionary<string, IScheduleValidationHandler> scheduleValidationHandler,
                                ILoggerAdapter<ScheduleService> logger,
                                IConfiguration configuration,
                                ITagService tagService)
            : base(ScheduleDto.Create, serviceProvider)
        {
            _tenantContext = tenantContext;
            _userContext = userContext;
            _reportingUnitOfWork = reportingUnitOfWork;
            _jobService = jobService;
            _auditLogService = auditLogService;
            _userService = userService;
            _scheduleVerificationValidator = scheduleVerificationValidator;
            _scheduleValidationHandler = scheduleValidationHandler;
            _logger = logger;
            _scheduleVerificationValidator = scheduleVerificationValidator;
            _endpoint = configuration["Endpoint:ReportService"]?.TrimEnd('/');
            _tagService = tagService;
        }

        public async Task<ScheduleDto> GetScheduleByIdAsync(GetScheduleById command)
        {
            var schedule = await _reportingUnitOfWork.ScheduleRepository.FindAsync(command.Id);
            if (schedule == null)
            {
                throw new EntityNotFoundException();
            }

            return await _tagService.FetchTagsAsync(ScheduleDto.Create(schedule));
        }

        public async Task<ScheduleDto> AddScheduleAsync(AddSchedule command)
        {
            Domain.Entity.Schedule requestEntity = null;
            IEnumerable<string> warnings;
            await _reportingUnitOfWork.BeginTransactionAsync();

            try
            {
                ValidateCronExpression(command.Cron);
                ValidateTimeZoneName(command.TimeZoneName);

                await ValidateSchedulerNameAsync(command.Name);
                warnings = await ValidateContactsAsync(command.Contacts);

                var validator = _scheduleValidationHandler[command.Type];
                await validator.HandleAsync(command);

                requestEntity = AddSchedule.Create(command, _endpoint);
                var additionalParams = BuildAdditionalParams(_tenantContext, requestEntity.Period, requestEntity.ScheduleTemplates.Select(x => x.TemplateId), requestEntity.ScheduleJobs.Select(x => x.JobId));
                var jobModel = new JobDto(requestEntity.JobId, requestEntity.Cron, requestEntity.Endpoint, requestEntity.Method, requestEntity.TimeZoneName, requestEntity.Start, requestEntity.End, additionalParams);

                // *NOTE: No need to wait for the job's response, because the schedule need to be created first, in case the job need to be started now, it could be able to call back to the new created schedule
                _ = _jobService.AddJobAsync(jobModel);

                requestEntity.AdditionalParams = additionalParams.ToJson();
                requestEntity.CreatedBy = _userContext.Upn;

                await _reportingUnitOfWork.ScheduleRepository.AddAsync(requestEntity);
                await _reportingUnitOfWork.CommitAsync();

                await _reportingUnitOfWork.BeginTransactionAsync();
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
                        EntityType = EntityTypeConstants.REPORT_SCHEDULE,
                        EntityIdInt = requestEntity.Id,
                        TagId = x
                    }).ToArray();

                    await _reportingUnitOfWork.EntityTagRepository.AddRangeWithSaveChangeAsync(entitiesTags);
                    await _reportingUnitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await _reportingUnitOfWork.RollbackAsync();
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.SCHEDULE, ActionType.Add, ex, payload: command);
                throw;
            }

            await _auditLogService.SendLogAsync(ActivityLogEntityConstants.SCHEDULE, ActionType.Add, ActionStatus.Success, requestEntity.Id, requestEntity.Name, command);

            Domain.Entity.Schedule schedule = await _reportingUnitOfWork.ScheduleRepository.FindAsync(requestEntity.Id);
            if (schedule == null)
            {
                throw new EntityNotFoundException();
            }

            return await _tagService.FetchTagsAsync(ScheduleDto.Create(requestEntity, warnings));
        }

        public async Task<ScheduleDto> UpdateScheduleAsync(UpdateSchedule command)
        {
            string entityName = null;
            Domain.Entity.Schedule schedule = null;
            IEnumerable<string> warnings;
            await _reportingUnitOfWork.BeginTransactionAsync();

            try
            {
                var entity = await _reportingUnitOfWork.ScheduleRepository.FindAsync(command.Id);
                if (entity == null)
                {
                    throw new EntityNotFoundException();
                }

                if (entity.Type != command.Type)
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.Type), Message.SCHEDULE_TYPE_CANNOT_CHANGED);
                }
                entityName = entity.Name;

                ValidateCronExpression(command.Cron);
                ValidateTimeZoneName(command.TimeZoneName);

                if (entity.Name != command.Name)
                {
                    await ValidateSchedulerNameAsync(command.Name);
                }
                warnings = await ValidateContactsAsync(command.Contacts);

                var validator = _scheduleValidationHandler[command.Type];
                await validator.HandleAsync(command);

                schedule = UpdateSchedule.Create(command, _endpoint);
                if (!string.IsNullOrEmpty(entity.JobId))
                {
                    var additionalParams = BuildAdditionalParams(_tenantContext, schedule.Period, schedule.ScheduleTemplates.Select(x => x.TemplateId), schedule.ScheduleJobs.Select(x => x.JobId));
                    var jobModel = new JobDto(entity.JobId, schedule.Cron, schedule.Endpoint, schedule.Method, schedule.TimeZoneName, schedule.Start, schedule.End, additionalParams);
                    var jobResponse = await _jobService.UpdateJobAsync(jobModel);

                    schedule.JobId = jobResponse.Id;
                    schedule.AdditionalParams = additionalParams.ToJson();
                }
                else
                {
                    schedule.JobId = entity.JobId;
                }

                await _reportingUnitOfWork.ScheduleRepository.UpdateAsync(schedule.Id, schedule);

                var tagIds = Array.Empty<long>();
                if (command.Tags != null && command.Tags.Any())
                {
                    command.Upn = _userContext.Upn;
                    command.ApplicationId = Guid.Parse(_userContext.ApplicationId ?? ApplicationInformation.ASSET_DASHBOARD_APPLICATION_ID);
                    tagIds = await _tagService.UpsertTagsAsync(command);
                }

                var entityTags = await _reportingUnitOfWork.EntityTagRepository.AsQueryable()
                                                            .Where(x => x.EntityIdInt == command.Id && x.EntityType == EntityTypeConstants.REPORT_SCHEDULE)
                                                            .ToArrayAsync();
                if (entityTags.Any())
                {
                    _reportingUnitOfWork.EntityTagRepository.RemoveRange(entityTags);
                }

                if (tagIds.Any())
                {
                    var entitiesTags = tagIds.Distinct().Select(x => new EntityTagDb
                    {
                        EntityType = EntityTypeConstants.REPORT_SCHEDULE,
                        EntityIdInt = command.Id,
                        TagId = x
                    }).ToArray();
                    await _reportingUnitOfWork.EntityTagRepository.AddRangeWithSaveChangeAsync(entitiesTags);
                }

                await _reportingUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _reportingUnitOfWork.RollbackAsync();
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.SCHEDULE, ActionType.Update, ex, command.Id, entityName, command);
                throw;
            }
            await _auditLogService.SendLogAsync(ActivityLogEntityConstants.SCHEDULE, ActionType.Update, ActionStatus.Success, schedule.Id, schedule.Name, command);

            schedule = await _reportingUnitOfWork.ScheduleRepository.FindAsync(command.Id);
            if (schedule == null)
            {
                throw new EntityNotFoundException();
            }

            return await _tagService.FetchTagsAsync(ScheduleDto.Create(schedule, warnings));
        }

        public async Task<ScheduleDto> PartialUpdateScheduleAsync(PartialUpdateSchedule command)
        {
            string entityName = null;
            Domain.Entity.Schedule schedule = null;
            IEnumerable<string> warnings = Enumerable.Empty<string>();
            await _reportingUnitOfWork.BeginTransactionAsync();

            try
            {
                var entity = await _reportingUnitOfWork.ScheduleRepository.FindAsync(command.Id);
                if (entity == null)
                {
                    throw new EntityNotFoundException();
                }
                entityName = entity.Name;
                var commandUpdate = new UpdateSchedule();
                var document = command.JsonPatchDocument;
                if (document == null || !document.Operations.Any())
                    throw EntityValidationExceptionHelper.GenerateException("Payload", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

                document.ApplyTo(commandUpdate);
                commandUpdate.Id = command.Id;
                if (commandUpdate.Contacts != null && commandUpdate.Contacts.Any())
                {
                    warnings = await ValidateContactsAsync(commandUpdate.Contacts);
                }

                if (!string.IsNullOrWhiteSpace(commandUpdate.Type) && entity.Type != commandUpdate.Type)
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(commandUpdate.Type), Message.SCHEDULE_TYPE_CANNOT_CHANGED);
                }

                if (!string.IsNullOrWhiteSpace(commandUpdate.Cron))
                {
                    ValidateCronExpression(commandUpdate.Cron);
                }

                if (!string.IsNullOrEmpty(commandUpdate.TimeZoneName))
                {
                    ValidateTimeZoneName(commandUpdate.TimeZoneName);
                }

                if (!string.IsNullOrEmpty(commandUpdate.Name))
                {
                    await ValidateSchedulerNameAsync(commandUpdate.Name);
                }

                commandUpdate.AssignDefaultValueFromDb(entity, command.JsonPatchDocument.Operations);

                var validator = _scheduleValidationHandler[entity.Type];
                await validator.HandleAsync(commandUpdate);

                var pathsNotRequiringSchedulerUpdate = new HashSet<string> { "/name", "/contacts" };
                var schedulerUpdateRequired = command.JsonPatchDocument.Operations.Any(x => !pathsNotRequiringSchedulerUpdate.Contains(x.path));
                if (!string.IsNullOrEmpty(entity.JobId) && schedulerUpdateRequired)
                {
                    var additionalParams = BuildAdditionalParams(_tenantContext, commandUpdate.Period, commandUpdate.Templates, commandUpdate.Jobs);
                    var jobModel = new JobDto(entity.JobId, commandUpdate.Cron, entity.Endpoint, entity.Method, commandUpdate.TimeZoneName, commandUpdate.Start, commandUpdate.End, additionalParams);
                    await _jobService.UpdateJobAsync(jobModel);
                }

                schedule = UpdateSchedule.Create(commandUpdate, _endpoint);

                await _reportingUnitOfWork.ScheduleRepository.UpdateAsync(schedule.Id, schedule);

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
                                                                .Where(x => x.EntityIdInt == commandUpdate.Id && x.EntityType == EntityTypeConstants.REPORT_SCHEDULE)
                                                                .ToArrayAsync();
                    if (entityTags.Any())
                    {
                        _reportingUnitOfWork.EntityTagRepository.RemoveRange(entityTags);
                    }

                    if (tagIds.Any())
                    {
                        var entitiesTags = tagIds.Distinct().Select(x => new EntityTagDb
                        {
                            EntityType = EntityTypeConstants.REPORT_SCHEDULE,
                            EntityIdInt = commandUpdate.Id,
                            TagId = x
                        }).ToArray();
                        await _reportingUnitOfWork.EntityTagRepository.AddRangeWithSaveChangeAsync(entitiesTags);
                    }
                }

                await _reportingUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _reportingUnitOfWork.RollbackAsync();
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.SCHEDULE, ActionType.Update, ex, command.Id, entityName, command);
                throw;
            }

            await _auditLogService.SendLogAsync(ActivityLogEntityConstants.SCHEDULE, ActionType.Update, ActionStatus.Success, schedule.Id, schedule.Name, command);

            schedule = await _reportingUnitOfWork.ScheduleRepository.FindAsync(command.Id);
            if (schedule == null)
            {
                throw new EntityNotFoundException();
            }

            return await _tagService.FetchTagsAsync(ScheduleDto.Create(schedule, warnings));
        }

        public async Task<BaseResponse> DeleteScheduleAsync(DeleteSchedule command)
        {
            var entityNames = await _reportingUnitOfWork.ScheduleRepository.AsQueryable().AsNoTracking().Where(x => command.Ids.Contains(x.Id)).Select(x => x.Name).ToListAsync();

            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                if (entityNames.Count != command.Ids.Count())
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.Ids), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_SOME_ITEMS_DELETED);
                }

                var scheduleJobsQuery = _reportingUnitOfWork.ScheduleJobRepository.AsQueryable();
                var hasLinkedSendSchedules = await _reportingUnitOfWork.ScheduleRepository.AsQueryable()
                    .Where(x => command.Ids.Contains(x.Id)
                        && x.Type == Reporting.Application.Constant.ScheduleType.REPORT_AND_SEND
                        && scheduleJobsQuery.Any(y => y.JobId == x.Id))
                    .AnyAsync();

                if (hasLinkedSendSchedules)
                {
                    throw new EntityInvalidException(Message.SCHEDULE_HAS_LINKED_SEND_SCHEDULES, Message.SCHEDULE_HAS_LINKED_SEND_SCHEDULES);
                }

                foreach (var id in command.Ids)
                {
                    var entity = await _reportingUnitOfWork.ScheduleRepository.FindAsync(id);
                    entity.Deleted = true;
                    entity.UpdatedUtc = DateTime.UtcNow;

                    if (!string.IsNullOrEmpty(entity.JobId))
                    {
                        await _jobService.DeleteJobAsync(entity.JobId);
                    }

                    await _reportingUnitOfWork.FailedScheduleRepository.DeleteByScheduleIdAsync(entity.Id);
                    await _reportingUnitOfWork.ScheduleTemplateRepository.DeleteByScheduleIdAsync(entity.Id);
                    await _reportingUnitOfWork.ScheduleJobRepository.DeleteByScheduleIdAsync(entity.Id);
                }

                await _reportingUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _reportingUnitOfWork.RollbackAsync();
                _logger.LogError(ex, JsonConvert.SerializeObject(command));
                await _auditLogService.SendLogAsync(ActivityLogEntityConstants.SCHEDULE, ActionType.Delete, ex, command.Ids, entityNames, command);
                throw;
            }

            await _auditLogService.SendLogAsync(ActivityLogEntityConstants.SCHEDULE, ActionType.Delete, ActionStatus.Success, command.Ids, entityNames, command);
            return BaseResponse.Success;
        }

        private void ValidateCronExpression(string cron)
        {
            var valid = CronJobHelper.IsValidCronExpression(cron);
            if (!valid)
            {
                throw EntityValidationExceptionHelper.GenerateException("Cron", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
            }
        }

        private void ValidateTimeZoneName(string timeZoneName)
        {
            var timeZoneInfo = timeZoneName.GetTimeZoneInfo();
            if (timeZoneInfo == null)
            {
                throw EntityValidationExceptionHelper.GenerateException("TimeZoneName", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
            }
        }

        public async Task ValidateSchedulerNameAsync(string name)
        {
            var exist = await _reportingUnitOfWork.ScheduleRepository.AsQueryable().AsNoTracking().AnyAsync(x => x.Name == name);
            if (exist)
            {
                throw EntityValidationExceptionHelper.GenerateException(nameof(name), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_DUPLICATED);
            }
        }

        private async Task<IEnumerable<string>> ValidateContactsAsync<T>(ICollection<T> contacts) where T : BaseScheduleContact
        {
            var failures = new List<string>();
            var invalidContacts = new List<T>();
            foreach (var contact in contacts.Where(x => x.ObjectType == Domain.EnumType.ContactType.Contact))
            {
                var dto = await _userService.FetchContactAsync(contact.ObjectId);
                if (dto is null)
                {
                    invalidContacts.Add(contact);
                }
            }
            foreach (var contactGroup in contacts.Where(x => x.ObjectType == Domain.EnumType.ContactType.ContactGroup))
            {
                var dto = await _userService.FetchContactGroupAsync(contactGroup.ObjectId);
                if (dto is null)
                {
                    invalidContacts.Add(contactGroup);
                }
            }
            if (invalidContacts.Any())
            {
                failures.Add(Message.WARNING_VALIDATION_SOME_CONTACTS_OR_CONTACT_GROUPS_DELETED);
            }
            foreach (var invalidContact in invalidContacts)
            {
                contacts.Remove(invalidContact);
            }
            return failures;
        }

        private IDictionary<string, object> BuildAdditionalParams(
            ITenantContext tenantContext,
            string period,
            IEnumerable<int> templateIds,
            IEnumerable<int> jobIds)
        {
            return new Dictionary<string, object>()
            {
                [FieldName.TENANT_ID] = tenantContext.TenantId,
                [FieldName.SUBSCRIPTION_ID] = tenantContext.SubscriptionId,
                [FieldName.PROJECT_ID] = tenantContext.ProjectId,
                [FieldName.TEMPLATES] = templateIds,
                [FieldName.JOBS] = jobIds,
                [FieldName.PERIOD] = period,
                [FieldName.DATETIME_FORMAT] = _userContext.DateTimeFormat
            };
        }

        protected override Type GetDbType()
        {
            return typeof(IScheduleRepository);
        }

        public async Task<IEnumerable<ScheduleDto>> ArchiveAsync(ArchiveSchedule command, CancellationToken cancellationToken)
        {
            var result = await _reportingUnitOfWork.ScheduleRepository.AsQueryable().Include(x => x.Contacts).AsNoTracking()
                                                                      .Where(x => x.UpdatedUtc <= command.ArchiveTime)
                                                                      .ToListAsync();
            return result.Select(x => ScheduleDto.CreateArchive(x));
        }

        public async Task<bool> VerifyArchiveAsync(VerifySchedule command, CancellationToken cancellationToken)
        {
            var data = command.Data.FromJson<IEnumerable<ScheduleDto>>();
            foreach (var schedule in data)
            {
                var validation = await _scheduleVerificationValidator.ValidateAsync(schedule);
                if (!validation.IsValid)
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.Data), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
                }
            }
            return true;
        }

        public async Task<bool> RetrieveAsync(RetrieveSchedule command, CancellationToken cancellationToken)
        {
            var data = command.Data.FromJson<IEnumerable<ScheduleDto>>();
            await PreProcessRetrieveScheduleContactListAsync(data);
            var schedules = data.OrderBy(x => x.UpdatedUtc).Select(ScheduleDto.Create).ToList();
            await _reportingUnitOfWork.BeginTransactionAsync();
            _userContext.SetUpn(command.Upn);
            try
            {
                schedules.ForEach(x => x.CreatedBy = command.Upn);
                schedules.ForEach(x => x.JobId = Guid.NewGuid().ToString());

                await _reportingUnitOfWork.ScheduleRepository.RetrieveAsync(schedules);
                await _reportingUnitOfWork.CommitAsync();
            }
            catch
            {
                await _reportingUnitOfWork.RollbackAsync();
                throw;
            }

            foreach (var schedule in schedules)
            {
                await RegisterScheduleJobAsync(schedule);
            }

            return true;
        }

        public async Task PreProcessRetrieveScheduleContactListAsync(IEnumerable<ScheduleDto> schedules)
        {
            var availableContacts = schedules.Where(x => x.Contacts != null && x.Contacts.Any()).SelectMany(x => x.Contacts);
            var distinctContacts = availableContacts.GroupBy(x => new { x.ObjectType, x.ObjectId }).Select(x => x.First()).ToList();
            var tasks = distinctContacts.Select(x =>
            {
                return CheckContactAsync(x);
            });
            var validContacts = await Task.WhenAll(tasks);
            validContacts = validContacts.Where(x => x != null).ToArray();

            foreach (var schedule in schedules.Where(x => x.Contacts != null && x.Contacts.Any()))
            {
                schedule.Contacts = schedule.Contacts.Where(x => validContacts.Any(a => a.ObjectId == x.ObjectId && a.ObjectType == x.ObjectType)).ToList();
            }
        }

        private async Task<ScheduleContactDto> CheckContactAsync(ScheduleContactDto contact)
        {
            if (contact.ObjectType == Domain.EnumType.ContactType.ContactGroup)
            {
                var dto = await _userService.FetchContactGroupAsync(contact.ObjectId);
                if (dto is null)
                    return null;
            }
            else
            {
                var dto = await _userService.FetchContactAsync(contact.ObjectId);
                if (dto is null)
                    return null;
            }
            return contact;
        }

        public async Task RegisterScheduleJobAsync(Domain.Entity.Schedule schedule)
        {
            var additionalParams = BuildAdditionalParams(_tenantContext, schedule.Period, schedule.ScheduleTemplates.Select(x => x.TemplateId), schedule.ScheduleJobs.Select(x => x.JobId));
            var jobModel = new JobDto(schedule.JobId, schedule.Cron, schedule.Endpoint, schedule.Method, schedule.TimeZoneName, schedule.Start, schedule.End, additionalParams);
            _ = await _jobService.AddJobAsync(jobModel);
        }
    }
}