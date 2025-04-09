using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibraryTest.DataLayerTest.TestUtilities
{
    internal static class TestWebSocketServer
    {
        #region API

        public static async Task Server(int p2p_port, Action<TestWebSocketConnection> onConnection)
        {
            Uri _uri = new Uri($@"http://localhost:{p2p_port}/");
            await ServerLoop(_uri, onConnection);
        }

        #endregion API

        #region private

        private static async Task ServerLoop(Uri _uri, Action<TestWebSocketConnection> onConnection)
        {
            HttpListener _server = new HttpListener();
            _server.Prefixes.Add(_uri.ToString());
            _server.Start();
            while (true)
            {
                HttpListenerContext _hc = await _server.GetContextAsync();
                if (!_hc.Request.IsWebSocketRequest)
                {
                    _hc.Response.StatusCode = 400;
                    _hc.Response.Close();
                }
                HttpListenerWebSocketContext _context = await _hc.AcceptWebSocketAsync(null);
                TestWebSocketConnection ws = new ServerWebSocketConnection(_context.WebSocket, _hc.Request.RemoteEndPoint);
                onConnection?.Invoke(ws);
            }
        }

        private class ServerWebSocketConnection : TestWebSocketConnection
        {
            public ServerWebSocketConnection(WebSocket webSocket, IPEndPoint remoteEndPoint)
            {
                m_WebSocket = webSocket;
                m_remoteEndPoint = remoteEndPoint;
                Task.Factory.StartNew(() => ServerMessageLoop(webSocket));
            }

            #region WebSocketConnection

            protected override Task SendTask(byte[] message)
            {
                return m_WebSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Binary, true, CancellationToken.None);
            }

            public override Task DisconnectAsync()
            {
                return m_WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown procedure started", CancellationToken.None);
            }

            #endregion WebSocketConnection

            #region Object

            public override string ToString()
            {
                return m_remoteEndPoint.ToString();
            }

            #endregion Object

            private WebSocket m_WebSocket = null;
            private IPEndPoint m_remoteEndPoint;

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
                    onMessage?.Invoke(buffer, count);
                }
            }
        }

        #endregion private
    }
}
