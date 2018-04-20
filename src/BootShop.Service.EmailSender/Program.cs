using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace BootShop.Service.EmailSender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseShutdownTimeout(TimeSpan.FromSeconds(5))
                .ConfigureLogging((ctx, logger) =>
                {
                    logger.ClearProviders();
                    logger.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    logger.AddConsole();
                })
                .UseStartup<Startup>()
                .UseUrls("http://+:5020")
                .Build();
    }
}
