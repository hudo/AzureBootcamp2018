using BootShop.Web.API.Infrastructure;
using BootShop.Web.API.Model;
using BootShop.Web.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace BootShop.Web.API
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
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Info { Title = "Azure Bootcamp Webshop Demo", Version = "v1"});
            });

            services.AddDbContext<WebshopDbContext>(opt =>
            {
                opt.UseInMemoryDatabase("webshop");
                opt.ConfigureWarnings(warn => warn.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            
            services.AddTransient<ProcessManager>();
            services.AddTransient<PaymentClient>();
            services.AddTransient<MailerClient>();

            services.AddMvc();

            services.AddTransient<IResilientHttpClientFactory, ResilientHttpClientFactory>();
            services.AddTransient<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateClient());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddDebug()
                .AddConsole();
                
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Azure Bootcamp Webshop Demo"));
            app.UseMvc();
        }
    }
}
