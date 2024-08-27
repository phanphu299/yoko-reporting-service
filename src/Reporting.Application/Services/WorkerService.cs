using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    /// <summary>
    // Because function filling & generating report file just work in windows, we need to call to
    // reporting hosted on webapp to do the job
    /// </summary> <summary
    public class WorkerService : IWorkerService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantContext _tenantContext;
        private readonly int _httpTimeoutInSeconds;
        private readonly ILoggerAdapter<WorkerService> _logger;

        public WorkerService(IHttpClientFactory httpClientFactory,
                            ITenantContext tenantContext,
                            IConfiguration configuration,
                            ILoggerAdapter<WorkerService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tenantContext = tenantContext;
            _logger = logger;

            // Default is 5 minutes
            _httpTimeoutInSeconds = Convert.ToInt32(configuration["HttpTimeoutInSeconds"] ?? "300");
        }

        public async Task<Stream> BuildReportFileAsync(BuildReportFile command)
        {
            var stream = new MemoryStream();
            try
            {
                var httpClient = _httpClientFactory.CreateClient(HttpClientName.REPORTING_SERVICE, _tenantContext);

                // The report service will take longer when building a big report
                httpClient.Timeout = TimeSpan.FromSeconds(_httpTimeoutInSeconds);

                var jsonBody = JsonConvert.SerializeObject(command);
                var response = await httpClient.PostAsync($"rpt/workers/build", new StringContent(jsonBody, Encoding.UTF8, mediaType: MediaType.APPLICATION_JSON));
                response.EnsureSuccessStatusCode();
                using (var body = await response.Content.ReadAsStreamAsync())
                {
                    await body.CopyToAsync(stream);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, ex.Message);
                await stream.DisposeAsync();
                throw new SystemCallServiceException(detailCode: Message.LOCAL_REPORT_FILL_FAILED);
            }
            return stream;
        }

        public async Task<PreviewReportFileDto> PreviewReportFileAsync(PreviewReportFile command)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(HttpClientName.REPORTING_SERVICE, _tenantContext);
                var jsonBody = JsonConvert.SerializeObject(command);
                var response = await httpClient.PostAsync($"rpt/workers/preview", new StringContent(jsonBody, Encoding.UTF8, mediaType: MediaType.APPLICATION_JSON));
                response.EnsureSuccessStatusCode();

                var message = await response.Content.ReadAsByteArrayAsync();
                return message.Deserialize<PreviewReportFileDto>();
            }
            catch (HttpRequestException ex)
            {
                throw new SystemCallServiceException(detailCode: Message.LOCAL_REPORT_FILL_FAILED);
            }
        }
    }
}