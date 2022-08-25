using IDMSWebServer.ViewModels;

namespace IDMSWebServer.Models.Charting
{
    public class DatasetFactory
    {

        public static ChartStyle[] GetHealthScoreSeriesDefaultProperty()
        {
            return CreateHealthScoreDatasets(null, null).Select(ds => ds.styles).ToArray();
        }

        internal static List<DataSet> CreateHealthScoreDatasets(List<double>? ScoreDataList, List<double>? ScoreWMADataList)
        {
            DataSet scoreWmaDataSet = new DataSet
            {
                label = "Score_WMA",
                data = ScoreWMADataList,
                borderColor = "rgb(255,255,255)",
            };
            DataSet scoreDataSet = new DataSet
            {
                label = "Score",
                data = ScoreDataList,
                borderColor = "rgb(30, 94, 197)",
            };
            return new List<DataSet> { scoreWmaDataSet, scoreDataSet };
        }

        internal static List<DataSet> CreateAlertIndexDatasets(List<double> AlertIndexDataList)
        {
            return new List<DataSet> { new ViewModels.DataSet
            {
                label = "Alert Index",
                data =AlertIndexDataList,
                borderColor = "orange",
            }};
        }

        internal static List<DataSet> CreateFrequencyDoublingSeverityDatasets(List<double> XDataList, List<double> YDataList, List<double> ZDataList)
        {
            DataSet xDataSet = new DataSet
            {
                label = "Frequency_Doubling.Severity.X",
                data = XDataList,
                borderColor = "rgb(13, 110, 253)",
            };
            DataSet yDataSet = new DataSet
            {
                label = "Frequency_Doubling.Severity.Y",
                data = YDataList,
                borderColor = "rgb(48, 239, 149)",
            };

            DataSet zDataSet = new DataSet
            {
                label = "Frequency_Doubling.Severity.Z",
                data = ZDataList,
                borderColor = "rgb(245, 62, 62)",
            };
            return new List<DataSet> { xDataSet, yDataSet, zDataSet };
        }


        internal static List<DataSet> CreateSideBandSeverityDatasets(List<double> XDataList, List<double> YDataList, List<double> ZDataList)
        {
            DataSet xDataSet = new DataSet
            {
                label = "SideBand.Severity.X",
                data = XDataList,
                borderColor = "rgb(13, 110, 253)",
            };
            DataSet yDataSet = new DataSet
            {
                label = "SideBand.Severity.Y",
                data = YDataList,
                borderColor = "rgb(48, 239, 149)",
            };

            DataSet zDataSet = new DataSet
            {
                label = "SideBand.Severity.Z",
                data = ZDataList,
                borderColor = "rgb(245, 62, 62)",
            };
            return new List<DataSet> { xDataSet, yDataSet, zDataSet };
        }



        internal static List<DataSet> CreatePhysicalQuantityDatasets(
            List<double> AccXDataList, List<double> AccYDataList, List<double> AccZDataList,
            List<double> VelXDataList, List<double> VelYDataList, List<double> VelZDataList,
            List<double> DisXDataList, List<double> DisYDataList, List<double> DisZDataList)
        {
            DataSet AccxDataSet = new DataSet
            {
                label = "加速度-P2P.X",
                data = AccXDataList,
                borderColor = "rgb(13, 110, 253)",
            };
            DataSet AccyDataSet = new DataSet
            {
                label = "加速度-P2P.Y",
                data = AccYDataList,
                borderColor = "rgb(48, 239, 149)",
            };

            DataSet AcczDataSet = new DataSet
            {
                label = "加速度-P2P.Z",
                data = AccZDataList,
                borderColor = "rgb(245, 62, 62)",
            };

            DataSet VelxDataSet = new DataSet
            {
                label = "速度.X",
                data = VelXDataList,
                borderColor = "rgb(13, 110, 253)",
            };
            DataSet VelyDataSet = new DataSet
            {
                label = "速度.Y",
                data = VelYDataList,
                borderColor = "rgb(48, 239, 149)",
            };

            DataSet VelzDataSet = new DataSet
            {
                label = "速度.Z",
                data = VelZDataList,
                borderColor = "rgb(245, 62, 62)",
            };

            DataSet DisxDataSet = new DataSet
            {
                label = "位移.X",
                data = DisXDataList,
                borderColor = "rgb(13, 110, 253)",
            };
            DataSet DisyDataSet = new DataSet
            {
                label = "位移.Y",
                data = DisYDataList,
                borderColor = "rgb(48, 239, 149)",
            };

            DataSet DiszDataSet = new DataSet
            {
                label = "位移.Z",
                data = DisZDataList,
                borderColor = "rgb(245, 62, 62)",
            };


            return new List<DataSet> {
                AccxDataSet, AccyDataSet, AcczDataSet,
                VelxDataSet, VelyDataSet, VelzDataSet,
                DisxDataSet, DisyDataSet, DiszDataSet
            };
        }


        internal static List<DataSet> CreateVibrationEnergyDatasets(List<double> XDataList, List<double> YDataList, List<double> ZDataList)
        {
            DataSet xDataSet = new DataSet
            {
                label = "X",
                data = XDataList,
                borderColor = "rgb(13, 110, 253)",
            };
            DataSet yDataSet = new DataSet
            {
                label = "Y",
                data = YDataList,
                borderColor = "rgb(48, 239, 149)",
            };

            DataSet zDataSet = new DataSet
            {
                label = "Z",
                data = ZDataList,
                borderColor = "rgb(245, 62, 62)",
            };
            return new List<DataSet> { xDataSet, yDataSet, zDataSet };
        }
    }

}
