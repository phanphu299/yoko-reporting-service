using System;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using MediatR;

namespace Template.Application.Command.Handler
{
    public class SearchReportRequestHandler : IRequestHandler<SearchReport, BaseSearchResponse<ReportDto>>
    {
        private readonly IReportService _service;
        private readonly ISystemContext _systemContext;

        public SearchReportRequestHandler(ISystemContext systemContext, IReportService service)
        {
            _service = service;
            _systemContext = systemContext;
        }

        public async Task<BaseSearchResponse<ReportDto>> Handle(SearchReport request, CancellationToken cancellationToken)
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