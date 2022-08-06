using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IDMSWebServer.Models.DataModels;
using IDMSWebServer.Models;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DBController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DBController(IConfiguration config)
        {
            this._config = config;
        }

        [HttpGet("VibrationEnergy")]
        public async Task<IActionResult> GetVibrationEnergy(string ip, DateTime from, DateTime to)
        {
            using var context = new IDMSContext(_config, ip);
            var data = context.vibration_energy.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime);

            clsQueryResult result = new clsQueryResult()
            {
                timeList = data.Select(i => i.datetime).ToList(),
                valueList = new List<clsDataValueInfo>()
                  {
                        new clsDataValueInfo
                       {
                            labelName = "X",
                            valueList = data.Select(i=>i.x).ToList(),
                            displayColor = "blue"
                       },
                       new clsDataValueInfo
                       {
                            labelName = "Y",
                             valueList = data.Select(i=>i.y).ToList(),
                              displayColor = "green"
                       },
                       new clsDataValueInfo
                       {
                            labelName = "Z",
                             valueList = data.Select(i=>i.z).ToList(),
                              displayColor = "red"
                       }
                  }
            };

            if (data.Count() > 1500)
            {
                string queryID = result.AddToCache();
                clsPreviewData preview = new clsPreviewData(data.Select(i => i.datetime).ToList(), data.Select(i => i.x).ToList());
                return Ok(new clsQueryResult
                {
                    preview = preview,
                    QueryID = queryID,
                });
            }



            return Ok(result);
        }

        [HttpGet("HealthScore")]
        public async Task<IActionResult> GetHealthScore(string ip, DateTime from, DateTime to)
        {
            using var context = new IDMSContext(_config, ip);
            var data = context.health_score.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime);

            clsQueryResult result = new clsQueryResult()
            {
                timeList = data.Select(i => i.datetime).ToList(),
                valueList = new List<clsDataValueInfo>()
                  {
                        new clsDataValueInfo
                       {
                            labelName = "HealthScore_WMA",
                            valueList = data.Select(i=>i.score_wma).ToList(),
                            displayColor = "white"
                       },
                       new clsDataValueInfo
                       {
                            labelName = "HealthScore",
                             valueList = data.Select(i=>i.score).ToList(),
                              displayColor = "blue"
                       }
                  }
            };

            if (data.Count() > 1500)
            {
                string queryID = result.AddToCache();
                clsPreviewData preview = new clsPreviewData(data.Select(i => i.datetime).ToList(), data.Select(i => i.score).ToList());
                return Ok(new clsQueryResult
                {
                    preview = preview,
                    QueryID = queryID,
                });
            }



            return Ok(result);
        }

        [HttpGet("AlertIndex")]
        public async Task<IActionResult> GetAlertIndex(string ip, DateTime from, DateTime to)
        {
            using var context = new IDMSContext(_config, ip);
            var data = context.alert_index.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime);

            clsQueryResult result = new clsQueryResult()
            {
                timeList = data.Select(i => i.datetime).ToList(),
                valueList = new List<clsDataValueInfo>()
                  {
                        new clsDataValueInfo
                       {
                            labelName = "Alert Index",
                            valueList = data.Select(i=>i.alert_index).ToList(),
                            displayColor = "orange"
                       } 
                  }
            };

            if (data.Count() > 1500)
            {
                string queryID = result.AddToCache();
                clsPreviewData preview = new clsPreviewData(data.Select(i => i.datetime).ToList(), data.Select(i => i.alert_index).ToList());
                return Ok(new clsQueryResult
                {
                    preview = preview,
                    QueryID = queryID,
                });
            }

            return Ok(result);
        }

        [HttpGet("vibration_raw_data")]
        public async Task<IActionResult> GetVibration_raw_data(string ip, DateTime from, DateTime to)
        {
            using var context = new IDMSContext(_config, ip);
            var data = context.vibration_raw_data.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime);
            //TODO設計分業機制
            //clsQueryResult result = new clsQueryResult()
            //{
            //    timeList = data.Select(i => i.datetime).ToList(),
            //    valueList = new List<clsDataValueInfo>()
            //      {
            //            new clsDataValueInfo
            //           {
            //                labelName = "Alert Index",
            //                valueList = data.Select(i=>i.alert_index).ToList(),
            //                displayColor = "orange"
            //           }
            //      }
            //};

            //if (data.Count() > 1500)
            //{
            //    string queryID = result.AddToCache();
            //    clsPreviewData preview = new clsPreviewData(data.Select(i => i.datetime).ToList(), data.Select(i => i.alert_index).ToList());
            //    return Ok(new clsQueryResult
            //    {
            //        preview = preview,
            //        QueryID = queryID,
            //    });
            //}

            return Ok(data.ToList());
        }
    }
}
