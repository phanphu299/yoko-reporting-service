using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Application.Command.Handler
{
    public class SearchReportScheduleRequestHandler : IRequestHandler<SearchReportSchedule, BaseSearchResponse<ReportScheduleDto>>
    {
        private readonly IReportScheduleService _service;
        private readonly ISystemContext _systemContext;

        public SearchReportScheduleRequestHandler(ISystemContext systemContext, IReportScheduleService service)
        {
            _systemContext = systemContext;
            _service = service;
        }

        public async Task<BaseSearchResponse<ReportScheduleDto>> Handle(SearchReportSchedule request, CancellationToken cancellationToken)
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