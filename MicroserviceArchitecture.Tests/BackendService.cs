using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MicroserviceArchitecture.Tests
{
    public class BackendService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _baseUrlForBackendForFrontend;

        public BackendService(IConfigurationRoot configuration, ITestOutputHelper testOutputHelper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
            _baseUrlForBackendForFrontend = _configuration[TestConfigurationKeys.BackendForFrontendBaseUrl];
        }


        public async Task<HttpResponseMessage> GetData(string id)
        {
            var baseUrl = $"{_baseUrlForBackendForFrontend}/api/bff";
            var query = $"{baseUrl}/{id}";

            _testOutputHelper.WriteLine($"--- Performing id get on {query}");

            var httpClient = new HttpClient();
            return await httpClient.GetAsync(query);
        }
    }
}