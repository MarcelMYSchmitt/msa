using MicroserviceArchitecture.Simulator.Data;

namespace MicroserviceArchitecture.Simulator.EventHub
{
    public interface IMessageCreator
    {
        byte[] CreateByteMessages(TestData data);
    }
}