using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class DeleteStorage : IRequest<BaseResponse>
    {
        public IEnumerable<int> Ids { get; set; }

        public DeleteStorage(IEnumerable<int> ids)
        {
            Ids = ids;
        }
    }
}