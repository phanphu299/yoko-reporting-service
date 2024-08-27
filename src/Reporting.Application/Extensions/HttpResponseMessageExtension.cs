using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Extension;

namespace Reporting.Application.Extension
{
    public static class HttpResponseMessageExtension
    {
        public static async Task<T> ReadContentAsync<T>(this HttpResponseMessage response, bool ensureSuccessStatus = true)
        {
            if (ensureSuccessStatus)
            {
                response.EnsureSuccessStatusCode();
            }
            var responseContent = await response.Content.ReadAsByteArrayAsync();
            return responseContent.Deserialize<T>();
        }
    }
}