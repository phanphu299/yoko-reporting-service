using AHI.Infrastructure.BlobStorage;
using AHI.Infrastructure.BlobStorage.Internal;
using AHI.Infrastructure.Exception;
using Microsoft.Azure.Storage;
using Newtonsoft.Json;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using Reporting.Application.Service.Abstraction;
using System;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    [Obsolete("Unused for now.")] // #65145 - refactor storage service. Right now, all file management should go through storage service (Native storage handler)
    public class AzureBlobStorageSpaceHandler : IStorageSpaceHandler
    {
        private IBlobManager _blobManager;

        public async Task<UploadStorageSpaceDto> HandleAsync(UploadStorageSpace command)
        {
            try
            {
                var current = DateTime.UtcNow;
                var azureBlobContent = JsonConvert.DeserializeObject<AzureBlobContent>(command.StorageContent);
                InitBlobManager(azureBlobContent);
                var fileUrl = await _blobManager.UploadBlobAsync($"{command.FolderName}/{command.FileName}", command.File.ConvertToByteArray());
                return new UploadStorageSpaceDto(fileUrl, command.FolderName, command.FileName);
            }
            catch (StorageException)
            {
                throw new SystemCallServiceException(detailCode: Message.AZURE_BLOB_UPLOAD_FAILED);
            }
        }

        public Task<string> GetDownloadFileUrlAsync(GetDownloadFileUrl command)
        {
            var azureBlobContent = JsonConvert.DeserializeObject<AzureBlobContent>(command.StorageSpaceContent);
            InitBlobManager(azureBlobContent);
            return Task.FromResult($"{command.StorageUrl}{_blobManager.GrantAccess()}");
        }

        public Task<string> GetDownloadFileTokenAsync(string storageSpaceContent)
        {
            var azureBlobContent = JsonConvert.DeserializeObject<AzureBlobContent>(storageSpaceContent);
            InitBlobManager(azureBlobContent);
            return Task.FromResult(_blobManager.GrantAccess());
        }

        private void InitBlobManager(AzureBlobContent azureBlobContent)
        {
            var option = new BlobStorageOptions();
            option.ConnectionStringOverride = azureBlobContent.ConnectionString;
            option.DefaultContainer = azureBlobContent.ContainerName;
            _blobManager = new BlobStorageManager(option);
        }
    }

    public class AzureBlobContent
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}