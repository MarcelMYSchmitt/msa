using System.Threading.Tasks;

namespace MicroserviceArchitecture.BackendForFrontend.Repository
{
    public class MicroServiceService
    {
        private readonly MicroServiceRepository _microServiceRepository;

        public MicroServiceService(MicroServiceRepository microServiceRepository)
        {
            _microServiceRepository = microServiceRepository;
        }

        public async Task<BffData> GetData(string id)
        {
            return await _microServiceRepository.GetData(id);
        }
    }
}
