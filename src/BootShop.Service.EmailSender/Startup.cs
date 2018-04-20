using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BootShop.Service.EmailSender
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddTransient<ISendGridClient, ChaosMonkeySendGridClient>();

            services.AddSingleton<IHostedService, NewOrdersReceiverService>();

            services.AddSingleton<TelemetryConfiguration>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, TelemetryConfiguration telemetry)
        {
            telemetry.DisableTelemetry = true;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (ctx, next) =>
            {
                Console.WriteLine($"Request: {ctx.Request.Method} {ctx.Request.Path}");
                await next();
            });

            app.Use(async (ctx, next) => { await ctx.Response.WriteAsync("Mailer service up & running"); });
        }
    }
}
