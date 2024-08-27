using System.Threading.Tasks;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class NativeStorageSpaceHandler : IStorageSpaceHandler
    {
        private readonly INativeStorageService _storageService;

        public NativeStorageSpaceHandler(INativeStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<UploadStorageSpaceDto> HandleAsync(UploadStorageSpace command)
        {
            var fileUrl = await _storageService.UploadAsync(command.FileName, command.FolderName, command.File);
            return new UploadStorageSpaceDto(fileUrl, command.FolderName, command.FileName);
        }

        public Task<string> GetDownloadFileUrlAsync(GetDownloadFileUrl command)
        {
            return _storageService.GetDownloadFileUrlAsync(command.StorageUrl);
        }
    }
}