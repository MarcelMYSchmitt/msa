using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Serilog;
using MicroserviceArchitecture.BackendForFrontend.Repository;

namespace MicroserviceArchitecture.BackendForFrontend.Controllers
{
    [Route("api/[controller]")]
    public class BffController : Controller
    {
        private static ILogger _logger;
        private static MicroServiceService _microServiceService;

        public BffController(ILogger logger, MicroServiceService microServiceService)
        {
            _logger = logger;
            _microServiceService = microServiceService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetId([FromRoute] string Id)
        {
            _logger.Information($"GetId for id: {Id}");

            var data = await _microServiceService.GetData(Id);
            
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }
    }
}
