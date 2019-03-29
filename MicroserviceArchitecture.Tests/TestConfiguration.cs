using Microsoft.Extensions.Configuration;

namespace MicroserviceArchitecture.Tests
{
    public sealed class TestConfiguration
    {
        private static volatile IConfigurationRoot _configuration;
        private static readonly object SyncRoot = new object();

        public static IConfigurationRoot GetConfiguration()
        {
            if (_configuration == null)
            {
                lock (SyncRoot)
                {
                    if (_configuration == null)
                    {
                        void AdditionalTestSources(ConfigurationBuilder builder)
                        {
                            builder.AddJsonFile("xunit.runner.json", true);
                        }

                        _configuration = new Core.ConfigurationProvider()
                            .CreateConfiguration(AdditionalTestSources);
                    }
                }
            }

            return _configuration;
        }
    }
}
