using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Internal;
using AHI.Infrastructure.UserContext.Abstraction;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Services.Abstractions
{
    public interface ICommandQueue<T> where T : IRequest
    {
        Task EnqueueAsync(T command, ITenantContext tenantContext, IUserContext userContext);
        Task<(T, ITenantContext, IUserContext)> DequeueAsync(CancellationToken stoppingToken);
    }
}
