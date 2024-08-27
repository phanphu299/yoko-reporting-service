using System;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.UserContext.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace Reporting.Application.Extension
{
    public static class ServiceProviderExtension
    {
        public static IServiceScope CreateScope(this IServiceProvider serviceProvider, ITenantContext tenantContext, IUserContext userContext = null)
        {
            var scope = serviceProvider.CreateScope();

            var scopeTenantContext = scope.ServiceProvider.GetService<ITenantContext>();
            tenantContext.CopyTo(scopeTenantContext);

            if (userContext != null)
            {
                var scopeUserContext = scope.ServiceProvider.GetService<IUserContext>();
                userContext.CopyTo(scopeUserContext);
            }

            return scope;
        }
    }
}