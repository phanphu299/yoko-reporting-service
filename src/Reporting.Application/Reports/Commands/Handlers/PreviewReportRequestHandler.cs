using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class PreviewReportRequestHandler : IRequestHandler<PreviewReport, PreviewReportFileDto>
    {
        private readonly IReportService _service;

        public PreviewReportRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<PreviewReportFileDto> Handle(PreviewReport request, CancellationToken cancellationToken)
        {
            return _service.PreviewReportAsync(request);
        }
    }
}