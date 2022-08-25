using System.Text.Json;
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

        internal void CustomStyleSettingApply(string? customSettingJson)
        {
            Dictionary<string, ChartStyle>? settingObj = new Dictionary<string, ChartStyle>();
            try
            {
                settingObj = JsonSerializer.Deserialize<Dictionary<string, ChartStyle>>(customSettingJson);
            }
            catch (Exception)
            {
            }

            var customStyle = datasets.Select(ds => ds).ToDictionary(ds => ds.label, ds => new ChartStyle()
            {
                backgroundColor = ds.backgroundColor,
                borderColor = ds.borderColor,
                borderWidth = ds.borderWidth,
                fill = ds.fill,
                lineTension = ds.lineTension,
                pointRadius = ds.pointRadius,
                pointStyle = ds.pointStyle
            });
            foreach (var styleSet in settingObj)
                customStyle[styleSet.Key] = styleSet.Value;


            ///同步
            foreach (var dataset in datasets)
            {
                var settings = customStyle[dataset.label];
                dataset.backgroundColor = settings.backgroundColor;
                dataset.borderColor = settings.borderColor;
                dataset.borderWidth = settings.borderWidth;
                dataset.fill = settings.fill;
                dataset.lineTension = settings.lineTension;
                dataset.pointRadius = settings.pointRadius;
                dataset.pointStyle = settings.pointStyle;
            }
            //throw new NotImplementedException();
        }
    }

    public class DataSet : ChartStyle
    {
        public string label { get; set; } = "";
        public List<double> data { get; set; } = new List<double>();
    }

    public class ChartStyle
    {
        public string borderColor { get; set; } = "blue";
        public string backgroundColor { get; set; } = "blue";
        public int borderWidth { get; set; } = 1;
        public string fill { get; set; } = "false";
        public string pointStyle { get; set; } = "none";
        public int pointRadius { get; set; } = 0;
        public int lineTension { get; set; } = 0;
    }
}
