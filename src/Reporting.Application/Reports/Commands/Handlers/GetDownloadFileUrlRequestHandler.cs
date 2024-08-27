using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command.Handler
{
    public class GetDownloadFileUrlRequestHandler : IRequestHandler<GetDownloadFileUrl, DownloadFileUrlDto>
    {
        private readonly IReportService _service;

        public GetDownloadFileUrlRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<DownloadFileUrlDto> Handle(GetDownloadFileUrl request, CancellationToken cancellationToken)
        {
            return _service.GetDownloadFileUrlAsync(request);
        }
    }
}