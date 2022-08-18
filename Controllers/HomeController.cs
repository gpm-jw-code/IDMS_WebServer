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
            return ReturnContent();
        }
        [HttpGet("/EdgeMain/{edge_ip}")]
        public async Task<ContentResult> EdgeMain(string edge_ip)
        {
            return ReturnContent();
        }


        [HttpGet("/VibrationEnergy/{edge_ip}")]
        public async Task<ContentResult> VibrationEnergy(string edge_ip)
        {

            return ReturnContent();
        }


        [HttpGet("/modulestates/{edge_ip}")]
        public async Task<ContentResult> modulestates(string edge_ip)
        {

            return ReturnContent();
        }

        [HttpGet("/query")]
        public async Task<ContentResult> Qurey()
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
