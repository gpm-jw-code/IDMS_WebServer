using System.ComponentModel.DataAnnotations;

namespace IDMSWebServer.Models.DataModels
{
    public class Vibration_Energy : ModelBase
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }
}
