namespace IDMSWebServer.Models.DataModels
{
    public class pc_information : ModelBase
    {
        public int index { get; set; }
        public string edge_ip { get; set; }
        public string idms_ip { get; set; }
        public int online_sensor_cnt { get; set; }
    }
}
