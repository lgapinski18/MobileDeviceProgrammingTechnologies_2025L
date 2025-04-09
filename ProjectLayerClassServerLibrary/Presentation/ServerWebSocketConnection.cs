using System.Net.WebSockets;
using System.Net;
using System.Text;
using ProjectLayerClassServerLibrary.Presentation.Implementations;


namespace ProjectLayerClassServerLibrary.Presentation
{
    internal class ServerWebSocketConnection : WebSocketConnection
    {
        private Task webSocketServerLoop;
        public ServerWebSocketConnection(WebSocket webSocket, IPEndPoint remoteEndPoint)
        {
            this.webSocket = webSocket;
            this.remoteEndPoint = remoteEndPoint;
            webSocketServerLoop = Task.Factory.StartNew(() => ServerMessageLoop(webSocket));
        }
    
        protected override Task SendTask(byte[] header, string message)
        {
            return webSocket.SendAsync(new ArraySegment<byte>([.. header, .. Encoding.UTF8.GetBytes(message)]), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public override Task DisconnectAsync()
        {
            return webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown procedure started", CancellationToken.None);
        }

        public override string ToString()
        {
            return remoteEndPoint.ToString();
        }
    
        private WebSocket webSocket = null;
        private IPEndPoint remoteEndPoint;
        private int? loggedOwnerId = null;

        public override int? LoggedOwnerId { get => loggedOwnerId; set => loggedOwnerId = value; }

        private void ServerMessageLoop(WebSocket ws)
        {
            try
            {
                byte[] buffer = new byte[4096];
                while (true)
                {
                    ArraySegment<byte> segments = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult receiveResult = ws.ReceiveAsync(segments, CancellationToken.None).Result;
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        onClose?.Invoke();
                        ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None);
                        return;
                    }
                    int count = receiveResult.Count;
                    while (!receiveResult.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            onClose?.Invoke();
                            ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                            return;
                        }
                        segments = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        receiveResult = ws.ReceiveAsync(segments, CancellationToken.None).Result;
                        count += receiveResult.Count;
                    }
                    onMessage?.Invoke(buffer);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Connection has been broken because of an exception {_ex}");
                ws.CloseAsync(WebSocketCloseStatus.InternalServerError, "Connection has been broken because of an exception", CancellationToken.None).Wait();
            }
        }
    }
}
