using Microsoft.AspNetCore.Builder;
using Serilog;

namespace MicroserviceArchitecture.BackendForFrontend.Prometheus
{
    public static class PrometheusMiddlewareExtensions
    {
        public static IApplicationBuilder UsePrometheusMiddleware(this IApplicationBuilder builder, ILogger logger)
        {
            return builder.UseMiddleware<PrometheusMiddleware>(logger);
        }
    }
}
