using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class GetStorageRequestHandler : IRequestHandler<GetStorage, StorageDto>
    {
        private readonly IStorageService _service;

        public GetStorageRequestHandler(IStorageService service)
        {
            _service = service;
        }

        public Task<StorageDto> Handle(GetStorage request, CancellationToken cancellationToken)
        {
            return _service.GetStorageByIdAsync(request);
        }
    }
}