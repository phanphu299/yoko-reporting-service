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
    public class ProjectValidator : AMDataSourceValidator
    {
        public ProjectValidator(ITenantContext tenantContext, IHttpClientFactory httpClientFactory) : base(tenantContext, httpClientFactory)
        {
        }

        public override async Task<Dictionary<string, string>> ValidateAsync(RequestAmContent requestAmContent)
        {
            if (_tenantContext == null)
                return _deletedItems;

            var projectHttpClient = _httpClientFactory.CreateClient(HttpClientName.PROJECT_SERVICE, _tenantContext);
            var checkExistProjectResponse = await projectHttpClient.GetAsync($"prj/projects/{requestAmContent.ProjectId}");
            if (checkExistProjectResponse.StatusCode == HttpStatusCode.NotFound)
            {
                _deletedItems.Add(requestAmContent.AssetId.ToString(), AMModelType.Asset);
            }

            if (_deletedItems.Any())
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