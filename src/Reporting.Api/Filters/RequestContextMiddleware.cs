using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Api.Filter
{
    public class RequestContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public RequestContextMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            using (MemoryStream stream = new MemoryStream())
            {
                await httpContext.Request.Body.CopyToAsync(stream);
                var body = Encoding.UTF8.GetString(stream.ToArray());
                var requestContext = httpContext.RequestServices.GetService(typeof(IRequestContext)) as IRequestContext;
                requestContext.SetBody(body);
            }
            httpContext.Request.Body.Position = 0;
            await _next.Invoke(httpContext);
        }
    }
}