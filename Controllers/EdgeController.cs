using IDMSWebServer.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EdgeController : ControllerBase
    {
        public ILogger logger;
        private IConfiguration config;

        public EdgeController(ILogger<EdgeController> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
        }


        [HttpPost("EdgeStates/{edgeIP}")]
        public async Task EdgeStates(string edgeIP, [FromBody] string jsonStr)
        {
            Models.IDMS.DataMiddleware.Update.UpdateEdgeStates(edgeIP, jsonStr);
        }


        [HttpPost("DignoseWsData/{edgeIP}")]
        public async Task DignoseWSdata(string edgeIP, [FromBody] string jsonStr)
        {
            Models.IDMS.DataMiddleware.Update.UpdateDignoseData(edgeIP, jsonStr);
        }

        [HttpPost("DignoseWs_HS_ChartingData/{edgeIP}")]
        public async Task DignoseWs_HS_ChartingData(string edgeIP, [FromBody] string jsonStr)
        {
            Models.IDMS.DataMiddleware.Update.UpdateDignoseHSCharingData(edgeIP, jsonStr);
        }
        [HttpPost("DignoseWs_AID_ChartingData/{edgeIP}")]
        public async Task DignoseWs_AID_ChartingData(string edgeIP, [FromBody] string jsonStr)
        {
            Models.IDMS.DataMiddleware.Update.UpdateDignoseAIDCharingData(edgeIP, jsonStr);
        }
        [HttpPost("DignoseWs_AIH_ChartingData/{edgeIP}")]
        public async Task DignoseWs_AIH_ChartingData(string edgeIP, [FromBody] string jsonStr)
        {
            Models.IDMS.DataMiddleware.Update.UpdateDignoseAIHSCharingData(edgeIP, jsonStr);
        }

        [HttpPost("ModuleStates/{edgeIP}")]
        public async Task ModuleStates(string edgeIP, [FromBody] string jsonstr)
        {
            Models.IDMS.DataMiddleware.Update.UpdateModuleStatesData(edgeIP, jsonstr);
        }


        [HttpPost("VEData/{edgeIP}")]
        public async Task VEData(string edgeIP, [FromBody] string jsonstr)
        {
            if (edgeIP == "192.168.0.138")
            {

            }
            Models.IDMS.DataMiddleware.Update.UpdateVEDataWithCharting(edgeIP, jsonstr);
        }
        [HttpPost("VEDataWithoutCharting/{edgeIP}")]
        public async Task VEDataWithoutCharting(string edgeIP, [FromBody] string jsonstr)
        {
            Models.IDMS.DataMiddleware.Update.UpdateVEDataWithoutCharting(edgeIP, jsonstr);
        }

        [HttpGet("EdgesWSDataState")]
        public async Task<IActionResult> GetEdgesWSDataState(bool? withData)
        {
            if (withData == null)
                withData = false;

            if ((bool)withData)
            {
                return Ok(Models.IDMS.DataMiddleware.EdgeDatas);
            }
            else
            {
                return Ok(Models.IDMS.DataMiddleware.EdgeDatasWithoutData);
                ;
            }
        }
        [HttpGet("edgename")]
        public async Task<IActionResult> GetEdgeName(string ip)
        {
            DBController dbcontroller = new DBController(config, null);
            var edges = await dbcontroller.GetEdgesInformation();
            var edge = edges.FirstOrDefault(edge => edge.EdgeIP == ip);
            return Ok(edge == null ? "" : edge.Name);
        }
    }
}
