using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using System.Collections.Generic;

namespace Reporting.Application.Template.Command
{
    public class RetrieveTemplate : IRequest<BaseResponse>
    {
        public string Data { get; set; }
        public IDictionary<string, object> AdditionalData { get; set; }
        public string Upn { get; set; }
    }
}
