using System.Text.Json;

namespace IDMSWebServer.Models.IDMS
{
    public class DataMiddleware
    {
        public static Dictionary<string, IDMSEdgeData> EdgeDatas = new Dictionary<string, IDMSEdgeData>();


        private static void InitializeEdge(string edgeIP)
        {
            if (!EdgeDatas.ContainsKey(edgeIP))
                EdgeDatas.Add(edgeIP, new IDMSEdgeData());
        }
        public struct Fetch
        {
            internal static string GetDignoseDataJson(string edgeIP)
            {
                if (EdgeDatas.TryGetValue(edgeIP, out IDMSEdgeData edge))
                {
                    return edge.DignoseData;
                }
                else
                {
                    return null;
                }
            }

            internal static string GetModuleStateData(string edgeIP)
            {
                if (EdgeDatas.TryGetValue(edgeIP, out IDMSEdgeData edge))
                {
                    return edge.ModuleStatesData;
                }
                else
                    return null;
            }

            internal static string GetEdgeStatus(string edgeIP)
            {
                if (EdgeDatas.TryGetValue(edgeIP, out IDMSEdgeData edge))
                {
                    return edge.EdgeStates;
                }
                else
                    return null;
            }

            internal static string GetVEData(string edgeIP, string sensorIP)
            {
                if (EdgeDatas.TryGetValue(edgeIP, out IDMSEdgeData edge))
                {

                    if (sensorIP == null)
                    {
                        return edge.VE.WithoutCharting;
                    }
                    else
                    {
                        var objs = JsonSerializer.Deserialize<Dictionary<string, Object>>(edge.VE.WithCharting);
                        if (objs.TryGetValue(sensorIP, out object json))
                        {
                            return JsonSerializer.Serialize(json);
                        }
                        else
                            return null;

                    }
                }
                else
                    return null;
            }

            internal static string? GetHSChartingDataJsonBySensorIP(string edgeIP, string sensorIP)
            {
                if (EdgeDatas.TryGetValue(edgeIP, out IDMSEdgeData edge))
                {
                    if (edge.HSCharingData == null)
                        return null;
                    if (sensorIP.ToUpper() == "ALL")
                        return edge.HSCharingData;
                    var obj = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(edge.HSCharingData);
                    var ob = obj.FirstOrDefault(o => o["IP"].ToString() == sensorIP);
                    return JsonSerializer.Serialize(ob);
                }
                else
                {
                    return null;
                }
            }

            internal static string? GetAIHChartingDataJsonBySensorIP(string edgeIP, string sensorIP)
            {
                if (EdgeDatas.TryGetValue(edgeIP, out IDMSEdgeData edge))
                {
                    if (edge.AIHCharingData == null)
                        return null;
                    if (sensorIP.ToUpper() == "ALL")
                        return edge.AIHCharingData;

                    var obj = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(edge.AIHCharingData);
                    var vm  = obj.FirstOrDefault(o=>o["IP"].ToString()==sensorIP);
                    return vm==null? null : JsonSerializer.Serialize(vm);
                }
                else
                {
                    return null;
                }
            }

            internal static string? GetAIDChartingDataJsonBySensorIP(string edgeIP, string sensorIP)
            {
                if (EdgeDatas.TryGetValue(edgeIP, out IDMSEdgeData edge))
                {
                    if (edge.AIDCharingData == null)
                        return null;
                    if (sensorIP.ToUpper() == "ALL")
                        return edge.AIDCharingData;
                    var obj = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(edge.AIDCharingData);
                    var vm = obj.FirstOrDefault(o => o["IP"].ToString() == sensorIP);
                    return vm == null ? null : JsonSerializer.Serialize(vm);
                }
                else
                {
                    return null;
                }
            }
        }
        public struct Update
        {
            internal static void UpdateDignoseData(string edgeIP, string jsonStr)
            {
                InitializeEdge(edgeIP);
                EdgeDatas[edgeIP].DignoseData = jsonStr;
            }

            internal static void UpdateDignoseHSCharingData(string edgeIP, string jsonStr)
            {
                InitializeEdge(edgeIP);
                EdgeDatas[edgeIP].HSCharingData = jsonStr;
            }

            internal static void UpdateDignoseAIDCharingData(string edgeIP, string jsonStr)
            {
                InitializeEdge(edgeIP);
                EdgeDatas[edgeIP].AIDCharingData = jsonStr;
            }

            internal static void UpdateDignoseAIHSCharingData(string edgeIP, string jsonStr)
            {
                InitializeEdge(edgeIP);
                EdgeDatas[edgeIP].AIHCharingData = jsonStr;
            }

            internal static void UpdateModuleStatesData(string edgeIP, string jsonstr)
            {
                InitializeEdge(edgeIP);
                EdgeDatas[edgeIP].ModuleStatesData = jsonstr;
            }

            internal static void UpdateVEDataWithCharting(string edgeIP, string jsonstr)
            {
                InitializeEdge(edgeIP);
                EdgeDatas[edgeIP].VE.WithCharting = jsonstr;
            }
            internal static void UpdateVEDataWithoutCharting(string edgeIP, string jsonstr)
            {
                InitializeEdge(edgeIP);
                EdgeDatas[edgeIP].VE.WithoutCharting = jsonstr;
            }

            internal static void UpdateEdgeStates(string edgeIP, string jsonStr)
            {
                InitializeEdge(edgeIP);
                EdgeDatas[edgeIP].EdgeStates = jsonStr;
            }
        }

    }
}
