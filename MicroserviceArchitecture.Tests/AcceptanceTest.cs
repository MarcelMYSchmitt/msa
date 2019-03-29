using FluentAssertions;
using MicroserviceArchitecture.Core;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MicroserviceArchitecture.Tests
{
    public class AcceptanceTest : AcceptanceTestBase
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly BackendService _backendService;

        public AcceptanceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
            _configuration = TestConfiguration.GetConfiguration();
            _backendService = new BackendService(_configuration, _testOutputHelper);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldGetDataFromBff_WhenSendingDataToEventHub()
        {
            // Arrange
            var testData = await SetupData();
            var encodedTestMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testData));
            var eventHubClient = CreateEventHubClient(_configuration[InfrastructureConfigurationKeys.EventHubSendConnectionString]);

            //act
            _testOutputHelper.WriteLine($"I am sending ID {testData.Id} to the event hub.");
            await eventHubClient.SendAsync(new EventData(encodedTestMessage));

            // Assert
            var currentAttempt = 0;
            var maxAttempts = 10;
            var delayBetweenAttemptsInSeconds = 2;
            var dataFound = false;

            while (currentAttempt < maxAttempts)
            {
                currentAttempt++;

                _testOutputHelper.WriteLine($"Attempt {currentAttempt}");

                var httpResponse = await _backendService.GetData(testData.Id);
                var stringResponse = await httpResponse.Content.ReadAsStringAsync();

                if (!dataFound)
                {
                    dataFound = stringResponse.Contains(testData.Id) && stringResponse.Contains(testData.Value);
                }

                if (dataFound)
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(delayBetweenAttemptsInSeconds));
            }

            dataFound.Should().BeTrue("Should be found on bff endpoint");

        }

        private async Task<TestData> SetupData()
        {
            var testData = new TestData
            {
                Id = "123",
                Value = "some value"
            };

            return testData;
        }
    }

    public class TestData
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}