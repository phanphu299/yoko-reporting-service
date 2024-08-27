using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class UploadStorageSpaceRequestHandler : IRequestHandler<UploadStorageSpace, UploadStorageSpaceDto>
    {
        private readonly IReportService _service;

        public UploadStorageSpaceRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<UploadStorageSpaceDto> Handle(UploadStorageSpace request, CancellationToken cancellationToken)
        {
            return _service.UploadStorageSpaceAsync(request);
        }
    }
}