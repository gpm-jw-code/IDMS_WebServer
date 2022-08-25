using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IDMSWebServer.Models.DataModels;
using IDMSWebServer.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using IDMSWebServer.Models.Charting;

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
                    var pcinfo = context.pc_information.AsNoTracking().FirstOrDefault();
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
        public async Task<IActionResult> GetVibrationEnergy(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel, string? customSettingJson)
        {
            await Task.Delay(1);
            int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel / 2);
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            var _data = context.vibration_energy.AsNoTracking().OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();
            List<Vibration_Energy> data = new List<Vibration_Energy>();

            int cnt = _data.Count();
            if (cnt < downSampleCnt)
            {
                data = _data.Select(v => v).ToList();
            }
            else
                for (int i = 0; i < cnt; i += downSampleCnt)
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
            data = data.OrderBy(i => i.datetime).ToList();
            List<DateTime>? timeList = data.OrderBy(i => i.datetime).Select(i => i.datetime).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel();
            chartData.labels = timeList;
            chartData.datasets = DatasetFactory.CreateVibrationEnergyDatasets(data.Select(d => d.x).ToList(), data.Select(d => d.y).ToList(), data.Select(d => d.z).ToList());
            chartData.CustomStyleSettingApply(customSettingJson);

            context.Dispose();
            return Ok(chartData);
        }


        [HttpGet("Physical_quantity")]
        public async Task<IActionResult> GetPhysicalQuantity(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel, string? customSettingJson)
        {
            await Task.Delay(1);
            try
            {
                int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel);
                using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
                var _data = context.physical_quantity.AsNoTracking().OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();
                List<Physical_quantity> data = new List<Physical_quantity>();
                var cnt = _data.Count();
                if (cnt < downSampleCnt)
                {
                    data = _data.ToList();
                }
                else
                    for (int i = 0; i < cnt; i += downSampleCnt)
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
                List<DateTime>? timeList = data.Select(i => i.datetime).ToList();

                ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel()
                {
                    ymax = -1,
                    ymin = -1
                };
                chartData.labels = timeList;
                chartData.datasets = DatasetFactory.CreatePhysicalQuantityDatasets(
                    data.Select(d => d.xacceleration_peak_to_peak).ToList(), data.Select(d => d.yacceleration_peak_to_peak).ToList(), data.Select(d => d.zacceleration_peak_to_peak).ToList(),
                    data.Select(d => d.xvelocity).ToList(), data.Select(d => d.yvelocity).ToList(), data.Select(d => d.zvelocity).ToList(),
                    data.Select(d => d.xdisplacement).ToList(), data.Select(d => d.ydisplacement).ToList(), data.Select(d => d.zdisplacement).ToList()
                );
                chartData.CustomStyleSettingApply(customSettingJson);

                context.Dispose();
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("SideBandSeverity")]
        public async Task<IActionResult> GetSideBandSeverity(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel, string? customSettingJson)
        {
            await Task.Delay(1);
            int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel / 2);
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            var _data = context.side_band.AsNoTracking().OrderBy(i => i.datetime).Select(r => r).Where(i => i.datetime >= from && i.datetime <= to).ToList();
            List<Side_Band> data = new List<Side_Band>();
            var cnt = _data.Count();
            if (cnt < downSampleCnt)
            {
                data = _data.ToList();
            }
            else
                for (int i = 0; i < cnt; i += downSampleCnt)
                {
                    var ls = _data.Skip(i).Take(downSampleCnt).ToList();
                    double minVal = ls.Select(h => h.severity.List_SideBandInfo[0].Severity).Min();
                    var min = ls.FirstOrDefault(h => h.severity.List_SideBandInfo[0].Severity == minVal);
                    if (min != null)
                        data.Add(min);
                    double maxVal = ls.Select(h => h.severity.List_SideBandInfo[0].Severity).Max();
                    var max = ls.FirstOrDefault(h => h.severity.List_SideBandInfo[0].Severity == maxVal);
                    if (max != null)
                        data.Add(max);
                }
            data = data.OrderBy(i => i.datetime).ToList();
            List<DateTime>? timeList = data.Select(i => i.datetime).ToList();

            var xData = data.Select(d => d.severity.List_SideBandInfo[0].Severity).ToList();
            var yData = data.Select(d => d.severity.List_SideBandInfo[1].Severity).ToList();
            var zData = data.Select(d => d.severity.List_SideBandInfo[2].Severity).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel();
            chartData.labels = timeList;
            chartData.datasets = DatasetFactory.CreateSideBandSeverityDatasets(xData, yData, zData);
            chartData.CustomStyleSettingApply(customSettingJson);
            context.Dispose();
            return Ok(chartData);
        }

        [HttpGet("Frequency_doublingSeverity")]
        public async Task<IActionResult> GetFrequency_doubling(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel, string? customSettingJson)
        {
            await Task.Delay(1);
            int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel / 2);
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            var _data = context.frequency_doubling.AsNoTracking().OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();

            List<Frequency_doubling> data = new List<Frequency_doubling>();
            int cnt = _data.Count();
            if (cnt < downSampleCnt)
            {
                data = _data.ToList();
            }
            else
                for (int i = 0; i < cnt; i += downSampleCnt)
                {
                    var ls = _data.Skip(i).Take(downSampleCnt).ToList();
                    double minVal = ls.Select(h => h.severity.List_MultiFreqLog[0].Severity).Min();
                    var min = ls.FirstOrDefault(h => h.severity.List_MultiFreqLog[0].Severity == minVal);
                    if (min != null)
                        data.Add(min);
                    double maxVal = ls.Select(h => h.severity.List_MultiFreqLog[0].Severity).Max();
                    var max = ls.FirstOrDefault(h => h.severity.List_MultiFreqLog[0].Severity == maxVal);
                    if (max != null)
                        data.Add(max);
                }

            data = data.OrderBy(i => i.datetime).ToList();
            List<DateTime>? timeList = data.Select(i => i.datetime).ToList();

            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel();
            chartData.labels = timeList;
            var xData = data.Select(d => d.severity.List_MultiFreqLog[0].Severity).ToList();
            var yData = data.Select(d => d.severity.List_MultiFreqLog[1].Severity).ToList();
            var zData = data.Select(d => d.severity.List_MultiFreqLog[2].Severity).ToList();
            chartData.datasets = DatasetFactory.CreateFrequencyDoublingSeverityDatasets(xData, yData, zData);
            chartData.CustomStyleSettingApply(customSettingJson);

            context.Dispose();

            return Ok(chartData);
        }
        [HttpGet("HealthScore")]
        public async Task<IActionResult> GetHealthScore(string edgename, string ip, DateTime from, DateTime to, int? chart_pixel, string? customSettingJson)
        {
            await Task.Delay(1);
            int downSampleCnt = (int)(chart_pixel == null ? 100 : chart_pixel / 2);

            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            var _data = context.health_score.AsNoTracking().OrderBy(i => i.datetime).Where(i => i.datetime >= from && i.datetime <= to).ToList();
            List<Health_Score> data = new List<Health_Score>();
            int cnt = _data.Count();
            if (cnt < downSampleCnt)
            {
                data = _data.ToList();
            }
            else
            {
                for (int i = 0; i < cnt; i += downSampleCnt)
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
            List<DateTime>? timeList = data.Select(i => i.datetime).ToList();
            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel()
            {
                ymax = 100,
                ymin = 0
            };
            chartData.labels = timeList;
            chartData.datasets = DatasetFactory.CreateHealthScoreDatasets(data.Select(d => d.score).ToList(), data.Select(d => d.score_wma).ToList());

            context.Dispose();
            chartData.CustomStyleSettingApply(customSettingJson);
            return Ok(chartData);
        }

        [HttpGet("AlertIndex")]
        public async Task<IActionResult> GetAlertIndex(string edgename, string ip, DateTime from, DateTime to, string? customSettingJson)
        {
            await Task.Delay(1);
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            List<Alert_Index>? _data = context.alert_index.AsNoTracking().Where(i => i.datetime >= from && i.datetime <= to).Select(i => i).OrderBy(i => i.datetime).ToList();
            List<Alert_Index> data = _data;
            List<DateTime>? timeList = data.Select(i => i.datetime).ToList();
            ViewModels.ChartingViewModel chartData = new ViewModels.ChartingViewModel()
            {
                ymax = 0,
                ymin = 100,
            };
            chartData.labels = timeList;
            chartData.datasets = DatasetFactory.CreateAlertIndexDatasets(data.Select(d => d.alert_index).ToList());
            chartData.CustomStyleSettingApply(customSettingJson);
            context.Dispose();
            chartData.CustomStyleSettingApply(customSettingJson);

            return Ok(chartData);
        }

        [HttpGet("vibration_raw_data_with_QueryID")]
        public async Task<IActionResult> GetVibration_raw_data_with_queryID(string edgename, string ip, DateTime from, DateTime to, string queryID)
        {
            await Task.Delay(1);
            Stopwatch watch = Stopwatch.StartNew();

            _logger.LogInformation("User query Edge:{0} Module:{1} From:{2} To:{3} - QueryID:{4} ", edgename, ip, from, to, queryID);
            ViewModels.ChartingViewModel vibrationChartData = new ViewModels.ChartingViewModel();

            IOrderedQueryable<Vibration_raw_data>? resultall = null;
            int count = 0;
            using var context = new IDMSContext(_config, edgename, SensorSchema(ip));
            try
            {
                resultall = context.vibration_raw_data.AsNoTracking().Where(i => i.datetime >= from && i.datetime <= to).OrderBy(i => i.datetime);
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
                    borderColor = "rgb(30, 94, 197)",
                    data = datals.Select(i => i.x.Average()).ToList()
                });
                vibrationChartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "y",
                    borderColor = "rgb(48, 239, 149)",
                    data = datals.Select(i => i.y.Average()).ToList()
                });
                vibrationChartData.datasets.Add(new ViewModels.DataSet
                {
                    label = "z",
                    borderColor = "rgb(245, 62, 62)",
                    data = datals.Select(i => i.z.Average()).ToList()
                });
            }
            else
            {
                datals = resultall.ToList();
                vibrationChartData.labels = new List<DateTime>();
                vibrationChartData.ymax = 2;
                vibrationChartData.ymin = -2;
                vibrationChartData.datasets.Add(new ViewModels.DataSet() { label = "X", borderColor = "rgb(13, 110, 253)" });
                vibrationChartData.datasets.Add(new ViewModels.DataSet() { label = "Y", borderColor = "rgb(48, 239, 149)" });
                vibrationChartData.datasets.Add(new ViewModels.DataSet() { label = "Z", borderColor = "rgb(245, 62, 62)" });


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
            context.Dispose();

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
            var eventDatas = context.health_score.AsNoTracking().Where(i => i.score <= i.warning_threshold && i.datetime >= from && i.datetime <= to).OrderBy(i => i.datetime);

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
            var eventDatas = context.side_band.AsNoTracking().Where(i => (i.severity.Threshold.X_IsOutofThreshold | i.severity.Threshold.Y_IsOutofThreshold | i.severity.Threshold.Z_IsOutofThreshold) && i.datetime >= from && i.datetime <= to).OrderBy(i => i.datetime);

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
