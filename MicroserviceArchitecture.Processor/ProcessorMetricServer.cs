using Prometheus;
using Serilog;
using System;

namespace MicroserviceArchitecture.Processor
{
    public class ProcessorMetricServer : IDisposable
    {
        private readonly MetricServer _metricServer;
        private readonly Counter _counter;
        private static ILogger _logger;

        public ProcessorMetricServer(ILogger logger)
        {
            _logger = logger;
            _metricServer = new MetricServer(port: 9201);
            _counter = Metrics.CreateCounter("processor_messagecounter", "counting accumulative all messages sent to redis and calculate messages processed by a second");

            _metricServer.Start();
            _logger.Debug($"[Processor at {DateTime.Now}] Metric server started");
        }

        public void Dispose()
        {
            _metricServer.Stop();
        }

        public void ReportMessagesProcessed()
        {
            int amount = 1;
            _counter.Inc(amount);
            _logger.Debug($"[Processor] Incremented message counter by {amount}");
        }
    }
}
