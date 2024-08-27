using System.IO;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface INativeStorageService
    {
        Task<string> UploadAsync(string fileName, string folderName, Stream data);
        Task<string> UploadCdnAsync(string fileName, string folderName, Stream data);
        Task DownloadFileToStreamAsync(string filePath, Stream outputStream);
        Task<DownloadResponseDto> DownloadFolderAsZipAsync(string folderPath, string fileName, bool @override = false);
        Task<string> GetDownloadFileUrlAsync(string fileUrl);
    }
}