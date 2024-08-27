using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;

namespace Template.Application.Command.Handler
{
    public class SearchScheduleTypeRequestHandler : IRequestHandler<SearchScheduleType, BaseSearchResponse<ScheduleTypeDto>>
    {
        private readonly IScheduleTypeService _service;

        public SearchScheduleTypeRequestHandler(IScheduleTypeService service)
        {
            _service = service;
        }

        public async Task<BaseSearchResponse<ScheduleTypeDto>> Handle(SearchScheduleType request, CancellationToken cancellationToken)
        {
            return await _service.SearchAsync(request);
        }
    }
}