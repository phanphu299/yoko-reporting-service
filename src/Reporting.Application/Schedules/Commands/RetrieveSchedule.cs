using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace Reporting.Application.Schedule.Command
{
    public class RetrieveSchedule : IRequest<BaseResponse>
    {
        public string Data { get; set; }
        public string Upn { get; set; }
    }
}
