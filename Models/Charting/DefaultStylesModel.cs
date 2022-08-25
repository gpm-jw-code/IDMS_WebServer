using IDMSWebServer.ViewModels;

namespace IDMSWebServer.Models.Charting
{
    public class DefaultStylesModel
    {
        public ChartStyle[] HealthScoreStyles { get; set; }
        public DefaultStylesModel()
        {
            HealthScoreStyles = DatasetFactory.GetHealthScoreSeriesDefaultProperty();
        }
    }
}
