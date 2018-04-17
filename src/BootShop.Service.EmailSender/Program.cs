using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
                .UseStartup<Startup>()
                .UseUrls("http://+:5020")
                .Build();
    }
}
