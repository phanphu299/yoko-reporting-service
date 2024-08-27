using System.IO;
using System.Threading.Tasks;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Service.Abstraction
{
    public interface IWorkerService
    {
        Task<Stream> BuildReportFileAsync(BuildReportFile command);
        Task<PreviewReportFileDto> PreviewReportFileAsync(PreviewReportFile command);
    }
}