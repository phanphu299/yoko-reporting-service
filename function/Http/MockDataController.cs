using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Reporting.Function.Extension;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Reporting.Function.Service.RabbitMQ
{
    public class MockDataController
    {
        private static string API_KEY = "x-api-key";
        private static string API_SECRET = "pPXk7Qwpj5Au0H36NXel";

        [FunctionName("UpsertMockDataAsync")]
        public async Task<IActionResult> UpsertMockDataAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "fnc/rpt/mockdata")] HttpRequestMessage req, ILogger logger)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<JToken>(requestBody);
            var id = Guid.NewGuid();
            File.WriteAllText($"{id}.json", requestBody);
            return new OkObjectResult(new { Id = id, Data = data });
        }

        [FunctionName("GetMockData")]
        public IActionResult GetMockData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "fnc/rpt/mockdata/{id}")] HttpRequestMessage req, string id, ILogger logger)
        {
            if (!ValidateRequest(req))
                return new UnauthorizedResult();

            if (!File.Exists($"{id}.json"))
                return new NotFoundResult();

            JToken data = null;
            using (var r = new StreamReader($"{id}.json"))
            {
                string json = r.ReadToEnd();
                data = json.TryExtractJsonObject();
            }
            return new OkObjectResult(data);
        }

        private bool ValidateRequest(HttpRequestMessage req)
        {
            IEnumerable<string> headerValues = Array.Empty<string>();
            string headerValue = string.Empty;

            if (req.Headers.TryGetValues(API_KEY, out headerValues))
            {
                headerValue = headerValues.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(headerValue) || headerValue != API_SECRET)
            {
                return false;
            }

            return true;
        }
    }
}