using Microsoft.Azure.EventHubs;

namespace MicroserviceArchitecture.Tests
{
    public class AcceptanceTestBase
    {
        public EventHubClient CreateEventHubClient(string eventHubSendConnectionString)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubSendConnectionString);

            var client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            return client;
        }
    }
}