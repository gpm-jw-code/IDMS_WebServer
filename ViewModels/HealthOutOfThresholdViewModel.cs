namespace IDMSWebServer.ViewModels
{
    public class HealthOutOfThresholdViewModel : DataViewModelBase
    {
        public bool OutOfWarningThreshold { get; set; }
        public bool OutOfAlarmThreshold { get; set; }
        public double Warning_Threshold { get; set; }
        public double Alarm_Threshold { get; set; }

        public string ModelName { get; set; }
    }
}
