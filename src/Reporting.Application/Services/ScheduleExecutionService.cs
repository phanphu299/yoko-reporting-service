using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reporting.Application.Command.Model;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using Reporting.Domain.Entity;
using Reporting.Domain.EnumType;
using System;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class ScheduleExecutionService : IScheduleExecutionService
    {
        private readonly IJobService _jobService;
        private readonly INotificationHandler _notificationHandler;
        private readonly IReportingUnitOfWork _unitOfWork;
        private readonly IReportProcessor _reportProcessor;
        private readonly ITenantContext _tenantContext;
        private readonly RetryOptions _retryOptions;
        private readonly ILoggerAdapter<ScheduleExecutionService> _logger;
        private readonly string _endpoint;

        public ScheduleExecutionService(
            IJobService jobService,
            INotificationHandler notificationHandler,
            IReportingUnitOfWork unitOfWork,
            IReportProcessor reportProcessor,
            ITenantContext tenantContext,
            RetryOptions retryOptions,
            ILoggerAdapter<ScheduleExecutionService> logger,
            IConfiguration configuration)
        {
            _jobService = jobService;
            _notificationHandler = notificationHandler;
            _unitOfWork = unitOfWork;
            _reportProcessor = reportProcessor;
            _tenantContext = tenantContext;
            _retryOptions = retryOptions;
            _logger = logger;
            _endpoint = configuration["Endpoint:ReportService"]?.TrimEnd('/');
        }

        public async Task TriggerExecutionAsync(Guid executionId, string retryJobId)
        {
            var scheduleExecution = await _unitOfWork.ScheduleExecutionRepository
                                                .AsQueryable()
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.Id == executionId);
            if (scheduleExecution == null)
            {
                _logger.LogInformation($"Execution {executionId} not found.");
                await CleanupRetryJob(retryJobId);
                return;
            }
            if (scheduleExecution.RetryCount >= scheduleExecution.MaxRetryCount)
            {
                _logger.LogInformation($"Execution {executionId} max retry attempts excedded.");
                await CleanupRetryJob(retryJobId);
                return;
            }
            if (scheduleExecution.State == ExecutionState.FIN)
            {
                _logger.LogInformation($"Execution {executionId} already in state {scheduleExecution.State}.");
                await CleanupRetryJob(retryJobId);
                return;
            }

            var executionParameter = scheduleExecution.ExecutionParam.FromJson<ReportAndSendParameter>();
            await GenerateReportAsync(scheduleExecution, retryJobId, executionParameter);
        }

        private async Task GenerateReportAsync(ScheduleExecution scheduleExecution, string retryJobId, ReportAndSendParameter executionParameter)
        {
            await UpdateExecutionStateAsync(scheduleExecution, executionParameter, retryJobId);

            bool isJobCompleted;
            try
            {
                await _reportProcessor.ProcessAsync(scheduleExecution, executionParameter);

                isJobCompleted = executionParameter.State == ExecutionState.FIN || scheduleExecution.RetryCount == scheduleExecution.MaxRetryCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate Report Failed.");
                isJobCompleted = false;
            }
            finally
            {
                scheduleExecution.State = executionParameter.State;
            }

            if (isJobCompleted)
            {
                try
                {
                    await PostProcessAsync(scheduleExecution, executionParameter);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Post Process Failed.");
                    scheduleExecution.State = ExecutionState.FAIL;
                }
            }

            await UpdateExecutionFinalState(scheduleExecution.Id, scheduleExecution.ScheduleId, executionParameter.ToJson(), scheduleExecution.State);
        }

        private async Task UpdateExecutionStateAsync(ScheduleExecution execution, ReportAndSendParameter executionParameter, string retryJobId)
        {
            bool isFirstRun = execution.State == ExecutionState.INIT;
            if (!isFirstRun)
            {
                execution.RetryCount++;
            }

            execution.RetryJobId = retryJobId;
            execution.State = ExecutionState.RUN;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (isFirstRun)
                {
                    var lastRunUtc = StringExtensions.UnixTimeStampToDateTime(executionParameter.ExecutionTimestamp.ToString());
                    await _unitOfWork.ScheduleRepository.UpdateLastRunAsync(execution.ScheduleId, lastRunUtc);
                }
                await _unitOfWork.ScheduleExecutionRepository.UpdateAsync(execution.Id, execution);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task UpdateExecutionFinalState(Guid scheduleExecutionId, int scheduleId, string executionParameter, ExecutionState state)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.ScheduleExecutionRepository.UpdateStateAsync(scheduleExecutionId, state, executionParameter);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task PostProcessAsync(ScheduleExecution scheduleExecution, ReportAndSendParameter executionParameter)
        {
            await _notificationHandler.SendGeneratedReportsAsync(scheduleExecution, executionParameter);

            if (!string.IsNullOrEmpty(scheduleExecution.RetryJobId))
                await CleanupRetryJob(scheduleExecution.RetryJobId);

            if (executionParameter.State == ExecutionState.FAIL)
                await AddFailedScheduleAsync(scheduleExecution, executionParameter);
        }

        private async Task CleanupRetryJob(string retryJobId)
        {
            try
            {
                await _jobService.DeleteJobAsync(retryJobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete retry job.");
                // Should not be important:
                // - retry job have end time
                // - if trigger again, retry will skip since execution is completed anyway
            }
        }

        private async Task AddFailedScheduleAsync(ScheduleExecution scheduleExecution, IExecutionParameter executionParameter)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var entity = executionParameter.CreateFailedSchedule();

                entity.ScheduleId = scheduleExecution.Schedule.Id;
                entity.ScheduleName = scheduleExecution.Schedule.Name;
                entity.CreatedUtc = DateTime.UtcNow;
                entity.UpdatedUtc = DateTime.UtcNow;

                await _unitOfWork.FailedScheduleRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await _unitOfWork.RollbackAsync();
            }
        }
    }
}