namespace IDMSWebServer.Models
{
    public class QueryResultManager
    {

        public static Dictionary<string, clsQueryResult> QueryResultCaches = new Dictionary<string, clsQueryResult>();


        public static void AddResult(string queryID, clsQueryResult result)
        {
            QueryResultCaches.Add(queryID, result);
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
