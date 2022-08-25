namespace IDMSWebServer.Models
{
    public class QueryResultManager
    {

        public static Dictionary<string, clsQueryResult> QueryResultCaches = new Dictionary<string, clsQueryResult>();
        public static Dictionary<string, ViewModels.ChartingViewModel> QueryResultChartViewModelCaches = new Dictionary<string, ViewModels.ChartingViewModel>();


        public static void AddResult(string queryID, clsQueryResult result)
        {
            QueryResultCaches.Add(queryID, result);
        }

        public static void AddResult(string queryID, ViewModels.ChartingViewModel result)
        {
            QueryResultChartViewModelCaches.Add(queryID, result);
        }
        internal static bool TryGetData(string queryID, DateTime from, DateTime to, out ViewModels.ChartingViewModel spliceResult)
        {
            spliceResult = new ViewModels.ChartingViewModel();
            if (!QueryResultChartViewModelCaches.ContainsKey(queryID))
                return false;

            QueryResultChartViewModelCaches.TryGetValue(queryID, out ViewModels.ChartingViewModel result);

            spliceResult.ymin = result.ymin;
            spliceResult.ymax = result.ymax;

            var timeLs = result.labels.FindAll(t => t >= from && t <= to);
            var indexStart = result.labels.FindIndex(t => t == timeLs.First());
            var indexEnd = result.labels.FindIndex(t => t == timeLs.Last());
            //0 10 ->11比
            int num = indexEnd - indexStart + 1;

            List<ViewModels.DataSet> newValObjls = new List<ViewModels.DataSet>();
            foreach (var item in result.datasets)
            {
                double[] sourceAry = item.data.ToArray();
                ArraySegment<double> splicesData = new ArraySegment<double>(sourceAry, indexStart, num);
                newValObjls.Add(new ViewModels.DataSet()
                {
                    borderColor = item.borderColor,
                    label = item.label,
                    data = splicesData.ToList(),
                    fill = "false",
                });

            }
            spliceResult.labels = timeLs;
            spliceResult.datasets = newValObjls;

            return true;
        }
        internal static bool TryGetData(string queryID, DateTime from, DateTime to, out clsQueryResult spliceResult)
        {
            spliceResult = new clsQueryResult();
            if (!QueryResultCaches.ContainsKey(queryID))
                return false;

            QueryResultCaches.TryGetValue(queryID, out clsQueryResult result);

            var timeLs = result.timeList.FindAll(t => t >= from && t <= to);
            var indexStart = result.timeList.FindIndex(t => t == timeLs.First());
            var indexEnd = result.timeList.FindIndex(t => t == timeLs.Last());
            //0 10 ->11比
            int num = indexEnd - indexStart + 1;

            List<clsDataValueInfo> newValObjls = new List<clsDataValueInfo>();
            foreach (var item in result.valueList)
            {
                double[] sourceAry = item.valueList.ToArray();
                ArraySegment<double> splicesData = new ArraySegment<double>(sourceAry, indexStart, num);
                newValObjls.Add(new clsDataValueInfo()
                {
                    displayColor = item.displayColor,
                    isJson = item.isJson,
                    labelName = item.labelName,
                    valueList = splicesData.ToList()
                });

            }
            spliceResult.timeList = timeLs;
            spliceResult.valueList = newValObjls;

            return true;
        }
    }
}
