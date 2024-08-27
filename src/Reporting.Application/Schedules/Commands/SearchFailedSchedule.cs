using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Constant;
using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class SearchFailedSchedule : BaseCriteria, IRequest<BaseSearchResponse<FailedScheduleDto>>
    {
        public bool ClientOverride { get; set; } = false;

        public SearchFailedSchedule()
        {
            PageSize = 20;
            PageIndex = 0;
            Sorts = DefaultSearch.DEFAULT_SORT;
        }
    }
}