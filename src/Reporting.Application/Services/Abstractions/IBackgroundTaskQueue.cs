using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;

namespace Reporting.Application.Service.Abstraction
{
    public interface IBackgroundTaskQueue
    {
        Task QueueAsync(object command, ITenantContext tenantContext, IUserContext userContext, CancellationToken cancellationToken = default);
        Task<QueueMessage> DequeueAsync(CancellationToken cancellationToken);

        public class QueueMessage
        {
            public ITenantContext TenantContext { get; set; }
            public IUserContext UserContext { get; set; }

            public object Command { get; set; }

            public QueueMessage(object command, ITenantContext tenantContext, IUserContext userContext)
            {
                Command = command;
                UserContext = userContext;
                TenantContext = tenantContext;
            }
        }
    }
}