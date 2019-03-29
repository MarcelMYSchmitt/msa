using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace MicroserviceArchitecture.Processor
{
    public class EventProcessorFactory : IEventProcessorFactory
    {
        private static ProcessorMetricServer _metricServer;
        private static IConfigurationRoot _config;
        private static ILogger _logger;

        public EventProcessorFactory(ProcessorMetricServer metricServer, IConfigurationRoot config, ILogger logger)
        {
            _metricServer = metricServer ?? throw new ArgumentNullException(nameof(metricServer));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger;
        }

        IEventProcessor IEventProcessorFactory.CreateEventProcessor(PartitionContext context)
        {
            return new EventProcessor(_metricServer, _config, _logger);
        }
    }
}
