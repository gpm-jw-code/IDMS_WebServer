using System.Data;
using System.Text.Json.Serialization;

namespace IDMSWebServer.Models
{
    public class clsQueryResult
    {

        public string QueryID { get; set; }
        public string TimeSpend { get; set; } = "";
        public List<DateTime> timeList { get; set; } = new List<DateTime>();

        public List<clsDataValueInfo> valueList { get; set; } = new List<clsDataValueInfo>();

        public bool dbConnected { get; set; }
        public string message { get; set; }
        public int count => timeList.Count;

        public clsPreviewData preview { get; set; }

        private IEnumerable<clsDataValueInfo> dataColNames;


        public clsQueryResult(DataTable table, string timeColName, string dataColName, string color = "orange")
        {
            var dict = table.Rows.Cast<DataRow>().ToDictionary(r => (DateTime)r[timeColName], r => (double)r[dataColName]);
            this.timeList = dict.Keys.ToList();
            this.valueList.Add(new clsDataValueInfo(dataColName, color) { valueList = dict.Values.ToList() });
            QueryID = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            QueryResultManager.AddResult(QueryID, this);
        }
        public clsQueryResult(DataTable table, string timeColName, IEnumerable<clsDataValueInfo> DataValueInfos)
        {
            foreach (var DataValueInfo in DataValueInfos)
            {
                var dict = table.Rows.Cast<DataRow>().ToDictionary(r => (DateTime)r[timeColName], r => (double)r[DataValueInfo.labelName]);

                if (DataValueInfo.isJson)
                {
                    var dict2 = table.Rows.Cast<DataRow>().ToDictionary(r => (DateTime)r[timeColName], r => System.Text.Json.JsonSerializer.Deserialize<double[]>((string)r[DataValueInfo.labelName]));

                }
                this.timeList = dict.Keys.ToList();
                DataValueInfo.valueList = dict.Values.ToList();
                this.valueList.Add(DataValueInfo);
            }
            QueryID = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            QueryResultManager.AddResult(QueryID, this);
        }
        public clsQueryResult(Dictionary<string, List<object>> table, string timeColName, IEnumerable<clsDataValueInfo> DataValueInfos)
        {
            this.timeList = table["datetime"].Select(v => (DateTime)v).ToList();
            foreach (var DataValueInfo in DataValueInfos)
            {
                var columnName = DataValueInfo.labelName;
                DataValueInfo.valueList = table[columnName].Select(v => (double)v).ToList();
                this.valueList.Add(DataValueInfo);
            }
            QueryID = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            QueryResultManager.AddResult(QueryID, this);
        }

        public clsQueryResult()
        {
        }
    }
    public class clsDataValueInfo
    {
        internal bool isJson = false;
        public string labelName { get; set; }
        public string displayColor { get; set; }
        public List<double> valueList { get; set; } = new List<double>();
        public clsDataValueInfo(string labelName, string displayColor)
        {
            this.labelName = labelName;
            this.displayColor = displayColor;
        }

        public clsDataValueInfo()
        {
        }
    }

    public class clsPreviewData
    {
        public clsPreviewData()
        {

        }
        public List<DateTime> TimeLs { get; private set; } = new List<DateTime>();
        public List<double> DataLs { get; private set; } = new List<double>();

        public clsPreviewData(List<DateTime> source_time, List<double> source_data, int WindowSize = 100)
        {


            var sourceDataAry = source_data.ToArray();
            var sourceTimeAry = source_time.ToArray();



            for (int i = 0; i < source_data.Count; i += WindowSize)
            {
                try
                {

                    int red = source_data.Count - i;
                    bool finalWindow = red < WindowSize;
                    ArraySegment<double> localDatas = new ArraySegment<double>(sourceDataAry, i, finalWindow ? red : WindowSize);
                    ArraySegment<DateTime> localTimes = new ArraySegment<DateTime>(sourceTimeAry, i, finalWindow ? red : WindowSize);
                    double min = localDatas.Min();
                    var index_of_min = localDatas.ToList().IndexOf(min);
                    TimeLs.Add(localTimes[index_of_min]);
                    DataLs.Add(min);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }

}
