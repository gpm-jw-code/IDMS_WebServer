using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace IDMSWebServer.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/Admin")]
        public async Task<ContentResult> get()
        {
            var html = System.IO.File.ReadAllText("wwwroot/admin.html");
            //var html = System.IO.File.ReadAllText("htmlpage.html");
            return base.Content(html, "text/html");
        }
        [HttpGet("/EdgeMain")]
        public async Task<ContentResult> EdgeMain(string? ip, string? edgename)
        {
            return ReturnContent();
        }


        [HttpGet("/ve")]
        public async Task<ContentResult> VibrationEnergy(string ip, string edgename)
        {

            return ReturnContent();
        }


        [HttpGet("/modulestates")]
        public async Task<ContentResult> modulestates(string? ip, string? edgename)
        {

            return ReturnContent();
        }

        [HttpGet("/query")]
        public async Task<ContentResult> Qurey(string? ip, string? edgename)
        {
            return ReturnContent();
        }

        [HttpGet("/chating")]
        public async Task<ContentResult> chating(string? ip, string? edgename)
        {
            return ReturnContent();
        }

        private ContentResult ReturnContent()
        {
            var html = System.IO.File.ReadAllText("wwwroot/index.html");
            //var html = System.IO.File.ReadAllText("htmlpage.html");
            return base.Content(html, "text/html");
        }
    }
}
