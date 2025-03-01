using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Reporting.Function.Service.RabbitMQ
{
    public class HealthCheckController
    {
        [FunctionName("HealthProbeCheck")]
        public IActionResult LivenessProbeCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fnc/healthz")] HttpRequestMessage req, ILogger logger)
        {
            return new OkResult();
        }
    }
}