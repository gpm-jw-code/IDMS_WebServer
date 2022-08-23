using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IDMSWebServer.Models.DataModels;
using IDMSWebServer.Models;
using System.Diagnostics;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DBController : ControllerBase
    {
        private readonly IConfiguration _config;
        private ILogger _logger;
        public DBController(IConfiguration config, ILogger<DBController> loger)
        {
            this._config = config;
            _logger = loger;
        }

        [HttpGet("GetEdgeInformation")]
        public async Task<IActionResult> GetEdgesInformation()
        {
            PostgrelDBController postgrelDBController = new PostgrelDBController(_config);
            var dbs = await postgrelDBController.GetDatabases();
            List<ViewModels.EdgeStatus> infos = new List<ViewModels.EdgeStatus>();
            Database.PostgresHepler helper = new Database.PostgresHepler(_config);
            var useageDict = await helper.GetDatabaseUsageState();
            foreach (var db in dbs)
            {
                using var context = new IDMSContext(_config, db, "public");
                try
                {
                    var pcinfo = context.pc_information.FirstOrDefault();
                    if (pcinfo != null)
                    {
                        var status = new ViewModels.EdgeStatus
                        {
                            EdgeIP = pcinfo.edge_ip,
                            Name = db,
                            SensorNum = pcinfo.online_sensor_cnt,
                            Status = (DateTime.Now - pcinfo.datetime).TotalSeconds < 30 ? "Online" : "Offline",

                        };
                        useageDict.TryGetValue(db, out var _value);
                        status.DbDickUsage = _value == null ? "?" : _value;
                        infos.Add(status);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return Ok(infos);
        }

        [HttpGet("DBDiskUsage")]
        public async Task<IActionResult> GetDBDiskUsage()
        {
            Database.PostgresHepler helper = new Database.PostgresHepler(_config);
            var useageDict = await helper.GetDatabaseUsageState();
            return Ok(useageDict);
        }

        [HttpGet("VibrationEnergy")]
        public async Task<IActionResult> GetVibrationEnergy(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel)
        {
            int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel / 2);
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            List<Vibration_Energy>? _data = context.vibration_energy.OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();
            List<Vibration_Energy> data = new List<Vibration_Energy>();
            if (_data.Count < downSampleCnt)
            {
                data = _data;
            }
            else
                for (int i = 0; i < _data.Count; i += downSampleCnt)
                {
                    var ls = _data.Skip(i).Take(downSampleCnt);
                    double minVal = ls.Min(h => h.x);
                    var min = ls.FirstOrDefault(h => h.x == minVal);
                    if (min != null)
                        data.Add(min);
                    double maxVal = ls.Max(h => h.x);
                    var max = ls.FirstOrDefault(h => h.x == maxVal);
                    if (max != null)
                        data.Add(max);
                }

            List<DateTime>? timeList = data.OrderBy(i => i.datetime).Select(i => i.datetime).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel();
            chartData.labels = timeList;
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "X",
                data = data.Select(d => d.x).ToList(),
                borderColor = "blue",
                backgroundColor = "blue",
            });
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Y",
                data = data.Select(d => d.y).ToList(),
                borderColor = "green",
                backgroundColor = "green"
            });

            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Z",
                data = data.Select(d => d.z).ToList(),
                borderColor = "red",
                backgroundColor = "red",
            });
            //if (data.Count() > 1500)
            //{
            //    string queryID = chartData.AddToCache();
            //    ViewModels.ChartingViewModel previewChartData = DownSamplingVMBuilder(queryID, "振動能量", chartData.labels, data.Select(d => (object)d).ToList(), "x", ymax: chartData.ymax, ymin: chartData.ymin);
            //    return Ok(previewChartData);
            //}


            return Ok(chartData);
        }


        [HttpGet("Physical_quantity")]
        public async Task<IActionResult> GetPhysicalQuantity(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel)
        {
            try
            {
                int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel );
                using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
                List<Physical_quantity> _data = context.physical_quantity.OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();
                List<Physical_quantity> data = new List<Physical_quantity>();
                if (_data.Count < downSampleCnt)
                {
                    data = _data;
                }
                else
                    for (int i = 0; i < _data.Count; i += downSampleCnt)
                    {
                        var ls = _data.Skip(i).Take(downSampleCnt);
                        double minVal = ls.Min(h => h.xacceleration_peak_to_peak);
                        var min = ls.FirstOrDefault(h => h.xacceleration_peak_to_peak == minVal);
                        if (min != null)
                            data.Add(min);
                        double maxVal = ls.Max(h => h.xacceleration_peak_to_peak);
                        var max = ls.FirstOrDefault(h => h.xacceleration_peak_to_peak == maxVal);
                        if (max != null)
                            data.Add(max);
                    }

                data = data.OrderBy(i => i.datetime).ToList();
                List<DateTime>? timeList = data .Select(i => i.datetime).ToList();

                ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel()
                {
                    ymax = -1,
                    ymin = -1
                };
                chartData.labels = timeList;
                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "加速度-P2P.X",
                    data = data.Select(d => d.xacceleration_peak_to_peak).ToList(),
                    borderColor = "rgb(0, 0, 255)",
                });
                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "加速度-P2P.Y",
                    data = data.Select(d => d.yacceleration_peak_to_peak).ToList(),
                    borderColor = "rgb(0, 128, 0)",
                });
                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "加速度-P2P.Z",
                    data = data.Select(d => d.xacceleration_peak_to_peak).ToList(),
                    borderColor = "rgb(255, 0, 0)",
                });

                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "速度.X",
                    data = data.Select(d => d.xvelocity).ToList(),
                    borderColor = "rgba(0, 0, 255,.7)",
                });
                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "速度.Y",
                    data = data.Select(d => d.yvelocity).ToList(),
                    borderColor = "rgba(0, 128,0,.7)",
                });
                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "速度.Z",
                    data = data.Select(d => d.zvelocity).ToList(),
                    borderColor = "rgba(255, 0, 0,.7)",
                });

                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "速度.X",
                    data = data.Select(d => d.xdisplacement).ToList(),
                    borderColor = "rgba(0, 0, 255,.3)",
                });
                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "速度.Y",
                    data = data.Select(d => d.ydisplacement).ToList(),
                    borderColor = "rgb(0, 128,0,.3)",
                });
                chartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "位移.Z",
                    data = data.Select(d => d.zdisplacement).ToList(),
                    borderColor = "rgba(255, 0, 0,.3)",
                });
                //if (data.Count() > 1500)
                //{
                //    string queryID = chartData.AddToCache();
                //    ViewModels.ChartingViewModel previewChartData = DownSamplingVMBuilder(queryID, "xacceleration_peak_to_peak", chartData.labels, data.Select(d => (object)d).ToList(), "xacceleration_peak_to_peak", fillMethod: "false", ymax: chartData.ymax, ymin: chartData.ymin);
                //    return Ok(previewChartData);
                //}

                return Ok(chartData);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("SideBandSeverity")]
        public async Task<IActionResult> GetSideBandSeverity(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel)
        {
            int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel / 2);
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            List<Side_Band> _data = context.side_band.OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();
            List<Side_Band> data = new List<Side_Band>();
            if (_data.Count < downSampleCnt)
            {
                data = _data;
            }
            else
                for (int i = 0; i < _data.Count; i += downSampleCnt)
                {
                    var ls = _data.Skip(i).Take(downSampleCnt);
                    double minVal = ls.Min(h => h.severity.List_SideBandInfo[0].Severity);
                    var min = ls.FirstOrDefault(h => h.severity.List_SideBandInfo[0].Severity == minVal);
                    if (min != null)
                        data.Add(min);
                    double maxVal = ls.Max(h => h.severity.List_SideBandInfo[0].Severity);
                    var max = ls.FirstOrDefault(h => h.severity.List_SideBandInfo[0].Severity == maxVal);
                    if (max != null)
                        data.Add(max);
                }
            data = data.OrderBy(i => i.datetime).ToList();
            List<DateTime>? timeList = data .Select(i => i.datetime).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel();
            chartData.labels = timeList;
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "SideBand.Severity.X",
                data = data.Select(d => d.severity.List_SideBandInfo[0].Severity).ToList(),
                borderColor = "blue",
            });
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "SideBand.Severity.Y",
                data = data.Select(d => d.severity.List_SideBandInfo[1].Severity).ToList(),
                borderColor = "green",
            });
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "SideBand.Severity.Z",
                data = data.Select(d => d.severity.List_SideBandInfo[2].Severity).ToList(),
                borderColor = "red",
            });

            //if (data.Count() > 1500)
            //{
            //    string queryID = chartData.AddToCache();
            //    ViewModels.ChartingViewModel previewChartData = DownSamplingVMBuilder(queryID, "SideBand.Severity", chartData.labels, chartData.datasets[0].data, "severity", fillMethod: "false", ymax: chartData.ymax, ymin: chartData.ymin);
            //    return Ok(previewChartData);
            //}

            return Ok(chartData);
        }

        [HttpGet("Frequency_doublingSeverity")]
        public async Task<IActionResult> GetFrequency_doubling(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel)
        {
            int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel / 2);
            List<Frequency_doubling> _data = new List<Frequency_doubling>();
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            _data = context.frequency_doubling.OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();

            List<Frequency_doubling> data = new List<Frequency_doubling>();
            if (_data.Count < downSampleCnt)
            {
                data = _data;
            }
            else
                for (int i = 0; i < _data.Count; i += downSampleCnt)
                {
                    var ls = _data.Skip(i).Take(downSampleCnt);
                    double minVal = ls.Min(h => h.severity.List_MultiFreqLog[0].Severity);
                    var min = ls.FirstOrDefault(h => h.severity.List_MultiFreqLog[0].Severity == minVal);
                    if (min != null)
                        data.Add(min);
                    double maxVal = ls.Max(h => h.severity.List_MultiFreqLog[0].Severity);
                    var max = ls.FirstOrDefault(h => h.severity.List_MultiFreqLog[0].Severity == maxVal);
                    if (max != null)
                        data.Add(max);
                }

            data = data.OrderBy(i => i.datetime).ToList();
            List<DateTime>? timeList = data.Select(i => i.datetime).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel();
            chartData.labels = timeList;
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Frequency_Doubling.Severity.X",
                data = data.Select(d => d.severity.List_MultiFreqLog[0].Severity).ToList(),
                borderColor = "blue",
            });
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Frequency_Doubling.Severity.Y",
                data = data.Select(d => d.severity.List_MultiFreqLog[1].Severity).ToList(),
                borderColor = "green",
            });
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Frequency_Doubling.Severity.Z",
                data = data.Select(d => d.severity.List_MultiFreqLog[2].Severity).ToList(),
                borderColor = "red",
            });

            //if (data.Count() > 1500)
            //{
            //    string queryID = chartData.AddToCache();
            //    ViewModels.ChartingViewModel previewChartData = DownSamplingVMBuilder(queryID, "Frequency_Doubling.Severity", chartData.labels, chartData.datasets[0].data, "severity", fillMethod: "false", ymax: chartData.ymax, ymin: chartData.ymin);
            //    return Ok(previewChartData);
            //}

            return Ok(chartData);
        }
        [HttpGet("HealthScore")]
        public async Task<IActionResult> GetHealthScore(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel)
        {
            int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel /2);

            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            List<Health_Score> _data = context.health_score.OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();
            List<Health_Score> data = new List<Health_Score>();

            if (_data.Count < downSampleCnt)
            {
                data = _data;
            }
            else
            {
                for (int i = 0; i < _data.Count; i += downSampleCnt)
                {
                    var ls = _data.Skip(i).Take(downSampleCnt);
                    double minVal = ls.Min(h => h.score);
                    var min = ls.FirstOrDefault(h => h.score == minVal);
                    if (min != null)
                        data.Add(min);
                    double maxVal = ls.Max(h => h.score);
                    var max = ls.FirstOrDefault(h => h.score == maxVal);
                    if (max != null)
                        data.Add(max);
                }
            }

            data = data.OrderBy(i => i.datetime).ToList();
            List<DateTime>? timeList = data .Select(i => i.datetime).ToList();

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
                backgroundColor = "white"
            });
            chartData.datasets.Add(new ViewModels.DataSet
            {
                label = "Score",
                data = data.Select(d => d.score).ToList(),
                borderColor = "blue",
                backgroundColor = "blue",
            });

            //if (data.Count() > 1500)
            //{
            //    string queryID = chartData.AddToCache();
            //    ViewModels.ChartingViewModel previewChartData = DownSamplingVMBuilder(queryID, "Score", chartData.labels, data.Select(d => (object)d).ToList(), "score", fillMethod: "false", ymax: chartData.ymax, ymin: chartData.ymin);
            //    return Ok(previewChartData);
            //}

            return Ok(chartData);
        }

        [HttpGet("AlertIndex")]
        public async Task<IActionResult> GetAlertIndex(string edgename, string ip, DateTime from, DateTime to)
        {
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            List<Alert_Index>? _data = context.alert_index.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime).ToList();

            List<Alert_Index> data = _data;
            //for (int i = 0; i < _data.Count; i += 1)
            //{
            //    Alert_Index ai = _data.Skip(i).Take(1).FirstOrDefault();
            //    if (ai != null)
            //        data.Add(ai);
            //}


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
                fill = "false"
            });

            //if (data.Count() > 1500)
            //{
            //    string queryID = chartData.AddToCache();
            //    ViewModels.ChartingViewModel previewChartData = DownSamplingVMBuilder(queryID, "Alert Index", chartData.labels, data.Select(d => (object)d).ToList(), "alert_index", fillMethod: "false", ymax: chartData.ymax, ymin: chartData.ymin);
            //    return Ok(previewChartData);
            //}
            return Ok(chartData);
        }

        [HttpGet("vibration_raw_data_with_QueryID")]
        public async Task<IActionResult> GetVibration_raw_data_with_queryID(string edgename, string ip, DateTime from, DateTime to, string queryID)
        {
            Stopwatch watch = Stopwatch.StartNew();

            _logger.LogInformation("User query Edge:{0} Module:{1} From:{2} To:{3} - QueryID:{4} ", edgename, ip, from, to, queryID);
            ViewModels.ChartingViewModel vibrationChartData = new ViewModels.ChartingViewModel();

            IOrderedQueryable<Vibration_raw_data>? resultall = null;
            int count = 0;
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            try
            {
                resultall = context.vibration_raw_data.Where(i => i.datetime >= from && i.datetime <= to).OrderBy(i => i.datetime);
                count = resultall.Count();
            }
            catch (Exception ex)
            {
                return Ok(vibrationChartData);
            }
            //context.vibration_raw_data.SkipWhile()
            if (count == 0)
            {
                return Ok(vibrationChartData);
            }
            bool isSmallAmountData = count < 200;
            bool isPreview = queryID == null && !isSmallAmountData;


            vibrationChartData.isPreview = isPreview;
            vibrationChartData.QueryID = isPreview ? $"Raw-${DateTime.Now}" : null;


            List<Vibration_raw_data> datals = new List<Vibration_raw_data>();

            if (isPreview)
            {
                try
                {
                    for (int i = 0; i < count; i += count / 100)
                    {
                        if (i >= count)
                            break;
                        var data = resultall.Skip(i).Take(1).First();
                        datals.Add(data);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetVibration_raw_data_with_queryID");
                }

                vibrationChartData.labels = datals.Select(i => i.datetime).ToList();
                vibrationChartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "X",
                    borderColor = "blue",
                    backgroundColor = "blue",
                    data = datals.Select(i => i.x.Average()).ToList()
                });
                vibrationChartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "y",
                    borderColor = "green",
                    backgroundColor = "green",
                    data = datals.Select(i => i.y.Average()).ToList()
                });
                vibrationChartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "z",
                    borderColor = "red",
                    backgroundColor = "red",
                    data = datals.Select(i => i.z.Average()).ToList()
                });
            }
            else
            {
                datals = resultall.ToList();
                vibrationChartData.labels = new List<DateTime>();
                vibrationChartData.ymax = 2;
                vibrationChartData.ymin = -2;
                vibrationChartData.datasets.Add(new ViewModels.DataSet() { label = "X", borderColor = "blue", backgroundColor = "blue" });
                vibrationChartData.datasets.Add(new ViewModels.DataSet() { label = "Y", borderColor = "green", backgroundColor = "green" });
                vibrationChartData.datasets.Add(new ViewModels.DataSet() { label = "Z", borderColor = "red", backgroundColor = "red" });


                foreach (var item in datals)
                {
                    var dataBegin = item.datetime;
                    for (int i = 0; i < 512; i++)
                    {
                        vibrationChartData.labels.Add(dataBegin.AddMilliseconds(i * 1.0 / 8000.0 * 1000.0));
                    }

                    vibrationChartData.datasets[0].data.AddRange(item.x);
                    vibrationChartData.datasets[1].data.AddRange(item.y);
                    vibrationChartData.datasets[2].data.AddRange(item.z);
                }
            }
            watch.Stop();
            _logger.LogWarning("Raw Data Query OUt Time Spend:{0}", watch.Elapsed);

            return Ok(vibrationChartData);
        }

        [HttpGet("vibration_raw_data")]
        public async Task<IActionResult> GetVibration_raw_data(string edgename, string ip, DateTime from, DateTime to)
        {
            _logger.LogInformation("User query Edge:{0} Module:{1} From:{2} To:{3} ", edgename, ip, from, to);
            return await GetVibration_raw_data_with_queryID(edgename, ip, from, to, null);
            //var data = context.vibration_raw_data.Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime).ToList();

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
        }

        /// <summary>
        /// 查詢切片檔案
        /// </summary>
        /// <param name="queryID"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet("GetSplice")]
        public async Task<IActionResult> GetSplice(string queryID, DateTime from, DateTime to)
        {
            QueryResultManager.TryGetData(queryID, from, to, out ViewModels.ChartingViewModel spliceDataRet);
            return Ok(spliceDataRet);
        }

        private ViewModels.ChartingViewModel DownSamplingVMBuilder(string queryID, string label, List<DateTime> dataTimesSource,
            List<object> dataSource, string propertyName, string color = "orange", string fillMethod = "false", double ymax = -1, double ymin = -1)
        {
            clsPreviewData preview = new clsPreviewData(dataTimesSource, dataSource.Select(i => i.GetType().GetProperty(propertyName).GetValue(i)).Select(d => (double)d).ToList());
            ViewModels.ChartingViewModel downSamplingChartData = new ViewModels.ChartingViewModel()
            {
                ymin = ymin,
                ymax = ymax,
            };

            downSamplingChartData.QueryID = queryID;
            downSamplingChartData.labels = preview.TimeLs;
            downSamplingChartData.datasets.Add(new ViewModels.DataSet
            {
                label = label,
                data = preview.DataLs,
                borderColor = color,
                fill = fillMethod,

            });

            return downSamplingChartData;
        }
        private ViewModels.ChartingViewModel DownSamplingVMBuilder(string queryID, string label, List<DateTime> dataTimesSource,
           List<double> dataSource, string propertyName, string color = "orange", string fillMethod = "false", double ymax = -1, double ymin = -1)
        {
            clsPreviewData preview = new clsPreviewData(dataTimesSource, dataSource);
            ViewModels.ChartingViewModel previewChartData = new ViewModels.ChartingViewModel()
            {
                ymin = ymin,
                ymax = ymax,
            };

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

        private string SensorSchema(string ip)
        {
            return $"sensor_{ip.Replace(".", "_")}";
        }

        #region 事件


        /// <summary>
        /// 查詢診斷分數超出閥值的事件
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet("DignoseOutOfThreshold")]
        public async Task<IActionResult> DignoseOutOfThreshold(string ip, DateTime from, DateTime to, int Page = 1)
        {
            using var context = new IDMSContext(_config, ip);
            var eventDatas = context.health_score.Where(i => i.score <= i.warning_threshold && i.datetime >= from && i.datetime <= to).OrderBy(i => i.datetime);

            var Data = eventDatas.Skip((Page - 1) * 15).Take(15).Select(dat => new ViewModels.HealthOutOfThresholdViewModel()
            {
                TimeStamp = dat.datetime,
                SensorIP = ip,
                Warning_Threshold = dat.warning_threshold,
                Alarm_Threshold = dat.alarm_threshold,
                OutOfAlarmThreshold = dat.score < dat.alarm_threshold,
                OutOfWarningThreshold = dat.score < dat.warning_threshold,
                ModelName = dat.recipe_id,
            }).ToList();
            ViewModels.EventViewModel result = new ViewModels.EventViewModel
            {
                SensorIP = ip,
                TotalNum = eventDatas.Count(),
                Page = Page,
                Data = Data,
                ElTableColumns = new List<ViewModels.EltableColumn>()
                 {
                      new ViewModels.EltableColumn{ label="使用模型",  prop = "ModelName", type="string"},
                      new ViewModels.EltableColumn{ label="Warning 閥值",  prop = "Warning_Threshold", type="double"},
                      new ViewModels.EltableColumn{ label="Alarm 閥值",  prop = "Alarm_Threshold", type="double"},
                      new ViewModels.EltableColumn{ label="超出Warning",  prop = "OutOfWarningThreshold", type="boolean", bindingStyle="color:red"},
                      new ViewModels.EltableColumn{ label="超出Alarm",  prop = "OutOfAlarmThreshold", type="boolean"},
                 }
            };
            return Ok(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="axis_IsOutofThreshold"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="Page"></param>
        /// <returns></returns>
        [HttpGet("SideBandSeverityOutOfThres")]
        public async Task<IActionResult> SideBandSeverity_X_OutOfThres(string ip, DateTime from, DateTime to, int Page = 1)
        {
            using var context = new IDMSContext(_config, ip);
            var eventDatas = context.side_band.Where(i => (i.severity.Threshold.X_IsOutofThreshold | i.severity.Threshold.Y_IsOutofThreshold | i.severity.Threshold.Z_IsOutofThreshold) && i.datetime >= from && i.datetime <= to).OrderBy(i => i.datetime);

            var Data = eventDatas.Skip((Page - 1) * 15).Take(15).Select(dat => new ViewModels.SideBandSeverityOutOfThresHoldViewModel()
            {
                SensorIP = ip,
                TimeStamp = dat.datetime,
                ThresHold_X = dat.severity.Threshold.X,
                ThresHold_Y = dat.severity.Threshold.Y,
                ThresHold_Z = dat.severity.Threshold.Z,
                Is_Outof_ThresHold_X = dat.severity.Threshold.X_IsOutofThreshold,
                Is_Outof_ThresHold_Y = dat.severity.Threshold.Y_IsOutofThreshold,
                Is_Outof_ThresHold_Z = dat.severity.Threshold.Z_IsOutofThreshold,
            }).ToList();
            ViewModels.EventViewModel result = new ViewModels.EventViewModel
            {
                SensorIP = ip,
                TotalNum = eventDatas.Count(),
                Page = Page,
                Data = Data,
                ElTableColumns = new List<ViewModels.EltableColumn>()
                 {
                      new ViewModels.EltableColumn{ label="X 閥值",  prop = "ThresHold_X", type="double"},
                      new ViewModels.EltableColumn{ label="Y 閥值",  prop = "ThresHold_Y", type="double"},
                      new ViewModels.EltableColumn{ label="Z 閥值",  prop = "ThresHold_Z", type="double"},
                      new ViewModels.EltableColumn{ label="X 超出閥值",  prop = "Is_Outof_ThresHold_X", type="boolean"},
                      new ViewModels.EltableColumn{ label="Y 超出閥值",  prop = "Is_Outof_ThresHold_Y", type="boolean"},
                      new ViewModels.EltableColumn{ label="Z 超出閥值",  prop = "Is_Outof_ThresHold_Z", type="boolean"},
                 }
            };
            return Ok(result);
        }
        #endregion
    }
}
