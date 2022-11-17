using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrontendController : ControllerBase
    {
        private readonly IConfiguration _config;

        public FrontendController(IConfiguration config)
        {
            this._config = config;
        }
        [HttpGet("CustomCOMPImage")]
        public async Task<IActionResult> GetCustomCOMPImage()
        {
            string imageFile = Path.Combine(Environment.CurrentDirectory,"Assets/image.jpg");
            if(!System.IO.File.Exists(imageFile))
                return NotFound();
            System.IO.File.Copy(imageFile,Path.Combine(Environment.CurrentDirectory,"wwwroot/img/image.jpg"),true);
            return Ok();
        }

        [HttpGet("LogoText")]
        public async Task<string> LogoText()
        {
            return (_config.GetSection("Website").GetValue("LogoText", ""));
        }

    }
}
