using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Schedule.Command.Handler
{
    public class VerifyArchiveScheduleRequestHandler : IRequestHandler<VerifySchedule, BaseResponse>
    {
        private readonly IScheduleService _service;

        public VerifyArchiveScheduleRequestHandler(IScheduleService service)
        {
            _service = service;
        }

        public async Task<BaseResponse> Handle(VerifySchedule request, CancellationToken cancellationToken)
        {
            var result = await _service.VerifyArchiveAsync(request, cancellationToken);
            return new BaseResponse(result, null);
        }
    }
}
