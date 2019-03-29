using System;
using System.Text;
using MicroserviceArchitecture.Simulator.Data;
using Newtonsoft.Json;

namespace MicroserviceArchitecture.Simulator.EventHub
{
    public class JsonMessageCreator : IMessageCreator
    {
        public byte[] CreateByteMessages(TestData data)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }
    }
}
