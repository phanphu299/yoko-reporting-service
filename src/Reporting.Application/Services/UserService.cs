using System;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Extension;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantContext _tenantContext;

        public UserService(IHttpClientFactory httpClientFactory, ITenantContext tenantContext)
        {
            _httpClientFactory = httpClientFactory;
            _tenantContext = tenantContext;
        }

        public async Task<ContactDto> FetchContactAsync(Guid contactId)
        {
            var client = _httpClientFactory.CreateClient(HttpClientName.USER_SERVICE, _tenantContext);
            var response = await client.GetAsync($"usr/contacts/{contactId}/fetch");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            return content.Deserialize<ContactDto>();
        }

        public async Task<ContactGroupDto> FetchContactGroupAsync(Guid contactGroupId)
        {
            var client = _httpClientFactory.CreateClient(HttpClientName.USER_SERVICE, _tenantContext);
            var response = await client.GetAsync($"usr/contacts/groups/{contactGroupId}/fetch");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            return content.Deserialize<ContactGroupDto>();
        }
    }
}