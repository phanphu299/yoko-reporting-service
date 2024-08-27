using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Extension;
using Newtonsoft.Json.Linq;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class NativeStorageService : INativeStorageService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantContext _tenantContext;

        public NativeStorageService(IHttpClientFactory httpClientFactory, ITenantContext tenantContext)
        {
            _httpClientFactory = httpClientFactory;
            _tenantContext = tenantContext;
        }

        public async Task<string> UploadAsync(string fileName, string folderName, Stream data)
        {
            try
            {
                var httpClient = GetStorageClient();

                var fileContent = new StreamContent(data);
                var response = await UploadFileAsync(httpClient, $"sta/files/{folderName}", fileName, fileContent);

                var responseContent = await response.Content.ReadAsByteArrayAsync();
                var filePath = responseContent.Deserialize<JObject>()["filePath"].ToString();
                return filePath;
            }
            catch (HttpRequestException)
            {
                throw new SystemCallServiceException(detailCode: Message.NATIVE_STORAGE_UPLOAD_FAILED);
            }
        }

        public async Task<string> UploadCdnAsync(string fileName, string folderName, Stream data)
        {
            try
            {
                var httpClient = GetStorageClient();

                var fileContent = new StreamContent(data);
                var response = await UploadFileAsync(httpClient, $"sta/cdn/{folderName}", fileName, fileContent);

                var responseContent = await response.Content.ReadAsByteArrayAsync();
                var filePath = responseContent.Deserialize<JObject>()["filePath"].ToString();
                return filePath;
            }
            catch (HttpRequestException)
            {
                throw new SystemCallServiceException(detailCode: Message.NATIVE_STORAGE_UPLOAD_FAILED);
            }
        }

        // Download file directly to output Stream by using HttpContent.CopyToAsync
        // Using HttpCompletionOption.ResponseHeadersRead to avoid HttpClient buffer http content to internal (memory) stream when receive request
        public async Task DownloadFileToStreamAsync(string filePath, Stream outputStream)
        {
            HttpResponseMessage response = null;
            try
            {
                var httpClient = GetStorageClient();
                var path = await GetLinkAsync(httpClient, filePath);

                var downloadClient = GetDownloadClient();
                response = await downloadClient.GetAsync(path, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                await response.Content.CopyToAsync(outputStream);
            }
            catch (HttpRequestException)
            {
                // Log and return default
                throw new SystemCallServiceException(detailCode: Message.NATIVE_STORAGE_DOWNLOAD_FAILED);
            }
            finally // when using HttpCompletionOption.ResponseHeadersRead, need to explicitly dispose response when finish ASAP, to avoid holding http resources
            {
                response?.Dispose();
            }
        }

        public async Task<DownloadResponseDto> DownloadFolderAsZipAsync(string folderPath, string fileName, bool @override = false)
        {
            // Create request payload
            var requestPayload = new DownloadFolderRequest
            {
                FolderPaths = new List<DownloadFolderPathDto>() {
                    new DownloadFolderPathDto {
                        Path = $"{_tenantContext.SubscriptionId.Replace("-","")}/{_tenantContext.ProjectId.Replace("-","")}/{folderPath}",
                        IsRoot = true
                    }
                },
                FileName = fileName
            };

            var httpClient = GetStorageClient();
            var token = await GetTokenAsync(httpClient);
            var uri = $"sta/files/download?token={token}&output=zip&getInfo=true&override={@override}";
            var content = new StringContent(requestPayload.ToJson(), Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsByteArrayAsync();
            return responseContent.Deserialize<DownloadResponseDto>();
        }

        public async Task<string> GetDownloadFileUrlAsync(string fileUrl)
        {
            var httpClient = GetStorageClient();
            try
            {
                return await GetLinkAsync(httpClient, fileUrl);
            }
            catch (HttpRequestException)
            {
                throw new SystemCallServiceException(detailCode: Message.NATIVE_STORAGE_GET_DOWNLOAD_TOKEN_FAILED);
            }
        }

        private HttpClient GetStorageClient()
        {
            return _httpClientFactory.CreateClient(HttpClientName.STORAGE_SERVICE, _tenantContext);
        }

        private HttpClient GetDownloadClient()
        {
            return _httpClientFactory.CreateClient(HttpClientName.DOWNLOAD_CLIENT);
        }

        private async Task<HttpResponseMessage> UploadFileAsync(HttpClient storageClient, string path, string fileName, HttpContent fileContent)
        {
            var link = await GetLinkAsync(storageClient, path, skipCheckExists: true);
            path = new Uri(link).PathAndQuery.TrimStart('/'); // extract file path from returned url

            HttpResponseMessage response;
            using (var content = new MultipartFormDataContent())
            {
                content.Add(fileContent, "file", fileName);

                response = await storageClient.PostAsync(path, content);
            }
            response.EnsureSuccessStatusCode();
            return response;
        }

        private async Task<string> GetLinkAsync(HttpClient storageClient, string path, bool skipCheckExists = false, int? durationInMinutes = null)
        {
            var requestBody = new { FilePath = path, SkipCheckExists = skipCheckExists, DurationInMinute = durationInMinutes }.ToJson();
            var response = await storageClient.PostAsync($"sta/files/link", new StringContent(requestBody, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        
        private async Task<string> GetTokenAsync(HttpClient client)
        {
            var response = await client.PostAsync("sta/files/token", null);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsByteArrayAsync();
            var message = responseContent.Deserialize<StorageTokenResponse>();
            return message.Token;
        }
    }

    class StorageTokenResponse
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
    }

    class DownloadFolderRequest
    {
        public ICollection<DownloadFolderPathDto> FolderPaths { get; set; } // using add function
        public string FileName { get; set; }
    }

    class DownloadFolderPathDto
    {
        public string Path { get; set; }
        public string ReplaceName { get; set; }
        public bool IsRoot { get; set; }
    }

    public class DownloadResponseDto
    {
        public bool? IsSuccess { get; set; }
        public DownloadResponsesInfoDto Info { get; set; }
    }

    public class DownloadResponsesInfoDto
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Checksum { get; set; }
        public long? Size { get; set; }
        public string ContentType { get; set; }
    }
}