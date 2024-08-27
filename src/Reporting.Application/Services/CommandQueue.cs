using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using MediatR;
using Reporting.Application.Services.Abstractions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Reporting.Application.Services
{
    public class CommandQueue<T> : ICommandQueue<T> where T : IRequest
    {
        private readonly Channel<(T, ITenantContext, IUserContext)> _channel;

        public CommandQueue()
        {
            _channel = Channel.CreateUnbounded<(T, ITenantContext, IUserContext)>();
        }

        public async Task EnqueueAsync(T command, ITenantContext tenantContext, IUserContext userContext)
        {
            _ = command ?? throw new EntityInvalidException(nameof(command));
            await _channel.Writer.WriteAsync((command, tenantContext, userContext));
        }

        public async Task<(T, ITenantContext, IUserContext)> DequeueAsync(CancellationToken stoppingToken)
        {
            return await _channel.Reader.ReadAsync(stoppingToken);
        }
    }
}
