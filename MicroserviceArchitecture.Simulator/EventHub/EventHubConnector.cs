using System;
using System.Text;
using System.Threading.Tasks;
using MicroserviceArchitecture.Core;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace MicroserviceArchitecture.Simulator.EventHub
{
    public class EventHubConnector: IEventHubConnector
    {
        private static EventHubClient _eventHubClient;
        private static ILogger _logger;

        public EventHubConnector(IConfiguration config, ILogger logger)
        {
            var connectionString = config[InfrastructureConfigurationKeys.EventHubSendConnectionString];
            _logger = logger;

            _logger.Information("[Simulator] EventHub = {ConnectionString}", connectionString);

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString);
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
        }

        public async Task SendMessagesToEventHub(TestData testData)
        {
            try
            {
                var eventData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testData)));
                await _eventHubClient.SendAsync(eventData);
            }
            catch (Exception e)
            {
                _logger.Error(e, "There was an error sending messages to the eventhub.");
                Log.CloseAndFlush();
                throw;
            }
        
        }
    }
}