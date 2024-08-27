using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class DeleteTemplate : IRequest<BaseResponse>
    {
        public IEnumerable<int> Ids { get; set; }

        public DeleteTemplate(IEnumerable<int> ids)
        {
            Ids = ids;
        }
    }
}