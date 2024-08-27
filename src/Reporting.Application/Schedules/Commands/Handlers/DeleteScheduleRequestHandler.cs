using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class DeleteScheduleRequestHandler : IRequestHandler<DeleteSchedule, BaseResponse>
    {
        private readonly IScheduleService _service;

        public DeleteScheduleRequestHandler(IScheduleService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(DeleteSchedule request, CancellationToken cancellationToken)
        {
            return _service.DeleteScheduleAsync(request);
        }
    }
}