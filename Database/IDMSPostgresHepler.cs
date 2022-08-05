
using IDMSWebServer.Models;
using Npgsql;
using System.Data;
using System.Diagnostics;

namespace IDMSWebServer.Database
{
    public class IDMSPostgresHepler : PostgresHepler
    {
        public enum QUERY_TYPE
        {
            health_score,
            alert_index
        }

        public IDMSPostgresHepler(IConfiguration config):base(config)
        {
        }
        #region API
        internal clsQueryResult HealthScoreQuery(string ip, DateTime from, DateTime to)
        {
            List<clsDataValueInfo> clsDataValueInfos = new List<clsDataValueInfo>()
            {
                 new clsDataValueInfo("score_wma","white"),
                 new clsDataValueInfo("score","rgb(0, 69, 255)"),
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
                return new clsQueryResult
                {
                    QueryID = result.QueryID,
                    preview = new clsPreviewData(result.timeList, result.valueList[1].valueList, 50)
                };
            }
            else
            {
                return result;
            }
        }

        internal clsQueryResult AlertIndexeQuery(string ip, DateTime from, DateTime to)
        {
            Stopwatch sw = Stopwatch.StartNew();
            bool success = TryGetTableFromDB(SqlCommandStringBuilder(QUERY_TYPE.alert_index, ip, from, to), out int dataNum, out DataTable table, out string message);
            clsQueryResult result = new clsQueryResult(table, "datetime", "alert_index") { message = message, dbConnected = success };
            sw.Stop();
            Console.WriteLine($"{success},{dataNum},{sw.Elapsed}");
            result.TimeSpend = sw.Elapsed.ToString();

            return result;
        }

        internal List<clsModuleInfo> ModuleInfosQuery()
        {
            string sqlCommand = $"SELECT sensor_ip,eq_name,unit_name,type FROM public.all_sensor_information";
            TryGetTableFromDB(sqlCommand,out int dataNum, out DataTable table,out string message);
            List<clsModuleInfo> result = new List<clsModuleInfo>();

            result = table.Rows.Cast<DataRow>().Select(row => new clsModuleInfo( (string) row["sensor_ip"], (string)row["eq_name"], (string)row["unit_name"])).ToList();

            return result;
        }



        #endregion
        protected string SqlCommandStringBuilder(QUERY_TYPE query_item, string ip, DateTime from, DateTime to, List<string> columnNames = null)
        {
            string schema_name = $"sensor_{ip.Replace(".", "_")}";
            string table_name = query_item.ToString();
            string condition = $"datetime > '{from:yyyy-MM-dd HH:mm:ss}' AND  datetime < '{to:yyyy-MM-dd HH:mm:ss}' order by datetime asc";
            string columnNamesstr = $"{(columnNames == null ? "*" : "datetime," + string.Join(",", columnNames))}";
            return $"SELECT {columnNamesstr} FROM {schema_name}.{table_name} WHERE {condition} ";
        }

    }
}
