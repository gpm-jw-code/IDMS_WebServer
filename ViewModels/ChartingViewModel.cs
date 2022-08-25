using System.Text.Json;
namespace IDMSWebServer.ViewModels
{
    public class ChartingViewModel
    {
        public bool isPreview { get; set; } = false;
        public string QueryID { get; set; } = "";
        public List<DateTime> labels { get; set; } = new List<DateTime>();
        public List<DataSet> datasets
        {
            get => _datasets;
            set
            {
                _datasets = value;
                //統計最大/小值
                if (_datasets.All(da => da.data.Count == 0))
                    return;
                ymaxOfAllData = _datasets.Select(da => da.data.Max()).Max();
                yminOfAllData = _datasets.Select(da => da.data.Min()).Min();
            }
        }
        public double ymax { get; set; } = -1;
        public double ymin { get; set; } = -1;

        public double ymaxOfAllData { get; set; }
        public double yminOfAllData { get; set; }

        private List<DataSet> _datasets = new List<DataSet>();

        public string AddToCache()
        {
            QueryID = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
            Models.QueryResultManager.AddResult(QueryID, this);
            return QueryID;
        }

        internal void CustomStyleSettingApply(string? customSettingJson)
        {
            if (customSettingJson == null)
                return;
            Dictionary<string, ChartStyle>? settingObj = new Dictionary<string, ChartStyle>();
            try
            {
                settingObj = JsonSerializer.Deserialize<Dictionary<string, ChartStyle>>(customSettingJson);
            }
            catch (Exception)
            {
            }

            var customStyle = datasets.Select(ds => ds).ToDictionary(ds => ds.label, ds => ds.styles);
            foreach (var styleSet in settingObj)
                customStyle[styleSet.Key] = styleSet.Value;


            ///同步
            foreach (var dataset in datasets)
            {
                var settings = customStyle[dataset.label];
                dataset.styles = settings;
            }
            //throw new NotImplementedException();
        }
    }

    public class DataSet : ChartStyle
    {
        public List<double> data { get; set; } = new List<double>();

        public ChartStyle styles
        {
            set
            {
                label = value.label;
                borderColor = value.borderColor;
                fill = value.borderColor;
                lineTension = value.lineTension;
                pointRadius = value.pointRadius;
                pointStyle = value.pointStyle;
                borderWidth = value.borderWidth;
            }
            get
            {
                return new ChartStyle()
                {
                    label = label,
                    borderColor = borderColor,
                    fill = borderColor,
                    lineTension = lineTension,
                    pointRadius = pointRadius,
                    pointStyle = pointStyle,
                    borderWidth = borderWidth,
                };
            }
        }
    }

    public class ChartStyle
    {
        private string _borderColor = "blue";
        public string borderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = backgroundColor = value;
            }
        }
        public string label { get; set; } = "";
        public string backgroundColor { get; private set; } = "blue";
        public int borderWidth { get; set; } = 1;
        public string fill { get; set; } = "false";
        public string pointStyle { get; set; } = "none";
        public int pointRadius { get; set; } = 0;
        public int lineTension { get; set; } = 0;
    }
}
