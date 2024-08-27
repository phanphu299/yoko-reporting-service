using System.IO;
using System.Threading.Tasks;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service
{
    public class NativeStorageSpaceHandler : IStorageSpaceHandler
    {
        private readonly INativeStorageService _storageService;

        public NativeStorageSpaceHandler(INativeStorageService storageService)
        {
            _storageService = storageService;
        }

        public Task<string> GetDownloadFileUrlAsync(string fileUrl, string storageContent)
        {
            return _storageService.GetDownloadFileUrlAsync(fileUrl);
        }

        public Task DownloadFileAsync(string fileUrl, string storageContent, Stream outputStream)
        {
            return _storageService.DownloadFileToStreamAsync(fileUrl, outputStream);
        }
    }
}