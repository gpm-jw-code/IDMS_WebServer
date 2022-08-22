namespace IDMSWebServer.Models.IDMS
{
    public class IDMSEdgeData
    {
        public string DignoseData { get; set; }
        public string HSCharingData { get; internal set; }
        public string AIHCharingData { get; internal set; }
        public string AIDCharingData { get; internal set; }
        public string ModuleStatesData { get; internal set; }
    }
}
