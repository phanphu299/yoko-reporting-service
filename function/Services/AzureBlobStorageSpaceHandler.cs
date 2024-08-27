using System.IO;
using System.Web;
using System.Threading.Tasks;
using AHI.Infrastructure.BlobStorage;
using AHI.Infrastructure.BlobStorage.Internal;
using Reporting.Function.Model;
using Reporting.Function.Constant;
using Reporting.Function.Extension;
using Reporting.Function.Service.Abstraction;
using Newtonsoft.Json;
using System;

namespace Reporting.Function.Service
{
    [Obsolete("Unused for now.")] // #65145 - refactor storage service. Right now, all file management should go through storage service (Native storage handler)
    public class AzureBlobStorageSpaceHandler : IStorageSpaceHandler
    {
        private IBlobManager _blobManager;
        private IFileManager _fileManager;
        private readonly IExportTrackingService _errorService;

        public AzureBlobStorageSpaceHandler(IExportTrackingService errorService)
        {
            _errorService = errorService;
        }

        public async Task<string> GetDownloadFileUrlAsync(string fileUrl, string storageContent)
        {
            try
            {
                var azureBlobContent = JsonConvert.DeserializeObject<AzureBlobContent>(storageContent);
                InitBlobManager(azureBlobContent);

                var extractedFileUrl = fileUrl.ExtractAzureBlobName(azureBlobContent.ContainerName);
                var decodedFileUrl = HttpUtility.UrlDecode(extractedFileUrl);

                // to check the existence of blob file, it will throw an error if the file doesn't exist
                _ = await _blobManager.DownloadBlobAsync(decodedFileUrl);

                return $"{fileUrl}{_blobManager.GrantAccess()}";
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ErrorCode.FAILED_WHILE_DOWNLOADING_FILE_FROM_AZURE_BLOB_STORAGE, ex);
            }

        }

        public async Task DownloadFileAsync(string fileUrl, string storageContent, Stream outputStream)
        {
            try
            {
                var azureBlobContent = JsonConvert.DeserializeObject<AzureBlobContent>(storageContent);
                InitFileManager(azureBlobContent);
                fileUrl = fileUrl.ExtractAzureBlobName(azureBlobContent.ContainerName);
                var decodedFileUrl = HttpUtility.UrlDecode(fileUrl);
                var stream = await _fileManager.DownloadStreamAsync(decodedFileUrl);
                await stream.CopyToAsync(outputStream);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ErrorCode.FAILED_WHILE_DOWNLOADING_FILE_FROM_AZURE_BLOB_STORAGE, ex);
            }
        }

        private void InitBlobManager(AzureBlobContent azureBlobContent)
        {
            var option = new BlobStorageOptions();
            option.ConnectionStringOverride = azureBlobContent.ConnectionString;
            option.DefaultContainer = azureBlobContent.ContainerName;
            option.DefaultFileContainer = azureBlobContent.ContainerName;
            _blobManager = new BlobStorageManager(option);
        }

        private void InitFileManager(AzureBlobContent azureBlobContent)
        {
            var option = new BlobStorageOptions();
            option.ConnectionStringOverride = azureBlobContent.ConnectionString;
            option.DefaultContainer = azureBlobContent.ContainerName;
            option.DefaultFileContainer = azureBlobContent.ContainerName;
            _fileManager = new FileStorageManager(option, azureBlobContent.ContainerName);
        }
    }
}