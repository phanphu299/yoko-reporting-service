using System.IO;
using System.Threading.Tasks;
using Reporting.Application.Command;
using Reporting.Application.Service.Abstraction;
using Reporting.Worker.Builder;
using Reporting.Worker.Service.Abstraction;

namespace Reporting.Worker.Service
{
    public class ReportBuildingService : IReportBuildingService
    {
        private readonly INativeStorageService _storageService;

        public ReportBuildingService(INativeStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<Stream> BuildReportFileAsync(BuildReportFile command)
        {
            var file = new System.IO.MemoryStream();

            await _storageService.DownloadFileToStreamAsync(command.Template.TemplateFileUrl, file);
            file.Seek(0, System.IO.SeekOrigin.Begin);

            var builder = new LocalReportBuilder(command.FromDate, command.ToDate);

            builder.SetTemplate(command.Template);
            builder.SetDataTableResults(command.Data);
            builder.SetTemplateFile(file);

            var filledStream = builder.BuildDataTables().BuildParams().BuildReportFile();

            filledStream.Position = 0;

            return filledStream;
        }
    }
}