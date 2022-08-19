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

        [HttpGet("ReleaseNote")]
        public async Task<ContentResult> ReleaseNote()
        {
            var md = System.IO.File.ReadAllText("wwwroot/release note.md");
            //var html = System.IO.File.ReadAllText("htmlpage.html");
            return base.Content(md, "text/markdown");
        }
        
    }
}
