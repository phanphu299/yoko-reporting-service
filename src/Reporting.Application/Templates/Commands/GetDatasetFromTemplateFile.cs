using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class GetDatasetFromTemplateFile : IRequest<BaseSearchResponse<string>>
    {
        public string TemplateFileUrl { get; set; }

        public GetDatasetFromTemplateFile(string templateFileUrl)
        {
            TemplateFileUrl = templateFileUrl;
        }
    }
}