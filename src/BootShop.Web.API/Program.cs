using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace BootShop.Web.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((ctx, logger) =>
                {
                    logger.ClearProviders();
                    logger.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    logger.AddConsole();
                })
                .UseStartup<Startup>()
                .UseUrls("http://+:5000")
                .Build();
    }
}
