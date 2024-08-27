using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;
using System.Collections.Generic;

namespace Reporting.Application.Command.Handler
{
    public class RunFailedScheduleRequestHandler : IRequestHandler<RunFailedSchedule, IEnumerable<GenerateReportDto>>
    {
        private readonly IFailedScheduleService _service;

        public RunFailedScheduleRequestHandler(IFailedScheduleService service)
        {
            _service = service;
        }

        public Task<IEnumerable<GenerateReportDto>> Handle(RunFailedSchedule request, CancellationToken cancellationToken)
        {
            return _service.RunFailedScheduleAsync(request);
        }
    }
}