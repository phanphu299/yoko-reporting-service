using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class DeleteTemplateRequestHandler : IRequestHandler<DeleteTemplate, BaseResponse>
    {
        private readonly ITemplateService _service;

        public DeleteTemplateRequestHandler(ITemplateService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(DeleteTemplate request, CancellationToken cancellationToken)
        {
            return _service.DeleteTemplateAsync(request);
        }
    }
}