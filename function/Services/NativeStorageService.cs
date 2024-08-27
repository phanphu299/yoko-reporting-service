using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Reporting.Function.Constant;
using Reporting.Function.Service.Abstraction;
using Newtonsoft.Json.Linq;
using AHI.Infrastructure.MultiTenancy.Extension;
using System;
using System.Text;

namespace Reporting.Function.Service
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

        public async Task<string> UploadAsync(string path, string fileName, Stream data)
        {
            try
            {
                var httpClient = GetStorageClient();

                var fileContent = new StreamContent(data);
                var response = await UploadFileAsync(httpClient, path, fileName, fileContent);

                var responseContent = await response.Content.ReadAsByteArrayAsync();
                var filePath = responseContent.Deserialize<JObject>()["filePath"].ToString();
                return filePath;
            }
            catch (HttpRequestException ex)
            {
                throw new System.Exception(ErrorCode.FAILED_WHILE_UPLOADING_FILE_TO_NATIVE_STORAGE, ex);
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
            catch (HttpRequestException ex)
            {
                throw new System.Exception(ErrorCode.FAILED_WHILE_DOWNLOADING_FILE_FROM_NATIVE_STORAGE, ex);
            }
            finally // when using HttpCompletionOption.ResponseHeadersRead, need to explicitly dispose response when finish ASAP, to avoid holding http resources
            {
                response?.Dispose();
            }
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
                throw new System.Exception(ErrorCode.FAILED_WHILE_DOWNLOADING_FILE_FROM_NATIVE_STORAGE);
            }
        }

        private HttpClient GetStorageClient()
        {
            return _httpClientFactory.CreateClient(ClientName.STORAGE_SERVICE, _tenantContext);
        }

        private HttpClient GetDownloadClient()
        {
            return _httpClientFactory.CreateClient(ClientName.DOWNLOAD_CLIENT);
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

        private async Task<string> GetLinkAsync(HttpClient storageClient, string path, bool skipCheckExists = false)
        {
            var requestBody = new { FilePath = path, SkipCheckExists = skipCheckExists }.ToJson();
            var response = await storageClient.PostAsync($"sta/files/link", new StringContent(requestBody, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}