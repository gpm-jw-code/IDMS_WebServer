using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace IDMSWebServer.Controllers.NetworkTools
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {

        [HttpGet("/ping-ip")]
        public async Task PingIP(string ip)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {

                WebSocket client = await HttpContext.WebSockets.AcceptWebSocketAsync();

                while (client.State == WebSocketState.Open)
                {
                    Ping ping = new Ping();
                    PingReply? reply = ping.Send(ip);
                    bool success = reply.Status == IPStatus.Success;
                    var time = reply.RoundtripTime;
                    ViewModels.NetworkViewModel.PingState pingState = new ViewModels.NetworkViewModel.PingState(ip);
                    pingState.Success = success;
                    pingState.PingTime = success ? (int)time : -1;
                    byte[] data = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(pingState));
                    try
                    {
                        client.ReceiveAsync(new ArraySegment<byte>(new byte[1]), CancellationToken.None);
                        await client.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace + "\r\n" + ex.Message);
                        break;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }


            }
        }
    }
}
