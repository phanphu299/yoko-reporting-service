using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Extension;
using Reporting.Application.Service.Abstraction;
using MediatR;
using System;

namespace Reporting.Application.Command.Handler
{
    public class GetDatasetFromTemplateFileRequestHandler : IRequestHandler<GetDatasetFromTemplateFile, BaseSearchResponse<string>>
    {
        private readonly INativeStorageService _service;

        public GetDatasetFromTemplateFileRequestHandler(INativeStorageService service)
        {
            _service = service;
        }

        public async Task<BaseSearchResponse<string>> Handle(GetDatasetFromTemplateFile request, CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var templateFile = new System.IO.MemoryStream();
            await _service.DownloadFileToStreamAsync(request.TemplateFileUrl, templateFile);
            templateFile.Seek(0, System.IO.SeekOrigin.Begin);
            var templateFileXml = XElement.Load(templateFile).RemoveAllNamespaces();
            var datasets = templateFileXml.Descendants("DataSet").Select(x => x.Attribute("Name").Value).ToArray();
            var end = DateTime.UtcNow;
            var response = new BaseSearchResponse<string>((long)(end - start).TotalMilliseconds, datasets.Count(), int.MaxValue, 0, datasets);
            return response;
        }
    }
}