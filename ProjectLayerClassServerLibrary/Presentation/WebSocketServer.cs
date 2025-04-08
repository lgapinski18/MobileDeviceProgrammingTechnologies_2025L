using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommonsMessageLibrary;
using ProjectLayerClassServerLibrary.LogicLayer;
using System.Xml;


namespace ProjectLayerClassServerLibrary.Presentation
{
    public class WebSocketServer
    {
        private Task serverLoopTask;
        private List<WebSocketConnection> connections = new();
        private ALogicLayer logicLayer;

        public bool IsRunning { get => !serverLoopTask.IsCompleted; }

        public WebSocketServer(int portNo)
        {
            logicLayer = ALogicLayer.CreateLogicLayerInstance();
            Uri _uri = new Uri($@"http://localhost:{portNo}/");
            serverLoopTask = Task.Factory.StartNew(() => ServerLoop(_uri, OnConnection));
        }

        #region private

        private static async Task ServerLoop(Uri uri, Action<WebSocketConnection> onConnection)
        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add(uri.ToString());
            server.Start();
            while (true)
            {
                HttpListenerContext httpContext = await server.GetContextAsync();
                if (!httpContext.Request.IsWebSocketRequest)
                {
                    httpContext.Response.StatusCode = 400;
                    httpContext.Response.Close();
                }
                HttpListenerWebSocketContext wsContext = await httpContext.AcceptWebSocketAsync(null);
                WebSocketConnection ws = new ServerWebSocketConnection(wsContext.WebSocket, httpContext.Request.RemoteEndPoint);
                onConnection?.Invoke(ws);
            }
        }

        private void OnConnection(WebSocketConnection connection)
        {
            connections.Add(connection);
            connection.onClose = () => connections.Remove(connection);
            connection.onError = () => Console.WriteLine("Error happened");
            connection.onMessage = message => Task.Factory.StartNew(() => ProcessConnectionMessage(connection, message));
        }

        const int MESSAGE_TYPE_POSITION = 0;
        const int MESSAGE_TYPE_LENGTH = 4;
        const int MESSAGE_SEQUENCE_NUMBER_POSITION = 4;
        const int MESSAGE_SEQUENCE_NUMBER_LENGTH = 4;
        const int MESSAGE_SIZE_POSITION = 8;
        const int MESSAGE_SIZE_LENGTH = 4;
        const int MESSAGE_CONTENT_POSITION = 12;

        private void ProcessConnectionMessage(WebSocketConnection connection, string message)
        {
            Console.WriteLine(message);


            string messageType = message.Substring(MESSAGE_TYPE_POSITION, MESSAGE_TYPE_LENGTH);
            int messageSequenceNo = BitConverter.ToInt32(Encoding.UTF8.GetBytes(message.Substring(MESSAGE_SEQUENCE_NUMBER_POSITION, MESSAGE_SEQUENCE_NUMBER_LENGTH)));
            int messageSize = BitConverter.ToInt32(Encoding.UTF8.GetBytes(message.Substring(MESSAGE_SIZE_POSITION, MESSAGE_SIZE_LENGTH)));

            Console.WriteLine($"MessageType: {messageType}, MessageSequenceNo: {messageSequenceNo}, MessageSize: {messageSize}");


            object? responseContent = null;
            XmlSerializer? serializer = null;
            string serializedMessage = "";
            int responseCode = -1;

            switch (messageType)
            {
                case "_CAO":
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "_CBA":
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "_GAO":
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GAAO":
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GBAN":
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GBAS":
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GABA":
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                default:
                    responseCode = 1;
                    break;
            }

            if (responseContent != null && serializer != null)
            {
                responseCode = 0;
                StringWriter stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, responseContent);
                serializedMessage = stringWriter.ToString();
            }
            else if (responseCode == -1)
            {
                responseCode = 2;
            }

            connection.SendAsync(messageType, messageSequenceNo, responseCode, serializedMessage).Wait();
            //_CAO - create account owner; Dane: "{ownerName};{ownerSurname};{ownerEmail};{ownerPassword}"
            //
            //_CBA - create bank account; Dane: int ownerId
            //
            //_GAO - get account owner; Dane: int ownerId
            //
            //GAAO - get all account owners; 0 danych
            //
            //GBAN - get bank account; dane: "{accountNumber}"
            //
            //GBAS - get bank accounts for owner id; dane: int owner id;
            //
            //GABA - get all accounts; 0 danych
        }

        private static T? GetData<T>(string message) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T?  deserialized = null;
            using (var reader = new StringReader(message))
            {
                deserialized = (T?)serializer.Deserialize(reader);
            }

            return deserialized;
        }

        private CreationAccountOwnerResponse? ProcessCreateAccountOwner(AccountOwnerCreationData? accountOwnerCreationData)
        {
            if (accountOwnerCreationData == null)
            {
                return null; 
            }

            AAccountOwner? accountOwner = logicLayer.CreateNewAccountOwner(accountOwnerCreationData.Name, accountOwnerCreationData.Surname, accountOwnerCreationData.Email, accountOwnerCreationData.Password, out ALogicLayer.CreationAccountOwnerFlags creationAccountOwnerFlags);

            CreationAccountOwnerResponse response = new();
            if (creationAccountOwnerFlags == ALogicLayer.CreationAccountOwnerFlags.SUCCESS)
            {
                response.AccountOwner = new AccountOwnerDto() { Id = accountOwner.GetId(), Name = accountOwner.OwnerName, Surname = accountOwner.OwnerSurname, Email = accountOwner.OwnerEmail };
                response.ResponseCodes = new() { ALogicLayer.CreationAccountOwnerFlags.SUCCESS.ToString() };
            }
            else
            {
                response.AccountOwner = null;
                response.ResponseCodes = new()
                    ;
                if ((creationAccountOwnerFlags & ALogicLayer.CreationAccountOwnerFlags.EMPTY) != 0)
                {
                    response.ResponseCodes.Add(ALogicLayer.CreationAccountOwnerFlags.EMPTY.ToString());
                }
                else if ((creationAccountOwnerFlags & ALogicLayer.CreationAccountOwnerFlags.INCORRECT_NAME) != 0)
                {
                    response.ResponseCodes.Add(ALogicLayer.CreationAccountOwnerFlags.INCORRECT_NAME.ToString());
                }
                else if ((creationAccountOwnerFlags & ALogicLayer.CreationAccountOwnerFlags.INCORRECT_SURNAME) != 0)
                {
                    response.ResponseCodes.Add(ALogicLayer.CreationAccountOwnerFlags.INCORRECT_SURNAME.ToString());
                }
                else if ((creationAccountOwnerFlags & ALogicLayer.CreationAccountOwnerFlags.INCORRECT_EMAIL) != 0)
                {
                    response.ResponseCodes.Add(ALogicLayer.CreationAccountOwnerFlags.INCORRECT_EMAIL.ToString());
                }
                else if ((creationAccountOwnerFlags & ALogicLayer.CreationAccountOwnerFlags.INCORRECT_PASSWORD) != 0)
                {
                    response.ResponseCodes.Add(ALogicLayer.CreationAccountOwnerFlags.INCORRECT_PASSWORD.ToString());
                }
            }

            return response;
        }

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

        #endregion private
    }
}
