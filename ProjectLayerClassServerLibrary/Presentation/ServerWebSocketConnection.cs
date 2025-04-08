using System.Net.WebSockets;
using System.Net;
using System.Text;


namespace ProjectLayerClassServerLibrary.Presentation
{
    public partial class WebSocketServer
    {
        private class ServerWebSocketConnection : WebSocketConnection
        {
            private Task webSocketServerLoop;
            public ServerWebSocketConnection(WebSocket webSocket, IPEndPoint remoteEndPoint)
            {
                this.webSocket = webSocket;
                this.remoteEndPoint = remoteEndPoint;
                webSocketServerLoop = Task.Factory.StartNew(() => ServerMessageLoop(webSocket));
            }

            #region WebSocketConnection

            protected override Task SendTask(byte[] header, string message)
            {
                return webSocket.SendAsync(new ArraySegment<byte>([.. header, .. Encoding.UTF8.GetBytes(message)]), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            public override Task DisconnectAsync()
            {
                return webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown procedure started", CancellationToken.None);
            }

            #endregion WebSocketConnection

            #region Object

            public override string ToString()
            {
                return remoteEndPoint.ToString();
            }

            #endregion Object

            private WebSocket webSocket = null;
            private IPEndPoint remoteEndPoint;

            private void ServerMessageLoop(WebSocket ws)
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    ArraySegment<byte> _segments = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult _receiveResult = ws.ReceiveAsync(_segments, CancellationToken.None).Result;
                    if (_receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        onClose?.Invoke();
                        ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None);
                        return;
                    }
                    int count = _receiveResult.Count;
                    while (!_receiveResult.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            onClose?.Invoke();
                            ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                            return;
                        }
                        _segments = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        _receiveResult = ws.ReceiveAsync(_segments, CancellationToken.None).Result;
                        count += _receiveResult.Count;
                    }
                    string _message = Encoding.UTF8.GetString(buffer, 0, count);
                    onMessage?.Invoke(_message);
                }
            }
        }

    }
}
