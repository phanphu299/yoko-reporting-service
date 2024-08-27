using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.UserContext.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Services.Abstractions;
using Reporting.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Command.Handler
{
    public class TriggerCollectReportsRequestHandler : IRequestHandler<TriggerCollectReports>
    {
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly IJobService _jobService;
        private readonly ICommandQueue<CollectReports> _commandQueue;
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;
        private readonly IConfiguration _configuration;
        private readonly ILoggerAdapter<TriggerCollectReportsRequestHandler> _logger;
        private readonly string _endpoint;

        public TriggerCollectReportsRequestHandler(IReportingUnitOfWork reportingUnitOfWork,
            IJobService jobService,
            ICommandQueue<CollectReports> commandQueue,
            ITenantContext tenantContext,
            IUserContext userContext,
            IConfiguration configuration,
            ILoggerAdapter<TriggerCollectReportsRequestHandler> logger)
        {
            _reportingUnitOfWork = reportingUnitOfWork;
            _jobService = jobService;
            _commandQueue = commandQueue;
            _tenantContext = tenantContext;
            _userContext = userContext;
            _configuration = configuration;
            _logger = logger;
            _endpoint = configuration["Endpoint:ReportService"]?.TrimEnd('/');
        }

        public async Task<Unit> Handle(TriggerCollectReports request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Report collection triggered for job {JobId}.", request.JobId);

            var schedule = await _reportingUnitOfWork.ScheduleRepository.AsQueryable()
                .FirstOrDefaultAsync(x => x.JobId == request.JobId && x.Type == Constant.ScheduleType.SEND);

            if (schedule == null)
            {
                _logger.LogInformation("Could not find Send schedule with job Id {JobId}.", request.JobId);
                await _jobService.DeleteJobAsync(request.JobId);
                return Unit.Value;
            }

            var reportGenerationScheduleIds = await _reportingUnitOfWork.ScheduleJobRepository
                .AsQueryable()
                .AsNoTracking()
                .Where(x => x.ScheduleId == schedule.Id)
                .Select(x => x.JobId)
                .ToListAsync();

            var parameters = new CollectReportsParameters
            {
                ReportGenerationScheduleIds = reportGenerationScheduleIds,
                TimeZoneName = request.TimeZoneName,
                ExecutionTimeUtc = request.ExecutionTimeUtc,
                Period = schedule.Period
            };

            var execution = await CreateScheduleExecutionAsync(schedule.Id, parameters);

            await _commandQueue.EnqueueAsync(new CollectReports
            {
                RetryJobId = execution.RetryJobId,
                ExecutionId = execution.Id
            }, _tenantContext, _userContext);

            return Unit.Value;
        }

        /// <summary>
        /// Creates a new schedule execution and retry job.
        /// </summary>
        private async Task<ScheduleExecution> CreateScheduleExecutionAsync(int scheduleId, CollectReportsParameters parameters)
        {
            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                var maxRetryAttempts = int.TryParse(_configuration["CollectReport:MaxRetryTime"], out var value) ? value : 3;
                var execution = await _reportingUnitOfWork.ScheduleExecutionRepository.AddAsync(new Domain.Entity.ScheduleExecution
                {
                    ScheduleId = scheduleId,
                    State = Domain.EnumType.ExecutionState.INIT,
                    MaxRetryCount = maxRetryAttempts,
                    RetryCount = 0,
                    RetryJobId = Guid.NewGuid().ToString(),
                    CreatedUtc = parameters.ExecutionTimeUtc,
                    ExecutionParam = parameters.ToJson()
                });
                await _reportingUnitOfWork.ScheduleRepository.UpdateLastRunAsync(execution.ScheduleId, parameters.ExecutionTimeUtc);
                await _reportingUnitOfWork.CommitAsync();
                await CreateRetryJobAsync(execution);
                return execution;
            }
            catch
            {
                await _reportingUnitOfWork.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Creates a new recurring job to trigger retry attempts at fixed intervals.
        /// This should be done as early as possible (before any long running operation),
        /// in order to ensure the retry attempts are triggered at the correct time.
        /// </summary>
        private async Task<JobDto> CreateRetryJobAsync(ScheduleExecution execution)
        {
            var maxRetryAttempts = int.TryParse(_configuration["CollectReport:MaxRetryTime"], out var retryTime) ? retryTime : 3;
            var retryInterval = int.TryParse(_configuration["CollectReport:RetryInterval"], out var interval) ? interval : 5;
            var endpoint = $"{_endpoint}/{Endpoint.RETRY_COLLECT_REPORT}";
            var cron = $"* {execution.CreatedUtc.Minute % retryInterval}/{retryInterval} * * * ?";
            var retryStartTime = execution.CreatedUtc.AddMinutes(retryInterval);
            var retryJob = new JobDto(execution.RetryJobId, cron, endpoint, HttpClientMethod.POST, "UTC", retryStartTime, null,
                new Dictionary<string, object>
                {
                    { FieldName.TENANT_ID, _tenantContext.TenantId },
                    { FieldName.SUBSCRIPTION_ID, _tenantContext.SubscriptionId },
                    { FieldName.PROJECT_ID, _tenantContext.ProjectId },
                    { "scheduleExecutionId", execution.Id }
                });
            return await _jobService.AddJobAsync(retryJob);
        }
    }
}