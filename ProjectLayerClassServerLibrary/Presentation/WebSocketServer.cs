using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProjectLayerClassServerLibrary.LogicLayer;
using ProjectLayerClassServerLibrary.Presentation.Message;
using System.Xml;


namespace ProjectLayerClassServerLibrary.Presentation
{
    public partial class WebSocketServer
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
                    //_CAO - create account owner; Dane: "{ownerName};{ownerSurname};{ownerEmail};{ownerPassword}"
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "_CBA":
                    //_CBA - create bank account; Dane: int ownerId
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "_GAO":
                    //_GAO - get account owner; Dane: int ownerId
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GAAO":
                    //GAAO - get all account owners; 0 danych
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GBAN":
                    //GBAN - get bank account; dane: "{accountNumber}"
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GBAS":
                    //GBAS - get bank accounts for owner id; dane: int owner id;
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(message.Substring(MESSAGE_CONTENT_POSITION, messageSize)));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GABA":
                    //GABA - get all accounts; 0 danych
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

        #endregion private
    }
}
