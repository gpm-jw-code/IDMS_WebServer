namespace IDMSWebServer.Models
{
    public class clsModuleInfo
    {
        public string IP { get; set; }
        public string EQ { get; set; }
        public string UNIT { get; set; }

        public clsModuleInfo(string IP, string EQ, string UNIT)
        {
            this.IP = IP;
            this.EQ = EQ;
            this.UNIT = UNIT;
        }
    }
}
