using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Constant;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class SearchDataSourceType : BaseCriteria, IRequest<BaseSearchResponse<DataSourceTypeDto>>
    {
        public bool ClientOverride { get; set; } = false;

        public SearchDataSourceType()
        {
            PageSize = 20;
            PageIndex = 0;
            Sorts = DefaultSearch.DEFAULT_SORT;
        }
    }
}