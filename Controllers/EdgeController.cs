﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EdgeController : ControllerBase
    {
        public ILogger logger;
        public EdgeController(ILogger<EdgeController> logger)
        {
            this.logger = logger;
        }



        [HttpPost("DignoseWsData/{edgeIP}")]
        public async Task DignoseWSdata(string edgeIP, [FromBody] string jsonStr)
        {
            logger.LogInformation("Edge:{0},Dignose Data Recieved", edgeIP);
            Models.IDMS.DataMiddleware.Update.UpdateDignoseData(edgeIP, jsonStr);
        }

        [HttpPost("DignoseWs_HS_ChartingData/{edgeIP}")]
        public async Task DignoseWs_HS_ChartingData(string edgeIP, [FromBody] string jsonStr)
        {
            logger.LogInformation("Edge:{0},Dignose Data Recieved", edgeIP);
            Models.IDMS.DataMiddleware.Update.UpdateDignoseHSCharingData(edgeIP, jsonStr);
        }
        [HttpPost("DignoseWs_AID_ChartingData/{edgeIP}")]
        public async Task DignoseWs_AID_ChartingData(string edgeIP, [FromBody] string jsonStr)
        {
            logger.LogInformation("Edge:{0},Dignose Data Recieved", edgeIP);
            Models.IDMS.DataMiddleware.Update.UpdateDignoseAIDCharingData(edgeIP, jsonStr);
        }
        [HttpPost("DignoseWs_AIH_ChartingData/{edgeIP}")]
        public async Task DignoseWs_AIH_ChartingData(string edgeIP, [FromBody] string jsonStr)
        {
            logger.LogInformation("Edge:{0},Dignose Data Recieved", edgeIP);
            Models.IDMS.DataMiddleware.Update.UpdateDignoseAIHSCharingData(edgeIP, jsonStr);
        }

        [HttpPost("ModuleStates/{edgeIP}")]
        public async Task ModuleStates(string edgeIP, [FromBody] string jsonstr)
        {
            Models.IDMS.DataMiddleware.Update.UpdateModuleStatesData(edgeIP, jsonstr);
        }
    }
}