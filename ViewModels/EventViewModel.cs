namespace IDMSWebServer.ViewModels
{
    public class EventViewModel
    {
        public int TotalNum { get; set; }
        public int Page { get; set; }
        public string SensorIP { get; set; }
        public string EQName { get; set; }
        public string UnitName { get; set; }
        public DateTime TimeStamp { get; set; }

        public object Data { get; set; }
        public List<EltableColumn> ElTableColumns { get; set; }
    }

    public class EltableColumn
    {
        public string label { get; set; }
        public string prop { get; set; }
        public string type { get; set; } = "string";

        public string bindingStyle { get; set; } = "";

    }
}
