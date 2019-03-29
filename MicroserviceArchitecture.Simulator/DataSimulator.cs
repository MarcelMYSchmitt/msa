using System;
using Serilog;
using MicroserviceArchitecture.Simulator.EventHub;
using MicroserviceArchitecture.Simulator.ExecutionEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MicroserviceArchitecture.Simulator
{
    public class DataSimulator
    {
        private static IEventHubConnector _eventHubConnector;
        private static IExecutionEngine _executionEngine;
        private static ILogger _logger;

        public DataSimulator(IEventHubConnector eventHubConnector, IExecutionEngine executionEngine, ILogger logger)
        {
            _eventHubConnector = eventHubConnector ?? throw new ArgumentNullException(nameof(eventHubConnector));
            _executionEngine = executionEngine ?? throw new ArgumentNullException(nameof(executionEngine));
            _logger = logger;
        }

        public DataSimulator(IEventHubConnector eventHubConnector, ILogger loggingProvider): this(eventHubConnector, new InfiniteRepeatExecutionEngine(), loggingProvider)
        {
        }

        public async Task Simulate(ICollection<TestData> testdataList)
        {
            var jsonMessageCreator = new JsonMessageCreator();

            async Task Body()
            {
                foreach(var testData in testdataList)
                {
                    _eventHubConnector.SendMessagesToEventHub(testData).GetAwaiter().GetResult();
                    _logger.Debug($"[Simulator] Start sending messages: {testData.ToString()}");
                }
            };
            
            _logger.Debug("[Simulator] Start sending messages to the EventHub.");
            await _executionEngine.Execute(Body);
            _logger.Debug("[Simulator] End sending messages to the EventHub.");
        }
    }
}