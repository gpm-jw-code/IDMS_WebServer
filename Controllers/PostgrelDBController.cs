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
        public async Task<List<string>> GetDatabases()
        {
            PostgresHepler postgresHepler = new PostgresHepler(_config);
            return postgresHepler.GetDatabases().ToList();
        }


        [HttpGet("GetEdges")]
        public async Task<IActionResult> GetEdges()
        {
            PostgresHepler postgresHepler = new PostgresHepler(_config);
            var dbs = postgresHepler.GetDatabases();

            //foreach (var db in dbs)
            //{
            //    postgresHepler.TryGetSchemasFromDB()
            //}
            //postgresHepler.TryGetSchemasFromDB(dbs.ToList()[2]);
            ////return Ok(await postgresHepler.GetSchemasFromDB(""));
            return Ok();
        }
    }
}
