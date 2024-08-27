using System.IO;
using System.Threading.Tasks;

namespace Reporting.Function.Service.Abstraction
{
    public interface IStorageSpaceHandler
    {
        Task<string> GetDownloadFileUrlAsync(string fileUrl, string storageContent);
        Task DownloadFileAsync(string fileUrl, string storageContent, Stream outputStream);
    }
}