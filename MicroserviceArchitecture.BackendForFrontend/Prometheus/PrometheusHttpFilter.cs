using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Prometheus;
using Serilog;


namespace MicroserviceArchitecture.BackendForFrontend.Prometheus
{
    public class PrometheusHttpFilter : IAsyncActionFilter
    {
        private static ILogger _logger;

        public PrometheusHttpFilter(ILogger logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var controller = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
            var action = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;

            var counter = Metrics.CreateCounter(BffMetrics.TotalRequestsCounter,
                "Backend for Frontend HTTP Requests Total",
                "controller", "action", "method", "status");

            await next();

            context.HttpContext.Features.Get<IExceptionHandlerFeature>();

            var statusCode = context.HttpContext.Response.StatusCode;

            _logger.Information("PrometheusHttpFilter: incrementing once for {Controller}, {Action}, {Method}, {StatusCode}", controller, action, method, statusCode);
            counter.Labels(controller, action, method, statusCode.ToString()).Inc();
        }
    }
}
