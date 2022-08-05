using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IDMSWebServer.Models;
using Npgsql;
using System.Data;
using System.Diagnostics;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        IConfiguration _config;
        public QueryController(IConfiguration config)
        {
            Host = config["Postgres:Host"];
            Port = config["Postgres:Port"];
            Database = config["Postgres:Database"];
            UserName = config["Postgres:UserName"];
            Password = config["Postgres:Password"];

        }

        //public static string DBHost = "127.0.0.1";
        public string Host = "127.0.0.1";
        public string Port = "5432";
        public string Database = "idms_pc";
        public string UserName = "postgres";
        public string Password = "changeme";
        public enum QUERY_TYPE
        {
            health_score,
            alert_index
        }

        [HttpGet("HealthScore")]
        public async Task<IActionResult> HealthScore(string ip, DateTime from, DateTime to)
        {
            List<clsDataValueInfo> clsDataValueInfos = new List<clsDataValueInfo>()
            {
                 new clsDataValueInfo("score_wma","white"),
                 new clsDataValueInfo("score","blue"),
            };

            Stopwatch sw = Stopwatch.StartNew();
            List<string> columns = clsDataValueInfos.Select(t => t.labelName).ToList();
            bool success1 = TryGetTableFromDB(SqlCommandStringBuilder(QUERY_TYPE.health_score, ip, from, to, columns), out int dataNum, out DataTable table, out string message);
            clsQueryResult result = new clsQueryResult(table, "datetime", clsDataValueInfos) { message = message, dbConnected = success1 };
            sw.Stop();


            Console.WriteLine($"{success1},{dataNum},{sw.Elapsed}");
            result.TimeSpend = sw.Elapsed.ToString();
            if (result.count > 2000)
            {
                return Ok(new clsQueryResult
                {
                    QueryID = result.QueryID,
                    preview = new clsPreviewData(result.timeList, result.valueList[1].valueList)
                });
            }
            else
            {
                return Ok(result);
            }

            //sw.Restart();
            //Dictionary<string, List<object>> output = clsDataValueInfos.ToDictionary(t => t.labelName, t => new List<object>());
            //bool success2 = TryGetTableFromDB(SqlCommandStringBuilder(QUERY_TYPE.health_score, ip, from, to, columns), out int dataNum2, ref output, out string message2);
            //clsQueryResult result2 = new clsQueryResult(output, "datetime", clsDataValueInfos) { message = message2, dbConnected = success2 };

            //sw.Stop();
            //Console.WriteLine($"{success2},{dataNum2},{sw.Elapsed}");
            //if (result.count > 100)
            //{
            //    result.preview = new clsPreviewData(result.timeList, result.valueList[1].valueList);
            //}
            //sw.Stop();
            //return Ok();
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
            Stopwatch sw = Stopwatch.StartNew();

            QueryResultManager.TryGetData(queryID, from, to, out clsQueryResult spliceDataRet);
            return Ok(spliceDataRet);
            //bool success = TryGetTableFromDB(SqlCommandStringBuilder(QUERY_TYPE.health_score, ip, from, to), out int dataNum, out DataTable table, out string message);
            //List<clsDataValueInfo> clsDataValueInfos = new List<clsDataValueInfo>()
            //{
            //     new clsDataValueInfo("score_wma","white"),
            //     new clsDataValueInfo("score","blue"),
            //};
            //clsQueryResult result = new clsQueryResult(table, "datetime", clsDataValueInfos) { message = message, dbConnected = success };
            //sw.Stop();
            //result.TimeSpend = sw.Elapsed.ToString();
            //return Ok(result);
        }




        [HttpGet("AlertIndex")]
        public async Task<IActionResult> AlertIndex(string ip, DateTime from, DateTime to)
        {
            Stopwatch sw = Stopwatch.StartNew();
            bool success = TryGetTableFromDB(SqlCommandStringBuilder(QUERY_TYPE.alert_index, ip, from, to), out int dataNum, out DataTable table, out string message);
            clsQueryResult result = new clsQueryResult(table, "datetime", "alert_index") { message = message, dbConnected = success };
            sw.Stop();
            Console.WriteLine($"{success},{dataNum},{sw.Elapsed}");
            result.TimeSpend = sw.Elapsed.ToString();

            return Ok(result);
        }
        [HttpGet("Raw")]
        public async Task<IActionResult> Raw(string ip, DateTime from, DateTime to)
        {

            return Ok(ip);
        }

        private string SqlCommandStringBuilder(QUERY_TYPE query_item, string ip, DateTime from, DateTime to, List<string> columnNames = null)
        {
            string schema_name = $"sensor_{ip.Replace(".", "_")}";
            string table_name = query_item.ToString();
            string condition = $"datetime > '{from:yyyy-MM-dd HH:mm:ss}' AND  datetime < '{to:yyyy-MM-dd HH:mm:ss}' order by datetime asc";
            string columnNamesstr = $"{(columnNames == null ? "*" : "datetime," + string.Join(",", columnNames))}";
            return $"SELECT {columnNamesstr} FROM {schema_name}.{table_name} WHERE {condition} ";
        }


        private bool TryGetTableFromDB(string sqlQueryString, out int dataNum, ref Dictionary<string, List<object>> _datatable, out string message)
        {
            message = "";
            dataNum = 0;
            NpgsqlConnection? conn = null;
            try
            {
                conn = new NpgsqlConnection($"Server={Host};Port={Port};Database={Database};User Id={UserName};Password={Password};");
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    using (var Command_Execute = new NpgsqlCommand(sqlQueryString, conn))
                    {
                        NpgsqlDataReader dr = Command_Execute.ExecuteReader();

                        while (dr.Read())
                        {
                            var info = dr.GetColumnSchema();
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                string columnName = info[i].ColumnName;
                                if (!_datatable.ContainsKey(columnName))
                                    _datatable.Add(columnName, new List<object>());
                                _datatable[columnName].Add(dr[columnName]);
                            }
                        }
                        bool re = dr.Read();
                        //NpgsqlDataAdapter DataAdapte = new NpgsqlDataAdapter(Command_Execute);
                        //DataAdapte.Fill(_datatable);
                    }
                    dataNum = _datatable[_datatable.Keys.First()].Count;
                    conn.CloseAsync();
                    return true;
                }
                else
                {
                    message = "資料庫無法開啟";
                    return false;
                }
            }
            catch (Exception ex)
            {
                conn?.Close();
                message = $"嘗試與資料庫進行連線時發生例外:${ex.Message}";
                return false;
            }
        }

        private bool TryGetTableFromDB(string sqlQueryString, out int dataNum, out DataTable _datatable, out string message)
        {
            message = "";
            dataNum = 0; _datatable = new DataTable();
            try
            {
                var conn = new NpgsqlConnection($"Server={Host};Port={Port};Database={Database};User Id={UserName};Password={Password};");
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    using (var Command_Execute = new NpgsqlCommand(sqlQueryString, conn))
                    {
                        NpgsqlDataAdapter DataAdapte = new NpgsqlDataAdapter(Command_Execute);
                        DataAdapte.Fill(_datatable);
                    }
                    dataNum = _datatable.Rows.Count;
                    conn.CloseAsync();
                    return true;
                }
                else
                {
                    message = "資料庫無法開啟";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = $"嘗試與資料庫進行連線時發生例外:${ex.Message}";
                return false;
            }
        }
    }
}
