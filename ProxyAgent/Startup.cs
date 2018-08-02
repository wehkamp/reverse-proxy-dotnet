// Copyright (c) Microsoft. All rights reserved.

using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;
using ILogger = Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics.ILogger;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This is where you register dependencies, add services to the
        // container. This method is called by the runtime, before the
        // Configure method below.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions()
                .Configure<Config>(this.Configuration.GetSection("Proxy"));
            
            services.AddTransient<ILogger, Logger>(p => new Logger(Uptime.ProcessId, p.GetService<IConfig>().LogLevel));
            
            services.AddTransient<IHttpClient>(p => new HttpClient.HttpClient(p.GetService<ILogger>(), p.GetService<IConfig>()));
            
            services.AddTransient<IConfig>(p => p.GetService<IOptions<Config>>().Value);
            services.AddTransient<IProxy>(p => new Proxy(p.GetService<IHttpClient>(), p.GetService<IConfig>(), p.GetService<ILogger>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));

            // Uncomment these lines if you want to host static files in wwwroot/
            // More info: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware
            // app.UseDefaultFiles();
            // app.UseStaticFiles();

            app.UseMiddleware<ProxyMiddleware>();
        }

        private static void PrintBootstrapInfo(IContainer container)
        {
            var logger = container.Resolve<ILogger>();
            var config = container.Resolve<IConfig>();
            logger.Info("Proxy agent started", () => new { Uptime.ProcessId });
            logger.Info("Remote endpoint: " + config.Endpoint, () => { });
            logger.Info("Max payload size: " + config.MaxPayloadSize, () => { });
        }
    }
}