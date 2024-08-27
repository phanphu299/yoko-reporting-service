using System;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Constant;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;

namespace Template.Application.Command.Handler
{
    public class SearchStorageTypeRequestHandler : IRequestHandler<SearchStorageType, BaseSearchResponse<StorageTypeDto>>
    {
        private readonly IStorageTypeService _service;
        private readonly ISystemContext _systemContext;

        public SearchStorageTypeRequestHandler(ISystemContext systemContext, IStorageTypeService service)
        {
            _service = service;
            _systemContext = systemContext;
        }

        public async Task<BaseSearchResponse<StorageTypeDto>> Handle(SearchStorageType request, CancellationToken cancellationToken)
        {
            if (request.ClientOverride == false)
            {
                var pageSize = await _systemContext.GetValueAsync(DefaultSearch.DEFAULT_SEARCH_PAGE_SIZE, DefaultSearch.DEFAULT_VALUE_PAGE_SIZE);
                request.PageSize = Convert.ToInt32(pageSize);
            }
            return await _service.SearchAsync(request);
        }
    }
}