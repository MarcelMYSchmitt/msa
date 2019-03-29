using MicroserviceArchitecture.Simulator.EventHub;
using Newtonsoft.Json;
using NSubstitute;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MicroserviceArchitecture.Simulator.Tests
{
    public class SimulatorTests
    {
        private readonly IEventHubConnector _eventHubConnector;
        private static ILogger _logger;

        public SimulatorTests()
        {
            _eventHubConnector = Substitute.For<IEventHubConnector>();
            _logger = new LoggerConfiguration().CreateLogger();
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldSimulateTestData()
        {
            // Arrange
            var testDataList = new List<TestData>
            {
                new TestData() { Id = "1", Value = "a"},
                new TestData() { Id = "2", Value = "b"}
            };

            var simulator = CreateSimulatorForNumberOfIterations(3);
            
            // Act
            await simulator.Simulate(testDataList);

            // Assert
            await _eventHubConnector.Received(6).SendMessagesToEventHub(Arg.Is<TestData>(x => IsValidMessage(x)));
        }


        private DataSimulator CreateSimulatorForNumberOfIterations(int numberOfIterationsToExecute)
        {
            var executionEngine = new DeterministicExecutionEngine(numberOfIterationsToExecute);
            var simulator = new DataSimulator(_eventHubConnector, executionEngine, _logger);
            return simulator;
        }

        private static bool IsValidMessage(TestData newMessage)
        {
            try
            {
                return newMessage.Id != null && newMessage.Value != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
