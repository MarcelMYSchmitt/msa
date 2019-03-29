using System.Threading.Tasks;
namespace MicroserviceArchitecture.Simulator.EventHub
{
    public interface IEventHubConnector
    {
        Task SendMessagesToEventHub(TestData testData);
    }
}