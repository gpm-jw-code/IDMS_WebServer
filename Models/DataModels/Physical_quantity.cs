namespace IDMSWebServer.Models.DataModels
{
    public class Physical_quantity : ModelBase
    {
        public double xdisplacement { get; set; }
        public double ydisplacement { get; set; }
        public double zdisplacement { get; set; }
        public double xvelocity { get; set; }
        public double yvelocity { get; set; }
        public double zvelocity { get; set; }
        public double xacceleration_peak_to_peak { get; set; }
        public double yacceleration_peak_to_peak { get; set; }
        public double zacceleration_peak_to_peak { get; set; }

    }
}
