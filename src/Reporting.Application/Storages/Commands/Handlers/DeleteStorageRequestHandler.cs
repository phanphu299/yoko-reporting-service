using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class DeleteStorageRequestHandler : IRequestHandler<DeleteStorage, BaseResponse>
    {
        private readonly IStorageService _service;

        public DeleteStorageRequestHandler(IStorageService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(DeleteStorage request, CancellationToken cancellationToken)
        {
            return _service.DeleteStorageAsync(request);
        }
    }
}