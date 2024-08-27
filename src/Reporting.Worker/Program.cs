using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Reporting.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Diagnostics.Activity.DefaultIdFormat = System.Diagnostics.ActivityIdFormat.W3C;
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}