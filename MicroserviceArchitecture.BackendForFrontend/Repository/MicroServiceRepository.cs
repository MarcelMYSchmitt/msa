using StackExchange.Redis;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MicroserviceArchitecture.BackendForFrontend.Repository
{
    public class MicroServiceRepository : IMicroServiceRepository
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public MicroServiceRepository(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        public async Task<BffData> GetData(string id)
        {
            var redis = _redisConnection.GetDatabase();
            var json = redis.StringGet(id);
            if (json.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<BffData>(json);
        }
    }


    public interface IMicroServiceRepository
    {
        Task<BffData> GetData(string id);
    }
}
