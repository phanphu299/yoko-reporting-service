using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace Reporting.Application.Template.Command
{
    public class VerifyArchivedTemplate : IRequest<BaseResponse>
    {
        public string Data { get; set; }
    }
}
