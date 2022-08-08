namespace IDMSWebServer.Models.DataModels
{
    public class Health_Score : ModelBase
    {
        public double score { get; set; }
        public double score_wma { get; set; }
        public double alarm_threshold { get; set; }
        public double warning_threshold { get; set; }

        public string recipe_id { get; set; }

    }
}
