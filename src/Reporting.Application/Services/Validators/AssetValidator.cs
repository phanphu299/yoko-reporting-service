using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Extension;
using Reporting.Application.Constant;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class AssetValidator : AMDataSourceValidator
    {
        public AssetValidator(ITenantContext tenantContext, IHttpClientFactory httpClientFactory) : base(tenantContext, httpClientFactory)
        {
        }

        public override async Task<Dictionary<string, string>> ValidateAsync(RequestAmContent requestAmContent)
        {
            if (_tenantContext == null)
                return _deletedItems;

            var deviceHttpClient = _httpClientFactory.CreateClient(HttpClientName.DEVICE_SERVICE, _tenantContext);
            var getAssetByIdResponse = await deviceHttpClient.GetAsync($"dev/assets/{requestAmContent.AssetId}?useCache=false");
            if (getAssetByIdResponse.StatusCode == HttpStatusCode.NotFound)
            {
                _deletedItems.Add(requestAmContent.AssetId.ToString(), AMModelType.Asset);
            }
            else
            {
                var responseContent = await getAssetByIdResponse.Content.ReadAsByteArrayAsync();
                _asset = responseContent.Deserialize<GetAssetByIdResponse>();

                if (_asset == null)
                {
                    _deletedItems.Add(requestAmContent.AssetId.ToString(), AMModelType.Asset);
                }
            }

            if (_deletedItems.Any())
            {
                return _deletedItems;
            }

            if (_nextValidator != null)
            {
                _nextValidator._asset = _asset;
                return await _nextValidator.ValidateAsync(requestAmContent);
            }

            return _deletedItems;
        }
    }
}