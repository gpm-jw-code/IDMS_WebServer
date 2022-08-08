namespace IDMSWebServer.ViewModels
{
    public class SideBandSeverityOutOfThresHoldViewModel : DataViewModelBase
    {
        public double ThresHold_X { get; set; }
        public double ThresHold_Y { get; set; }
        public double ThresHold_Z { get; set; }

        public bool Is_Outof_ThresHold_X { get; set; }
        public bool Is_Outof_ThresHold_Y { get; set; }
        public bool Is_Outof_ThresHold_Z { get; set; }
    }
}
