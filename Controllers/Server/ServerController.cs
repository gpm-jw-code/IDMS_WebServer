using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDMSWebServer.Controllers.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {


        [HttpGet]
        public async Task<string> Host()
        {
            return HttpContext.Request.Host.ToString();
        }
    }
}
