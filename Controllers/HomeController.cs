using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDMSWebServer.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public async Task<ContentResult> get()
        {
            var html = System.IO.File.ReadAllText("wwwroot/index.html");
            return base.Content(html, "text/html");
        }

        [HttpGet("/VibrationEnergy/{edge_ip}/Dashboard")]
        public async Task<ContentResult> VibrationEnergy(string edge_ip)
        {
            var html = System.IO.File.ReadAllText("wwwroot/index.html");
            //var html = System.IO.File.ReadAllText("htmlpage.html");
            return base.Content(html, "text/html");
        }
    }
}
