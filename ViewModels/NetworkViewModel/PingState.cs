namespace IDMSWebServer.ViewModels.NetworkViewModel
{
    public class PingState
    {

        public PingState(string IP)
        {
            this.IP = IP;
        }
        public string IP { get; set; }
        public bool Success{ get; set; }
        public int PingTime { get; set; }
    }
}
