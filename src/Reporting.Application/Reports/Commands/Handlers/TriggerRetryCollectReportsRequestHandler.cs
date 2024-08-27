using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using MediatR;
using Reporting.Application.Repository;
using Reporting.Application.Services.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Command.Handler
{
    public class TriggerRetryCollectReportRequestHandler : IRequestHandler<TriggerRetryCollectReports>
    {
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly ICommandQueue<CollectReports> _commandQueue;
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;
        private readonly ILoggerAdapter<TriggerRetryCollectReportRequestHandler> _logger;

        public TriggerRetryCollectReportRequestHandler(
            IReportingUnitOfWork reportingUnitOfWork,
            ICommandQueue<CollectReports> commandQueue,
            ITenantContext tenantContext,
            IUserContext userContext,
            ILoggerAdapter<TriggerRetryCollectReportRequestHandler> logger)
        {
            _reportingUnitOfWork = reportingUnitOfWork;
            _commandQueue = commandQueue;
            _tenantContext = tenantContext;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(TriggerRetryCollectReports request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Report collection retry attempt triggered for execution {ExecutionId}.", request.ScheduleExecutionId);
            await _commandQueue.EnqueueAsync(new CollectReports
            {
                RetryJobId = request.JobId,
                ExecutionId = request.ScheduleExecutionId
            }, _tenantContext, _userContext);

            return Unit.Value;
        }
    }
}