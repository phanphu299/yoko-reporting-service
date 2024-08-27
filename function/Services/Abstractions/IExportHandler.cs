using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Reporting.Function.Model;

namespace Reporting.Function.Service.Abstraction
{
    public interface IExportHandler
    {
        Task<string> HandleAsync(ExecutionContext context, object data);
        Task<int> GetDataAsync(DownloadReportRequest request);
    }
}