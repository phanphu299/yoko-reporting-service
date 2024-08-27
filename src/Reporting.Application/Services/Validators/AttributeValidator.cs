using AHI.Infrastructure.MultiTenancy.Abstraction;
using Reporting.Application.Constant;
using Reporting.Application.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Reporting.Application.Services.Validators
{
    public class AttributeValidator : AMDataSourceValidator
    {
        public AttributeValidator(ITenantContext tenantContext, IHttpClientFactory httpClientFactory) : base(tenantContext, httpClientFactory)
        {
        }

        public override async Task<Dictionary<string, string>> ValidateAsync(RequestAmContent requestAmContent)
        {
            if (_asset == null)
                return _deletedItems;

            var attributeIdList = _asset.Attributes.Select(x => x.Id);
            foreach (var attribute in requestAmContent.Attributes)
            {
                if (!attributeIdList.Contains(attribute.Id.ToString()))
                {
                    _deletedItems.Add(attribute.Id.ToString(), AMModelType.Attribute);
                }
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