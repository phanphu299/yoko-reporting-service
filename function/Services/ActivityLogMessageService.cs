using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Extension;
using Function.Constants;
using Reporting.Function.Constant;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service
{
    public class ActivityLogMessageService : IActivityLogMessageService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ActivityLogMessageService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<Dictionary<string, string>> GetMessagesAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient(ClientName.CDN);
                var response = await client.GetAsync(Endpoint.ENGLISH_TRANSLATION_FILE);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var messages = content.Deserialize<Dictionary<string, string>>();
                    return messages;
                }
                return new Dictionary<string, string>();
            }
            catch (HttpRequestException)
            {
                return new Dictionary<string, string>();
            }
        }

        public async Task<string> GetMessageAsync(string messageCode)
        {
            var messages = await GetMessagesAsync();
            if (messages.TryGetValue(messageCode, out var message))
            {
                return message;
            }
            return messageCode;
        }
    }
}