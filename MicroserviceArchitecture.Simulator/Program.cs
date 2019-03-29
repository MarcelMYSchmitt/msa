using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MicroserviceArchitecture.Core;
using MicroserviceArchitecture.Simulator.EventHub;

namespace MicroserviceArchitecture.Simulator
{
    public class Program
    {
        private static DataSimulator _dataSimulator;

        public static void Main(string[] args)
        {
            var config = new ConfigurationProvider().CreateConfiguration();

            var logEventLevelSwitch =
                new LoggingProvider().ConvertStringToLoggingLevelSwitch(
                    config[ApplicationConfigurationKeys.LogEventLevel]);

            var logger = new LoggingProvider().CreateLogger(config, logEventLevelSwitch,
                Assembly.GetEntryAssembly().GetName().Version.ToString(),
                Assembly.GetEntryAssembly().GetName().ToString());

            _dataSimulator = new DataSimulator(new EventHubConnector(config, logger), logger);
            logger.Information($"[Simulator] starting for {config[InfrastructureConfigurationKeys.EnvironmentTag]} on region {config[InfrastructureConfigurationKeys.AzureRegionTag]}");

            StartSimulation().Wait();
        }

        private static async Task StartSimulation()
        {
            new Thread(StartSimulateData).Start();
        }
        private static void StartSimulateData()
        {
            var testDataList = new List<TestData>();
            for (int i = 1; i <= 10; i++)
            {
                var id = i; 
                var testData = new TestData { Id = id.ToString(), Value = "test1" };
                testDataList.Add(testData);
            }

            //_dataSimulator.Simulate(testDataList).Wait();
        }
    }


    public class TestData
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
