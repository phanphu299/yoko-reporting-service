using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Reporting.Function.Constant;
using Reporting.Function.Model;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service
{
    public class MasterService : IMasterService
    {
        private readonly ITenantContext _tenantContext;
        private readonly ICache _cache;
        private readonly IHttpClientFactory _httpClientFactory;

        public MasterService(ITenantContext tenantContext, ICache cache, IHttpClientFactory httpClientFactory)
        {
            _tenantContext = tenantContext;
            _cache = cache;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectBySubscriptionAsync()
        {
            var key = RedisKey.ALL_PROJECT_KEY;
            IEnumerable<ProjectDto> result = await _cache.GetAsync<IEnumerable<ProjectDto>>(key);
            if (result == null)
            {
                var httpClient = _httpClientFactory.CreateClient(ClientName.MASTER_FUNCTION);
                var response = await httpClient.GetAsync($"fnc/mst/projects?migrated=true");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsByteArrayAsync();
                result = content.Deserialize<IEnumerable<ProjectDto>>();
            }
            return result.Where(x => !x.Deleted && x.IsMigrated && x.SubscriptionResourceId == _tenantContext.SubscriptionId && x.ProjectType == ProjectDto.ASSET_DASHBOARD_TYPE);
        }
    }
}