using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using Reporting.Application.Constant;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class SubscriptionValidator : AMDataSourceValidator
    {
        public SubscriptionValidator(ITenantContext tenantContext, IHttpClientFactory httpClientFactory) : base(tenantContext, httpClientFactory)
        {
        }

        public override async Task<Dictionary<string, string>> ValidateAsync(RequestAmContent requestAmContent)
        {
            if (_tenantContext == null)
                return _deletedItems;

            var tenantHttpClient = _httpClientFactory.CreateClient(HttpClientName.TENANT_SERVICE, _tenantContext);
            var checkExistSubscriptionResponse = await tenantHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"tnt/subscriptions/{requestAmContent.SubscriptionId}"));
            if (checkExistSubscriptionResponse.StatusCode == HttpStatusCode.NotFound)
            {
                _deletedItems.Add(requestAmContent.AssetId.ToString(), AMModelType.Asset);
            }

            if(_deletedItems.Any())
            {
                return _deletedItems;
            }

            if (_nextValidator != null)
            {
                return await _nextValidator.ValidateAsync(requestAmContent);
            }

            return _deletedItems;
        }
    }
}