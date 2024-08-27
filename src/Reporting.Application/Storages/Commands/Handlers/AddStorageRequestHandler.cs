using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class AddStorageRequestHandler : IRequestHandler<AddStorage, StorageDto>
    {
        private readonly IStorageService _service;

        public AddStorageRequestHandler(IStorageService service)
        {
            _service = service;
        }

        public Task<StorageDto> Handle(AddStorage request, CancellationToken cancellationToken)
        {
            return _service.AddStorageAsync(request);
        }
    }
}