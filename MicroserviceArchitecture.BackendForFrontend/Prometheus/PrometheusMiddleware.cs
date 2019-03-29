using Microsoft.AspNetCore.Http;
using Prometheus;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MicroserviceArchitecture.BackendForFrontend.Prometheus
{
    public class PrometheusMiddleware
    {
        private readonly RequestDelegate _next;
        private static ILogger _logger;

        public PrometheusMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path;
            var method = httpContext.Request.Method;

            var counter = Metrics.CreateCounter(BffMetrics.TotalRequestsCounter,
                "HTTP Requests Total", "path", "method", "status");
            int statusCode;

            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                statusCode = 500;
                _logger.Error(ex, "PrometheusMiddleware: [ERROR] incrementing once for {Path}, {Method},{StatusCode}", path, method, statusCode);
                counter.Labels(path, method, statusCode.ToString()).Inc();

                throw;
            }

            if (path != "/metrics")
            {
                statusCode = httpContext.Response.StatusCode;
                _logger.Information("PrometheusMiddleware: incrementing once for {Path}, {Method}, {StatusCode}", path, method, statusCode);
                counter.Labels(path, method, statusCode.ToString()).Inc();
            }
        }
    }
}
