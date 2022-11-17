using IDMSWebServer.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EdgeController : ControllerBase
    {

        public class EdgePostDataEntity
        {
            public string jsonStr { get; set; } = "";
        }


        public ILogger logger;
        private IConfiguration config;

        public EdgeController(ILogger<EdgeController> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
        }


        [HttpPost("EdgeStates/{edgeIP}")]
        public async Task EdgeStates(string edgeIP, EdgePostDataEntity entity)
        {
            Models.IDMS.DataMiddleware.Update.UpdateEdgeStates(edgeIP, entity.jsonStr);
        }


        [HttpPost("DignoseWsData/{edgeIP}")]
        public async Task DignoseWSdata(string edgeIP, EdgePostDataEntity entity)
        {

            Models.IDMS.DataMiddleware.Update.UpdateDignoseData(edgeIP, entity.jsonStr);
        }

        [HttpPost("DignoseWs_HS_ChartingData/{edgeIP}")]
        public async Task DignoseWs_HS_ChartingData(string edgeIP, EdgePostDataEntity entity)
        {
            Models.IDMS.DataMiddleware.Update.UpdateDignoseHSCharingData(edgeIP, entity.jsonStr);
        }
        [HttpPost("DignoseWs_AID_ChartingData/{edgeIP}")]
        public async Task DignoseWs_AID_ChartingData(string edgeIP, EdgePostDataEntity entity)
        {
            Models.IDMS.DataMiddleware.Update.UpdateDignoseAIDCharingData(edgeIP, entity.jsonStr);
        }
        [HttpPost("DignoseWs_AIH_ChartingData/{edgeIP}")]
        public async Task DignoseWs_AIH_ChartingData(string edgeIP, EdgePostDataEntity entity)
        {
            Models.IDMS.DataMiddleware.Update.UpdateDignoseAIHSCharingData(edgeIP, entity.jsonStr);
        }

        [HttpPost("ModuleStates/{edgeIP}")]
        public async Task ModuleStates(string edgeIP, EdgePostDataEntity entity)
        {
            Models.IDMS.DataMiddleware.Update.UpdateModuleStatesData(edgeIP, entity.jsonStr);
        }


        [HttpPost("VEData/{edgeIP}")]
        public async Task VEData(string edgeIP, EdgePostDataEntity entity)
        {
            if (edgeIP == "192.168.0.138")
            {

            }
            Models.IDMS.DataMiddleware.Update.UpdateVEDataWithCharting(edgeIP, entity.jsonStr);
        }
        [HttpPost("VEDataWithoutCharting/{edgeIP}")]
        public async Task VEDataWithoutCharting(string edgeIP, EdgePostDataEntity entity)
        {
            Models.IDMS.DataMiddleware.Update.UpdateVEDataWithoutCharting(edgeIP, entity.jsonStr);
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
