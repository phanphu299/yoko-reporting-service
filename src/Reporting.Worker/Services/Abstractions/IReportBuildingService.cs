using System.IO;
using System.Threading.Tasks;
using Reporting.Application.Command;

namespace Reporting.Worker.Service.Abstraction
{
    public interface IReportBuildingService
    {
        Task<Stream> BuildReportFileAsync(BuildReportFile command);
    }
}