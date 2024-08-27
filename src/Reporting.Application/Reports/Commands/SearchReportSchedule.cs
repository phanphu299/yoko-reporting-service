using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;

namespace Reporting.Application.Command
{
    public class SearchReportSchedule : BaseCriteria, IRequest<BaseSearchResponse<ReportScheduleDto>>
    {
        public bool ClientOverride { get; set; } = false;

        public SearchReportSchedule()
        {
            PageSize = 20;
            PageIndex = 0;
            Sorts = DefaultSearch.DEFAULT_SORT_BY_CREATED;
        }
    }
}