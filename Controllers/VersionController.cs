using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly IConfiguration _config;

        public VersionController(IConfiguration config)
        {
            this._config = config;
        }

        [HttpGet("WebsiteVersion")]
        public async Task<IActionResult> WebSiteVersion()
        {
            return Ok(_config.GetSection("Website").GetValue("Version", ""));
        }
    }
}
