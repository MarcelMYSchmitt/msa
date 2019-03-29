using MicroserviceArchitecture.Core;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System.Threading;
using System.Threading.Tasks;

namespace MicroserviceArchitecture.Processor
{
    public class EventHubConnector
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var config = new ConfigurationProvider().CreateConfiguration();

            var logEventLevelSwitch =
                new LoggingProvider().ConvertStringToLoggingLevelSwitch(
                    config[ApplicationConfigurationKeys.LogEventLevel]);

            var logger = new LoggingProvider().CreateLogger(config, logEventLevelSwitch,
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().ToString());

            var environmentTag = config[InfrastructureConfigurationKeys.EnvironmentTag];
            var azureRegionTag = config[InfrastructureConfigurationKeys.AzureRegionTag];

            logger.Information($"[Processor] starting for {environmentTag} environment on region {azureRegionTag}");

            var storageAccountName = config[InfrastructureConfigurationKeys.StorageAccountName];
            var storageAccessKey = config[InfrastructureConfigurationKeys.StorageAccessKey];
            var storageConnectionString = string.Format(
                "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                storageAccountName, storageAccessKey);

            const string leaseContainerName = "hostsync";

            
            var eventHubEndpoint = config[InfrastructureConfigurationKeys.EventHubEndpoint];
            var eventHubPath = config[InfrastructureConfigurationKeys.EventHubPath];
            var eventHubListenConnectionString = config[InfrastructureConfigurationKeys.EventHubListenConnectionString];

            var eventProcessorHost = new EventProcessorHost(
                eventHubPath,
                PartitionReceiver.DefaultConsumerGroupName,
                eventHubListenConnectionString,
                storageConnectionString,
                leaseContainerName
            );

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorFactoryAsync
                (new EventProcessorFactory(new ProcessorMetricServer(logger), config, logger));
            logger.Information($"[Processor] Listening to EventHub at {eventHubPath}");

            await Task.Delay(Timeout.Infinite);
        }

    }
}
