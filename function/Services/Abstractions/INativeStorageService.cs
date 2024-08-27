using System.IO;
using System.Threading.Tasks;

namespace Reporting.Function.Service.Abstraction
{
    public interface INativeStorageService
    {
        Task<string> UploadAsync(string path, string fileName, Stream data);
        Task DownloadFileToStreamAsync(string filePath, Stream outputStream);
        Task<string> GetDownloadFileUrlAsync(string fileUrl);
    }
}