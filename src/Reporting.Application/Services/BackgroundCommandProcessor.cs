using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reporting.Application.Services.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Services
{
    internal class BackgroundCommandProcessor<T> : BackgroundService where T : IRequest
    {
        private readonly ICommandQueue<T> _commandQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILoggerAdapter<BackgroundCommandProcessor<T>> _logger;

        public BackgroundCommandProcessor(ICommandQueue<T> commandQueue,
            IServiceScopeFactory serviceScopeFactory,
            ILoggerAdapter<BackgroundCommandProcessor<T>> logger)
        {
            _commandQueue = commandQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    (var command, var tenantContext, var userContext) = await _commandQueue.DequeueAsync(stoppingToken);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var targetTenantContext = scope.ServiceProvider.GetService(typeof(ITenantContext)) as ITenantContext;
                    targetTenantContext.SetTenantId(tenantContext.TenantId);
                    targetTenantContext.SetSubscriptionId(tenantContext.SubscriptionId);
                    targetTenantContext.SetProjectId(tenantContext.ProjectId);

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(command);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
