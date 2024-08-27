using AHI.Infrastructure.MultiTenancy.Abstraction;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public abstract class AMDataSourceValidator
    {
        protected AMDataSourceValidator _nextValidator;
        protected readonly ITenantContext _tenantContext;
        protected readonly IHttpClientFactory _httpClientFactory;
        public GetAssetByIdResponse _asset;
        protected Dictionary<string, string> _deletedItems; 

        public AMDataSourceValidator(ITenantContext tenantContext, IHttpClientFactory httpClientFactory)
        {
            _tenantContext = tenantContext;
            _httpClientFactory = httpClientFactory;
            _deletedItems = new Dictionary<string, string>();
        }

        public void SetNextValidator(AMDataSourceValidator nextValidator)
        {
            this._nextValidator = nextValidator;
        }

        public abstract Task<Dictionary<string, string>> ValidateAsync(RequestAmContent requestAmContent);
    }
}