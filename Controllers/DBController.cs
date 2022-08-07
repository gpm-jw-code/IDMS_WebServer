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
            List<DateTime>? timeList = data.Select(i => i.datetime).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel();
            chartData.labels = timeList;
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "X",
                data = data.Select(d => d.x).ToList(),
                borderColor = "blue",
            });
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Y",
                data = data.Select(d => d.y).ToList(),
                borderColor = "green",
            });

            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Z",
                data = data.Select(d => d.z).ToList(),
                borderColor = "red",
            });
            if (data.Count() > 1500)
            {
                string queryID = chartData.AddToCache();
                clsPreviewData preview = new clsPreviewData(timeList, chartData.datasets[0].data);
                ViewModels.ChartingViewModel previewChartData = PreviewVMBuilder(queryID, "振動能量", chartData.labels, data.Select(d => (object)d).ToList(), "x", ymax: chartData.ymax, ymin: chartData.ymin);

                return Ok(previewChartData);
            }


            return Ok(chartData);
        }

        [HttpGet("HealthScore")]
        public async Task<IActionResult> GetHealthScore(string ip, DateTime from, DateTime to)
        {
            using var context = new IDMSContext(_config, ip);
            IOrderedQueryable<Health_Score> data = context.health_score.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime);
            List<DateTime>? timeList = data.Select(i => i.datetime).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel()
            {
                ymax = 100,
                ymin = 0
            };
            chartData.labels = timeList;
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Score_WMA",
                data = data.Select(d => d.score_wma).ToList(),
                borderColor = "white",
            });
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Score",
                data = data.Select(d => d.score).ToList(),
                borderColor = "blue",
            });

            if (data.Count() > 1500)
            {
                string queryID = chartData.AddToCache();

                clsPreviewData preview = new clsPreviewData(timeList, chartData.datasets[1].data);
                ViewModels.ChartingViewModel previewChartData = PreviewVMBuilder(queryID, "Score", chartData.labels, data.Select(d => (object)d).ToList(), "score", fillMethod: "false", ymax: chartData.ymax, ymin: chartData.ymin);
                return Ok(previewChartData);
            }


            return Ok(chartData);
            //using var context = new IDMSContext(_config, ip);
            //var data = context.health_score.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime);

            //clsQueryResult result = new clsQueryResult()
            //{
            //    timeList = data.Select(i => i.datetime).ToList(),
            //    valueList = new List<clsDataValueInfo>()
            //      {
            //            new clsDataValueInfo
            //           {
            //                labelName = "HealthScore_WMA",
            //                valueList = data.Select(i=>i.score_wma).ToList(),
            //                displayColor = "white"
            //           },
            //           new clsDataValueInfo
            //           {
            //                labelName = "HealthScore",
            //                 valueList = data.Select(i=>i.score).ToList(),
            //                  displayColor = "blue"
            //           }
            //      }
            //};

            //if (data.Count() > 1500)
            //{
            //    string queryID = result.AddToCache();
            //    clsPreviewData preview = new clsPreviewData(data.Select(i => i.datetime).ToList(), data.Select(i => i.score).ToList());
            //    return Ok(new clsQueryResult
            //    {
            //        preview = preview,
            //        QueryID = queryID,
            //    });
            //}



            //return Ok(result);
        }

        [HttpGet("AlertIndex")]
        public async Task<IActionResult> GetAlertIndex(string ip, DateTime from, DateTime to)
        {
            using var context = new IDMSContext(_config, ip);
            IOrderedQueryable<Alert_Index>? data = context.alert_index.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime);
            List<DateTime>? timeList = data.Select(i => i.datetime).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel()
            {
                ymax = 0,
                ymin = 100,

            };
            chartData.labels = timeList;
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Alert Index",
                data = data.Select(d => d.alert_index).ToList(),
                borderColor = "orange",
                fill="false"
            });

            if (data.Count() > 1500)
            {
                string queryID = chartData.AddToCache();
                clsPreviewData preview = new clsPreviewData(timeList, data.Select(i => i.alert_index).ToList());
                ViewModels.ChartingViewModel previewChartData = PreviewVMBuilder(queryID, "Alert Index", chartData.labels, data.Select(d => (object)d).ToList(), "alert_index", fillMethod: "false", ymax: chartData.ymax, ymin:chartData.ymin) ;
                return Ok(previewChartData);
            }


            return Ok(chartData);
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


        private ViewModels.ChartingViewModel PreviewVMBuilder(string queryID, string label, List<DateTime> dataTimesSource,
            List<object> dataSource, string propertyName, string color = "orange", string fillMethod = "false",double ymax=-1,double ymin=-1)
        {
            clsPreviewData preview = new clsPreviewData(dataTimesSource, dataSource.Select(i => i.GetType().GetProperty(propertyName).GetValue(i)).Select(d => (double)d).ToList());
            ViewModels.ChartingViewModel previewChartData = new ViewModels.ChartingViewModel()
            {
                 ymin = ymin,
                 ymax = ymax,
            };
            previewChartData.isPreview = true;
            previewChartData.QueryID = queryID;
            previewChartData.labels = preview.TimeLs;
            previewChartData.datasets.Add(new ViewModels.DataSet
            {
                label = label,
                data = preview.DataLs,
                borderColor = color,
                fill = fillMethod,
                 
            });

            return previewChartData;
        }

    }
}
