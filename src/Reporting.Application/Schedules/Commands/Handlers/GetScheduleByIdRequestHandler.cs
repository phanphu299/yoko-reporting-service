using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class GetScheduleByIdRequestHandler : IRequestHandler<GetScheduleById, ScheduleDto>
    {
        private readonly IScheduleService _service;

        public GetScheduleByIdRequestHandler(IScheduleService service)
        {
            _service = service;
        }

        public Task<ScheduleDto> Handle(GetScheduleById request, CancellationToken cancellationToken)
        {
            return _service.GetScheduleByIdAsync(request);
        }
    }
}