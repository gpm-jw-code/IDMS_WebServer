namespace IDMSWebServer.Models.IDMS
{
    public class IDMSEdgeData
    {

        public DateTime DignoseDataUpdateTime { get; private set; }
        public DateTime HSCharingDataUpdateTime { get; private set; }
        public DateTime AIHCharingDataUpdateTime { get; private set; }
        public DateTime AIDCharingDataUpdateTime { get; private set; }
        public DateTime ModuleStatesDataUpdateTime { get; private set; }
        public DateTime EdgeStatesUpdateTime { get; private set; }

        public DateTime VEWithChartingUpdataTime => VE.WithChartingUpdateTime;
        public DateTime VEWithoutChartingUpdataTime => VE.WithoutChartingUpdateTime;

        public string DignoseData { get => _DignoseData; set { _DignoseData = value; DignoseDataUpdateTime = DateTime.Now; } }
        public string HSCharingData { get => _HSCharingData; set { _HSCharingData = value; HSCharingDataUpdateTime = DateTime.Now; } }
        public string AIHCharingData { get => _AIHCharingData; set { _AIHCharingData = value; AIHCharingDataUpdateTime = DateTime.Now; } }
        public string AIDCharingData { get => _AIDCharingData; set { _AIDCharingData = value; AIDCharingDataUpdateTime = DateTime.Now; } }
        public string ModuleStatesData { get => _ModuleStatesData; set { _ModuleStatesData = value; ModuleStatesDataUpdateTime = DateTime.Now; } }
        public string EdgeStates { get => _EdgeStates; set { _EdgeStates = value; EdgeStatesUpdateTime = DateTime.Now; } }

        public clsVE VE { get; set; } = new clsVE();

        private string _DignoseData = "";
        private string _HSCharingData = "";
        private string _AIHCharingData = "";
        private string _AIDCharingData = "";
        private string _ModuleStatesData = "";
        private string _EdgeStates = "";
    }
    public class clsVE
    {
        public DateTime WithoutChartingUpdateTime { get; private set; }
        public DateTime WithChartingUpdateTime { get; private set; }
        public string WithoutCharting { get => _WithoutCharting; set { _WithoutCharting = value; WithoutChartingUpdateTime = DateTime.Now; } }
        public string WithCharting { get => _WithCharting; set { _WithCharting = value; WithChartingUpdateTime = DateTime.Now; } }


        private string _WithoutCharting = "";
        private string _WithCharting = "";
    }
}
