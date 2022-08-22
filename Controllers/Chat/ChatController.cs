using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IDMSWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        public static List<WebsocketClient> onlineClients = new List<WebsocketClient>();

        public ILogger logger;
        public ChatController(ILogger<ChatController> logger)
        {
            this.logger = logger;
        }
        private WebsocketClient user;

        [HttpGet("/chat")]
        public async Task Chat(string name)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var userName = name;
                System.Net.WebSockets.WebSocket? ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
                user = new WebsocketClient(ws)
                {
                    name = name
                };
                onlineClients.Add(user);

                while (ws.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    byte[] buf = new byte[8192];
                    await ws.ReceiveAsync(new ArraySegment<byte>(buf), CancellationToken.None);
                    var userInput = Encoding.UTF8.GetString(buf).Replace("\0", "");
                    if (userInput == "")
                        continue;
                    BrocastMessageOutAsync(userInput);
                    logger.LogInformation("{0} Say:{1}", userName, userInput);
                }
            }
        }


        private async Task BrocastMessageOutAsync(string msg)
        {
            await Task.Delay(1);
            Message msgObj = new Message()
            {
                from = user.name,
                id = user.id,
                message = msg,
                time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

            };
            foreach (var client in onlineClients.FindAll(cl => cl.id != user.id).ToList())
            {
                client.SendMsgAsync(msgObj);
            }
        }

    }

    public class Message
    {
        public string time { get; set; }
        public string id { get; set; }
        public string from { get; set; }
        public string message { get; set; }
    }

    public class WebsocketClient
    {
        private System.Net.WebSockets.WebSocket ws;
        public string id { get; private set; }
        public string name { get; set; } = "";

        public WebsocketClient(System.Net.WebSockets.WebSocket ws)
        {
            this.ws = ws;
            this.id = DateTime.Now.Ticks + "";
        }
        public async Task SendMsgAsync(string mesg)
        {
            await Task.Delay(1);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(mesg)), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
        }

        internal async Task SendMsgAsync(Message msgObj)
        {
            await Task.Delay(1);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msgObj))), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);

        }
    }
}
