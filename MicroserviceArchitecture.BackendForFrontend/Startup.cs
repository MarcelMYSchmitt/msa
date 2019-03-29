using MicroserviceArchitecture.BackendForFrontend.Prometheus;
using MicroserviceArchitecture.BackendForFrontend.Repository;
using MicroserviceArchitecture.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Prometheus;
using Serilog;
using StackExchange.Redis;

namespace MicroserviceArchitecture.BackendForFrontend
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; private set; }
        public static ILogger Logger;

        // Configure (add) services to the IoC container here, order does not matter
        public void ConfigureServices(IServiceCollection services)
        {
            var redisConfig = ConfigurationOptions.Parse("database:6379");
            redisConfig.ConnectTimeout = 15000;
            redisConfig.ConnectRetry = 3;
            var redisConnection = ConnectionMultiplexer.Connect(redisConfig);
            services.AddSingleton<IConnectionMultiplexer>(redisConnection);

            var config = new Core.ConfigurationProvider().CreateConfiguration();
            services.AddSingleton(config);
            Configuration = config;

            var logEventLevelSwitch = new LoggingProvider().ConvertStringToLoggingLevelSwitch(config[ApplicationConfigurationKeys.LogEventLevel]);

            var logger = new LoggingProvider().CreateLogger(config, logEventLevelSwitch,
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().ToString());

            services.AddSingleton<ILogger>(logger);
            Logger = logger;

            services.AddCors();

            var prometheusHttpFilter = new PrometheusHttpFilter(Logger);
            services.AddSingleton(prometheusHttpFilter);

            var microServiceRepository = new MicroServiceRepository(redisConnection);
            services.AddSingleton(microServiceRepository);
            var microServiceService = new MicroServiceService(microServiceRepository);
            services.AddSingleton(microServiceService);


            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }


        // Use this method to configure the HTTP request pipeline (middleware).
        public void Configure(IApplicationBuilder app)
        {
            if (Configuration[InfrastructureConfigurationKeys.EnvironmentTag] != "prod")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }


            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            // new metricServer with seperate port for prometheus
            var metricServer = new KestrelMetricServer(8081);
            metricServer.Start();

            app.UsePrometheusMiddleware(Logger);

            app.UseMvc();
        }
    }
}
