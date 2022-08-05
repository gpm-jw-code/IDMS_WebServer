using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IDMSWebServer.Models;
using Npgsql;
using System.Data;
using System.Diagnostics;
using IDMSWebServer.Database;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        IConfiguration _config;
        public QueryController(IConfiguration config)
        {
            _config = config;

        }

        [HttpGet("HealthScore")]
        public async Task<IActionResult> HealthScore(string ip, DateTime from, DateTime to)
        {

            IDMSPostgresHepler IDMSDBHepler = new IDMSPostgresHepler(_config);
            clsQueryResult result = IDMSDBHepler.HealthScoreQuery(ip, from, to);
            return Ok(result);
        }


        /// <summary>
        /// 查詢切片檔案
        /// </summary>
        /// <param name="queryID"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet("HealthScoreSplice")]
        public async Task<IActionResult> HealthScoreSplice(string queryID, DateTime from, DateTime to)
        {
            QueryResultManager.TryGetData(queryID, from, to, out clsQueryResult spliceDataRet);
            return Ok(spliceDataRet);
        }


        [HttpGet("AlertIndex")]
        public async Task<IActionResult> AlertIndex(string ip, DateTime from, DateTime to)
        {
            IDMSPostgresHepler IDMSDBHepler = new IDMSPostgresHepler(_config);
            clsQueryResult result = IDMSDBHepler.AlertIndexeQuery(ip, from, to);
            return Ok(result);
        }
        [HttpGet("Raw")]
        public async Task<IActionResult> Raw(string ip, DateTime from, DateTime to)
        {

            return Ok(ip);
        }

        [HttpGet("ModuleInfos")]
        public async Task<IActionResult> ModuleInfos()
        {
            var helper = new IDMSPostgresHepler(_config);
            return Ok(helper.ModuleInfosQuery());
        }

    }
}
