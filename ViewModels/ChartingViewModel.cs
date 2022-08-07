namespace IDMSWebServer.ViewModels
{
    public class ChartingViewModel
    {
        public bool isPreview { get; set; } = false;
        public string QueryID { get; set; } = "";
        public List<DateTime> labels { get; set; } = new List<DateTime>();
        public List<DataSet> datasets { get; set; } = new List<DataSet>();
        public double ymax { get; set; } = -1;   
        public double ymin { get; set; } = -1;


        public string AddToCache()
        {
            QueryID = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
            Models.QueryResultManager.AddResult(QueryID, this);
            return QueryID;
        }
    }

    public class DataSet
    {
        public string label { get; set; } = "";
        public List<double> data { get; set; } = new List<double>();
        public string borderColor { get; set; } = "blue";
        public string backgroundColor { get; set; } = "blue";
        public int borderWidth { get; set; } = 1;
        public string fill { get; set; } = "false";
        public string pointStyle { get; set; } = "none";
        public int pointRadius { get; set; } = 0;
        public int lineTension { get; set; } = 0;
    }
}
