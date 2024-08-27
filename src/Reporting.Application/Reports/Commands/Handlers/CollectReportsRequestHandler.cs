using AHI.Infrastructure.Exception;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using Reporting.Domain.Entity;
using Reporting.Domain.EnumType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Command.Handler
{
    public class CollectReportsRequestHandler : IRequestHandler<CollectReports>
    {
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly IJobService _jobService;
        private readonly INotificationHandler _notificationHandler;
        private readonly ILoggerAdapter<CollectReportsRequestHandler> _logger;

        public CollectReportsRequestHandler(
            IReportingUnitOfWork reportingUnitOfWork,
            IJobService jobService,
            INotificationHandler notificationHandler,
            ILoggerAdapter<CollectReportsRequestHandler> logger
            )
        {
            _reportingUnitOfWork = reportingUnitOfWork;
            _jobService = jobService;
            _notificationHandler = notificationHandler;
            _logger = logger;
        }

        public async Task<Unit> Handle(CollectReports request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Report collection attempt started for execution {ExecutionId}.", request.ExecutionId);

            var execution = await _reportingUnitOfWork.ScheduleExecutionRepository.FindAsync(request.ExecutionId);
            if (execution == null)
            {
                _logger.LogInformation("Could not find schedule execution with Id {ExecutionId}.", request.ExecutionId);
                await _jobService.DeleteJobAsync(request.RetryJobId);
                return Unit.Value;
            }

            var parameters = execution.ExecutionParam.FromJson<CollectReportsParameters>();

            switch (execution.State)
            {
                case ExecutionState.INIT:
                    execution.State = ExecutionState.RUN;
                    await UpdateExecutionAsync(execution);
                    await CollectAndSendReportsAsync(execution, parameters);
                    break;

                case ExecutionState.RUN:
                    if (execution.RetryCount < execution.MaxRetryCount)
                    {
                        execution.RetryCount++;
                        await UpdateExecutionAsync(execution);
                        await CollectAndSendReportsAsync(execution, parameters);
                    }
                    else
                    {
                        _logger.LogInformation("Report collection max retry attempts exceeded. Schedule {ScheduleId}, Execution {ExecutionId}, Last status {LastStatus}.",
                            execution.ScheduleId, execution.Id, execution.State);
                        execution.State = ExecutionState.FAIL;
                        await _notificationHandler.SendCollectedReportsAsync(execution, parameters, new List<Report>(), new List<string>(), new List<string>());
                        await UpdateExecutionAsync(execution);
                        await _jobService.DeleteJobAsync(execution.RetryJobId);
                    }
                    break;

                default:
                    _logger.LogInformation("Report collection completed in previous attempts: Schedule {ScheduleId}, Execution {ExecutionId}, Last status {LastStatus}.",
                        execution.ScheduleId, execution.Id, execution.State);
                    await _jobService.DeleteJobAsync(execution.RetryJobId);
                    break;
            }

            return Unit.Value;
        }

        private async Task UpdateExecutionAsync(ScheduleExecution execution)
        {
            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                await _reportingUnitOfWork.ScheduleExecutionRepository.UpdateAsync(execution.Id, execution);
                await _reportingUnitOfWork.CommitAsync();
            }
            catch
            {
                await _reportingUnitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task CollectAndSendReportsAsync(ScheduleExecution execution, CollectReportsParameters parameters)
        {
            var collectionFromUtc = CalculateCollectionFromUtc(parameters);
            bool isLastAttempt = execution.RetryCount >= execution.MaxRetryCount;

            var executions = await _reportingUnitOfWork.ScheduleExecutionRepository
                .AsQueryable()
                .AsNoTracking()
                .Where(x => parameters.ReportGenerationScheduleIds.Contains(x.ScheduleId) && x.CreatedUtc >= collectionFromUtc && x.CreatedUtc <= execution.CreatedUtc)
                .OrderBy(x => x.CreatedUtc)
                .ToListAsync();

            if (!isLastAttempt)
            {
                var unfinishedExecution = executions.FirstOrDefault(x => x.State != ExecutionState.FIN);
                if (unfinishedExecution != null)
                {
                    _logger.LogInformation("Report collection attempt aborted for Schedule {ScheduleId}, Execution {ExecutionId}. Schedule {ReportAndSendScheduleId} has not completed yet: Execution {ReportAndSendExcutionId}, Status {State}.",
                        execution.ScheduleId, execution.Id,
                        unfinishedExecution.ScheduleId, unfinishedExecution.Id, unfinishedExecution.State);
                    return;
                }

                var notStartedScheduleId = parameters.ReportGenerationScheduleIds
                    .Where(id => !executions.Any(x => x.ScheduleId == id))
                    .Select(id => (int?)id)
                    .FirstOrDefault();
                if (notStartedScheduleId.HasValue)
                {
                    _logger.LogInformation("Report collection attempt aborted for Schedule {ScheduleId}, Execution {ExecutionId}. Schedule {ReportAndSendScheduleId} has not started yet.",
                        execution.ScheduleId, execution.Id, notStartedScheduleId);
                    return;
                }
            }

            var executionIds = executions
                .Select(x => x.Id)
                .ToList();

            var generatedReports = await _reportingUnitOfWork.ReportRepository
                .AsQueryable()
                .AsNoTracking()
                .Where(x => x.ScheduleExecutionId.HasValue && executionIds.Contains(x.ScheduleExecutionId.Value))
                .ToListAsync();

            var failedScheduleIds = parameters.ReportGenerationScheduleIds
                .Where(id => !generatedReports.Any(x => x.ScheduleId == id))
                .ToList();

            var partialSuccessScheduleIds = parameters.ReportGenerationScheduleIds
                .Where(id => generatedReports.Any(x => x.ScheduleId == id)
                    && executions.Any(x => x.ScheduleId == id && x.State != ExecutionState.FIN))
                .ToList();

            execution.State = (failedScheduleIds.Any() || partialSuccessScheduleIds.Any())
                ? (generatedReports.Any() ? ExecutionState.PFIN : ExecutionState.FAIL)
                : ExecutionState.FIN;

            var failedScheduleNames = execution.State == ExecutionState.PFIN
                ? await _reportingUnitOfWork.ScheduleRepository
                    .AsQueryable()
                    .AsNoTracking()
                    .Where(x => failedScheduleIds.Contains(x.Id))
                    .Select(x => x.Name)
                    .ToListAsync()
                : new List<string>();

            var partialSuccessScheduleNames = execution.State == ExecutionState.PFIN
                ? await _reportingUnitOfWork.ScheduleRepository
                    .AsQueryable()
                    .AsNoTracking()
                    .Where(x => partialSuccessScheduleIds.Contains(x.Id))
                    .Select(x => x.Name)
                    .ToListAsync()
                : new List<string>();

            await _notificationHandler.SendCollectedReportsAsync(execution, parameters, generatedReports, failedScheduleNames, partialSuccessScheduleNames);
            await UpdateExecutionAsync(execution);
            await _jobService.DeleteJobAsync(execution.RetryJobId);
        }

        private DateTime CalculateCollectionFromUtc(CollectReportsParameters collectReportParameters)
        {
            string unit = collectReportParameters.Period.Substring(collectReportParameters.Period.Length - 1);
            int value = int.Parse(collectReportParameters.Period.Substring(1, collectReportParameters.Period.Length - 2));
            switch (unit)
            {
                case "y":
                    return collectReportParameters.ExecutionTimeUtc.AddYears(value);
                case "M":
                    return collectReportParameters.ExecutionTimeUtc.AddMonths(value);
                case "d":
                    return collectReportParameters.ExecutionTimeUtc.AddDays(value);
                case "h":
                    return collectReportParameters.ExecutionTimeUtc.AddHours(value);
                case "m":
                    return collectReportParameters.ExecutionTimeUtc.AddMinutes(value);
                default:
                    return collectReportParameters.ExecutionTimeUtc;
            }
        }
    }
}