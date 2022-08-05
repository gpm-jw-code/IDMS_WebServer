using IDMSWebServer.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostgrelDBController : ControllerBase
    {

        IConfiguration _config;
        public PostgrelDBController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("Databases")]
        public async Task<IActionResult> PostgrelDB()
        {
            PostgresHepler postgresHepler = new PostgresHepler(_config);
            return Ok(postgresHepler.GetDatabases());
        }
    }
}
