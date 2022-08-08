namespace IDMSWebServer.Models.DataModels
{
    public class Frequency_doubling : ModelBase
    {
        public clsSeverity_Freuency_doubling severity { get; set; }
    }

    public class clsSeverity_Freuency_doubling : clsSeverity
    {
        public List<clsSideBandInfo> List_MultiFreqLog { get; set; }
    }
}
