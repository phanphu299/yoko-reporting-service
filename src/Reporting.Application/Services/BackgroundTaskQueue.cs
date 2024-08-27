using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.UserContext.Extension;
using Microsoft.Extensions.Logging;
using Reporting.Application.Service.Abstraction;
using static Reporting.Application.Service.Abstraction.IBackgroundTaskQueue;

namespace Reporting.Application.Service
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<QueueMessage> _channel;
        private readonly ILogger<BackgroundTaskQueue> _logger;

        public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger)
        {
            _channel = Channel.CreateUnbounded<QueueMessage>();
            _logger = logger;
        }

        public async Task QueueAsync(object command, ITenantContext tenantContext, IUserContext userContext, CancellationToken cancellationToken = default)
        {
            tenantContext = tenantContext?.Clone();
            userContext = userContext?.Clone();
            await _channel.Writer.WriteAsync(new QueueMessage(command, tenantContext, userContext), cancellationToken);
            // _logger.LogTrace($"Enqueue task: {command.GetType().Name}");
        }

        public async Task<QueueMessage> DequeueAsync(CancellationToken cancellationToken)
        {
            var message = await _channel.Reader.ReadAsync(cancellationToken);
            // _logger.LogTrace($"Dequeue task: {message.Command?.GetType().Name}");
            return message;
        }
    }
}