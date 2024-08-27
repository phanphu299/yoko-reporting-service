using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;
using Newtonsoft.Json;
using AHI.Infrastructure.MultiTenancy.Extension;

namespace Reporting.Application.Service
{
    public class JobService : IJobService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantContext _tenantContext;
        private readonly IConfiguration _configuration;

        public JobService(IHttpClientFactory httpClientFactory, ITenantContext tenantContext, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _tenantContext = tenantContext;
            _configuration = configuration;
        }

        public async Task<JobDto> AddJobAsync(JobDto model)
        {
            var payload = JsonConvert.SerializeObject(model);
            var httpClient = _httpClientFactory.CreateClient(HttpClientName.SCHEDULER_SERVICE, _tenantContext);
            var response = await httpClient.PostAsync($"sch/jobs/recurring", new StringContent(payload, Encoding.UTF8, MediaType.APPLICATION_JSON));
            return await response.ReadContentAsync<JobDto>(true);
        }

        public async Task<JobDto> UpdateJobAsync(JobDto model)
        {
            var payload = JsonConvert.SerializeObject(model);
            var httpClient = _httpClientFactory.CreateClient(HttpClientName.SCHEDULER_SERVICE, _tenantContext);
            var response = await httpClient.PutAsync($"sch/jobs/{model.Id}/recurring", new StringContent(payload, Encoding.UTF8, MediaType.APPLICATION_JSON));
            return await response.ReadContentAsync<JobDto>(true);
        }

        public async Task<BaseResponse> DeleteJobAsync(string id)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientName.SCHEDULER_SERVICE, _tenantContext);
            var response = await httpClient.DeleteAsync($"sch/jobs/{id}/recurring");
            return await response.ReadContentAsync<BaseResponse>(true);
        }
    }
}