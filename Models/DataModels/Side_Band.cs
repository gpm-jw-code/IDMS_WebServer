using System.ComponentModel.DataAnnotations;

namespace IDMSWebServer.Models.DataModels
{
    public class Side_Band : ModelBase
    {
        public clsSeverity severity { get; set; }
    }

    public class clsSeverity
    {
        public string SensorID { get; set; }
        public string EQName { get; set; }
        public string UnitName { get; set; }
        public DateTime TimeStamp { get; set; }

        public List<clsSideBandInfo> List_SideBandInfo { get; set; }
        public clsThreshold Threshold { get; set; }
    }
    public class clsSideBandInfo
    {
        public string AxisName { get; set; }
        public double Severity { get; set; }
    }

    public class clsThreshold
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double All { get; set; }

        public bool X_IsOutofThreshold { get; set; }
        public bool Y_IsOutofThreshold { get; set; }
        public bool Z_IsOutofThreshold { get; set; }
    }
}
