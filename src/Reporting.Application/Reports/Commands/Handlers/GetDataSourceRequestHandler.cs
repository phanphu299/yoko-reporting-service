using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class GetDataSourceRequestHandler : IRequestHandler<GetDataSource, IEnumerable<DataTableResult>>
    {
        private readonly IReportService _service;

        public GetDataSourceRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<IEnumerable<DataTableResult>> Handle(GetDataSource request, CancellationToken cancellationToken)
        {
            return _service.GetDataSourceAsync(request);
        }
    }
}