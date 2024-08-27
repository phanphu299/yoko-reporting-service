using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.UserContext.Abstraction;
using MediatR;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Command.Handler
{
    public class RetryGenerateReportRequestHandler : IRequestHandler<TriggerGenerateReport, BaseResponse>
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;

        public RetryGenerateReportRequestHandler(IBackgroundTaskQueue taskQueue, ITenantContext tenantContext, IUserContext userContext)
        {
            _taskQueue = taskQueue;
            _tenantContext = tenantContext;
            _userContext = userContext;
        }

        public async Task<BaseResponse> Handle(TriggerGenerateReport request, CancellationToken cancellationToken)
        {
            await _taskQueue.QueueAsync(request, _tenantContext, _userContext);
            return BaseResponse.Success;
        }
    }
}