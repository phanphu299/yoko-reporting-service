using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Reporting.Application.Service.Abstraction;
using System.Collections.Generic;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Template.Command.Handler
{
    public class ArchiveTemplateRequestHandler : IRequestHandler<ArchiveTemplate, IEnumerable<TemplateBasicDto>>
    {
        private readonly ITemplateService _service;

        public ArchiveTemplateRequestHandler(ITemplateService service)
        {
            _service = service;
        }
        
        public Task<IEnumerable<TemplateBasicDto>> Handle(ArchiveTemplate request, CancellationToken cancellationToken)
        {
            return _service.ArchiveAsync(request, cancellationToken);
        }
    }
}
