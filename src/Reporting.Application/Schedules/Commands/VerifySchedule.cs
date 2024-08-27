using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace Reporting.Application.Schedule.Command
{
    public class VerifySchedule : IRequest<BaseResponse>
    {
        public string Data { get; set; }
    }
}
