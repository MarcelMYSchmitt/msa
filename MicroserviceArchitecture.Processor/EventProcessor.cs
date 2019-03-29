using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceArchitecture.Processor
{
    public class EventProcessor : IEventProcessor
    {
        private const string RedisConfigString = "database:6379";
        private static ConnectionMultiplexer _connection;
        private static ProcessorMetricServer _metricServer;
        private static IConfigurationRoot _config;
        private static ILogger _logger;

        public EventProcessor(ProcessorMetricServer metricServer, IConfigurationRoot config, ILogger logger)
        {
            _metricServer = metricServer ?? throw new ArgumentNullException(nameof(metricServer));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            _logger.Debug($"[Processor shutting down. Partition {context.PartitionId}, Reason: {reason}");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            var config = ConfigurationOptions.Parse(RedisConfigString);
            config.ConnectTimeout = 20000;
            config.ConnectRetry = 9;
            _connection = ConnectionMultiplexer.Connect(config);

            _logger.Debug($"[Processor] started for partition: {context.PartitionId}");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception exception)
        {
            _logger.Error(exception, "[Processor] Error");
            Log.CloseAndFlush();
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var dict = new Dictionary<RedisKey, RedisValue>();

            foreach (var eventData in messages)
            {
                var data = eventData.Body.Array;

                if (!IsJsonFormat(data))
                {
                    _logger.Error("[Processor] received unsupported message format");
                    Log.CloseAndFlush();
                }
                else
                {
                    var dataMessage =
                        JsonConvert.DeserializeObject<ProcessorData>(Encoding.UTF8.GetString(data));

                    dict[dataMessage.Id] = JsonConvert.SerializeObject(dataMessage);

                    _logger.Debug($"[Processor] Processing message for {dataMessage.Id} ...");
                }


                var array = dict.ToArray();

                var database = _connection.GetDatabase();
                database.StringSet(array);

                _metricServer.ReportMessagesProcessed();

                await context.CheckpointAsync();
            }
        }

        private static bool IsJsonFormat(byte[] input)
        {
            try
            {
                Encoding.UTF8.GetString(input);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static DateTime UnixTimeStampToDateTime(long timestamp)
        {
            try
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error converting UnixTimestampToDateTime.");
                Log.CloseAndFlush();
                throw;
            }
        }
    }
}
