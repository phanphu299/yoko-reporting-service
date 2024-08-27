using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command.Handler
{
    public class GetReportRequestHandler : IRequestHandler<GetReport, ReportDetailDto>
    {
        private readonly IReportService _service;

        public GetReportRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<ReportDetailDto> Handle(GetReport request, CancellationToken cancellationToken)
        {
            return _service.GetReportByIdAsync(request);
        }
    }
}