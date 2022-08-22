using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebClientController : ControllerBase
    {
        private ILogger<WebClientController> logger;

        public WebClientController(ILogger<WebClientController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("/EdgeStatus")]
        public async Task GetEdgeStatus(string edgeIP)
        {
            IsWebsocketRequest(out bool iswebsocket, out System.Net.WebSockets.WebSocket client);
            if (iswebsocket && client.State == System.Net.WebSockets.WebSocketState.Open)
            {
                Task.Factory.StartNew(async () => await client.ReceiveAsync(new ArraySegment<byte>(new byte[3]), CancellationToken.None));
                while (client.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    await Task.Delay(1000);
                    string stateJson = Models.IDMS.DataMiddleware.Fetch.GetEdgeStatus(edgeIP);
                    if (stateJson == null)
                        continue;
                    client.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(stateJson)), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);

                }
                logger.LogInformation("websocket client close [EdgeStatus]");
            }
        }


        [HttpGet("/VibrationEnergy")]
        public async Task GetVibrationEnergy(string edgeIP, string? sensorIP)
        {
            IsWebsocketRequest(out bool iswebsocket, out System.Net.WebSockets.WebSocket client);
            if (iswebsocket && client.State == System.Net.WebSockets.WebSocketState.Open)
            {
                Task.Factory.StartNew(async () => await client.ReceiveAsync(new ArraySegment<byte>(new byte[3]), CancellationToken.None));
                while (client.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    await Task.Delay(1000);
                    string stateJson = Models.IDMS.DataMiddleware.Fetch.GetVEData(edgeIP, sensorIP);
                    if (stateJson == null)
                        continue;
                    client.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(stateJson)), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);

                }
                logger.LogInformation("websocket client close [VibrationEnergy]");
            }
        }

        [HttpGet("/ModuleInfoGet")]
        public async Task GetModuleInfo(string edgeIP)
        {
            IsWebsocketRequest(out bool iswebsocket, out System.Net.WebSockets.WebSocket client);
            if (iswebsocket && client.State == System.Net.WebSockets.WebSocketState.Open)
            {
                Task.Factory.StartNew(async () => await client.ReceiveAsync(new ArraySegment<byte>(new byte[3]), CancellationToken.None));
                while (client.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    await Task.Delay(1000);
                    string stateJson = Models.IDMS.DataMiddleware.Fetch.GetModuleStateData(edgeIP);
                    if (stateJson == null)
                        continue;
                    client.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(stateJson)), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);

                }
                logger.LogInformation("websocket client close [ModuleInfoGet]");

            }
        }

        [HttpGet("/Dignose")]
        public async Task GetDignoseData(string type, string edgeIP, string? sensorIP, string? chart_type)
        {
            IsWebsocketRequest(out bool iswebsocket, out System.Net.WebSockets.WebSocket client);
            if (iswebsocket)
            {
                logger.LogInformation("Websocket Client Connected");

                byte[] dataBuf = null;

                _ = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await client.ReceiveAsync(new ArraySegment<byte>(new byte[3]), CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                );

                while (client.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    string dignoseDataJson = null;
                    if (type == "list")
                    {
                        dignoseDataJson = Models.IDMS.DataMiddleware.Fetch.GetDignoseDataJson(edgeIP);
                    }
                    else
                    {
                        if (sensorIP == null | chart_type == null)
                            break;

                        if (chart_type == "HS")
                            dignoseDataJson = Models.IDMS.DataMiddleware.Fetch.GetHSChartingDataJsonBySensorIP(edgeIP, sensorIP);
                        if (chart_type == "AID")
                            dignoseDataJson = Models.IDMS.DataMiddleware.Fetch.GetAIDChartingDataJsonBySensorIP(edgeIP, sensorIP);
                        if (chart_type == "AIH")
                            dignoseDataJson = Models.IDMS.DataMiddleware.Fetch.GetAIHChartingDataJsonBySensorIP(edgeIP, sensorIP);

                    }

                    if (dignoseDataJson != null)
                    {
                        dataBuf = Encoding.ASCII.GetBytes(dignoseDataJson);
                    }
                    await Task.Delay(1000);
                    if (dataBuf == null)
                        continue;
                    try
                    {
                        await client.SendAsync(new ArraySegment<byte>(dataBuf), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Send Dignose Data to Websocket Client fail,{0}", ex.Message);

                    }

                }

                logger.LogInformation("websocket client close [Dignose]");
            }
        }


        private void IsWebsocketRequest(out bool isWebsocket, out System.Net.WebSockets.WebSocket client)
        {
            client = null;
            isWebsocket = HttpContext.WebSockets.IsWebSocketRequest;
            if (isWebsocket)
                client = HttpContext.WebSockets.AcceptWebSocketAsync().Result;
        }
    }
}
