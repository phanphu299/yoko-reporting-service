using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Application.Command.Handler
{
    public class SearchFailedScheduleRequestHandler : IRequestHandler<SearchFailedSchedule, BaseSearchResponse<FailedScheduleDto>>
    {
        private readonly IFailedScheduleService _service;
        private readonly ISystemContext _systemContext;

        public SearchFailedScheduleRequestHandler(ISystemContext systemContext, IFailedScheduleService service)
        {
            _service = service;
            _systemContext = systemContext;
        }

        public Task<BaseSearchResponse<FailedScheduleDto>> Handle(SearchFailedSchedule request, CancellationToken cancellationToken)
        {
            return _service.SearchAsync(request);
        }
    }
}