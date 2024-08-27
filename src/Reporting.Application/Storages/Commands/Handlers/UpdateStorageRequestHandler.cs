using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class UpdateStorageRequestHandler : IRequestHandler<UpdateStorage, StorageDto>
    {
        private readonly IStorageService _service;

        public UpdateStorageRequestHandler(IStorageService service)
        {
            _service = service;
        }

        public Task<StorageDto> Handle(UpdateStorage request, CancellationToken cancellationToken)
        {
            return _service.UpdateStorageAsync(request);
        }
    }
}