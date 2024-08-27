using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class DeleteSchedule : IRequest<BaseResponse>
    {
        public IEnumerable<int> Ids { get; set; }

        public DeleteSchedule(IEnumerable<int> ids)
        {
            Ids = ids;
        }
    }
}