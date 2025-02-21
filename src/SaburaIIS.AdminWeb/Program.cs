using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SaburaIIS.AdminWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder =
                Host.CreateDefaultBuilder()
                    .ConfigureHostConfiguration(hostConfig =>
                    {
                        hostConfig.AddEnvironmentVariables();
                        hostConfig.AddJsonFile("appsettings.json");
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });

            hostBuilder.Build().Run();
        }
    }
}
