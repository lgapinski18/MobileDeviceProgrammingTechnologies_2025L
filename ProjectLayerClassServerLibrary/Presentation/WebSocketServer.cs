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
using System.Security.Principal;


namespace ProjectLayerClassServerLibrary.Presentation
{
    internal class WebSocketServer : IWebSocketServer
    {
        private Task serverLoopTask;
        private List<WebSocketConnection> connections = new();
        private ALogicLayer logicLayer;

        public bool IsRunning { get => !serverLoopTask.IsCompleted; }

        public WebSocketServer(int portNo, ALogicLayer logicLayer)
        {
            this.logicLayer = logicLayer;
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
            string messageContent = message.Substring(MESSAGE_CONTENT_POSITION, messageSize);

            Console.WriteLine($"MessageType: {messageType}, MessageSequenceNo: {messageSequenceNo}, MessageSize: {messageSize}\nMessageContent:\n{messageContent}\n");


            object? responseContent = null;
            XmlSerializer? serializer = null;
            string serializedMessage = "";
            int responseCode = -1;

            switch (messageType)
            {
                case "_AAO":
                    //AAO - authethicate account owner, data: string login, string password
                    responseContent = ProcessAuthenthicateAccountOwner(GetData<Credentials>(messageContent));
                    serializer = new XmlSerializer(typeof(bool)); //////////
                    break;

                case "_CAO":
                    //_CAO - create account owner; Dane: "{ownerName};{ownerSurname};{ownerEmail};{ownerPassword}"
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(messageContent));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "_CBA":
                    //_CBA - create bank account; Dane: int ownerId
                    responseContent = ProcessCreateBankAccount(GetInt(messageContent));
                    serializer = new XmlSerializer(typeof(BankAccountDto)); //////////
                    break;

                case "_GAO":
                    //_GAO - get account owner; Dane: int ownerId
                    responseContent = ProcessGetAccountOwner(GetInt(messageContent));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "GAAO":
                    //GAAO - get all account owners; 0 danych
                    responseContent = ProcessGetAllAccountOwners();
                    serializer = new XmlSerializer(typeof(List<AccountOwnerDto>));
                    break;

                case "GBAN":
                    //GBAN - get bank account; dane: "{accountNumber}"
                    responseContent = ProcessGetBankAccountByNumber(GetData<string>(messageContent));
                    serializer = new XmlSerializer(typeof(BankAccountDto)); /////////
                    break;

                case "GBAS":
                    //GBAS - get bank accounts for owner id; dane: int owner id;
                    responseContent = ProcessGetBankAccounts(GetInt(messageContent));
                    serializer = new XmlSerializer(typeof(List<BankAccountDto>)); ////////
                    break;

                case "GABA":
                    //GABA - get all accounts; 0 danych
                    responseContent = ProcessGetAllBankAccounts();
                    serializer = new XmlSerializer(typeof(List<BankAccountDto>)); 
                    break;

                case "GAOL":
                    //GAOL - get account owner by login, data: string login
                    responseContent = ProcessGetAccountOwnerByLogin(GetData<string>(messageContent));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    break;

                case "CFRU":
                    //CFRU - check for reports updates, data: int ownerId
                    responseContent = ProcessCheckForReportsUpdate(GetInt(messageContent));
                    serializer = new XmlSerializer(typeof(bool)); ///////
                    break;

                case "___T":
                    //___T - string ownerAccountNumber, string targetAccountNumber, float amount, string description,
                    TransferResultCodes? transferResponseCode = ProcessTransfer(GetData<TransferData>(messageContent));
                    if (transferResponseCode != null)
                    {
                        responseContent = transferResponseCode.Value;
                    }
                    serializer = new XmlSerializer(typeof(TransferResultCodes));
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

        private static int? GetInt(string message) 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(int));
            int? deserialized = null;
            using (var reader = new StringReader(message))
            {
                deserialized = (int?)serializer.Deserialize(reader);
            }

            return deserialized;
        }

        private bool? ProcessAuthenthicateAccountOwner(Credentials? credentials)
        {
            if (credentials == null)
            {
                return null;
            }
            return logicLayer.AuthenticateAccountOwner(credentials.Login, credentials.Password);
        }

        private CreationAccountOwnerResponse? ProcessCreateAccountOwner(AccountOwnerCreationData? accountOwnerCreationData)
        {
            if (accountOwnerCreationData == null)
            {
                return null; 
            }

            AAccountOwner? accountOwner = 
                logicLayer.CreateNewAccountOwner(accountOwnerCreationData.Name, 
                                                 accountOwnerCreationData.Surname,
                                                 accountOwnerCreationData.Email,
                                                 accountOwnerCreationData.Password,
                                                 out ALogicLayer.CreationAccountOwnerFlags creationAccountOwnerFlags);

            CreationAccountOwnerResponse response = new();
            response.CreationFlags = (CreationAccountOwnerDataLayerFlags)(int)creationAccountOwnerFlags;
            if (accountOwner != null)
            {
                response.AccountOwner = new AccountOwnerDto() { 
                    Id = accountOwner.GetId(), 
                    Name = accountOwner.OwnerName, 
                    Surname = accountOwner.OwnerSurname,
                    Email = accountOwner.OwnerEmail 
                };
            }
            else
            {
                response.AccountOwner = null;
            }

            return response;
        }

        private List<AccountOwnerDto> ProcessGetAllAccountOwners()
        {
            return logicLayer.GetAllAccountsOwners()
                                .Select(accountOwner => new  AccountOwnerDto() { 
                                    Id = accountOwner.GetId(), 
                                    Name = accountOwner.OwnerName, 
                                    Surname = accountOwner.OwnerSurname,
                                    Email = accountOwner.OwnerEmail 
                                })
                                .ToList();
        }

        private List<BankAccountDto> ProcessGetAllBankAccounts()
        {
            return logicLayer.GetAllBankAccounts()
                                .Select(bankAccount => new BankAccountDto()
                                {
                                    Id = bankAccount.GetId(),
                                    OwnerId = bankAccount.AccountOwner.GetId(),
                                    AccountNumber = bankAccount.AccountNumber,
                                    Balance = bankAccount.AccountBalance,
                                })
                                .ToList();
        }

        private AccountOwnerDto? ProcessGetAccountOwner(int? ownerId)
        {
            if (ownerId == null)
            {
                return null;
            }
            AAccountOwner? accountOwner = logicLayer.GetAccountOwner(ownerId.Value);
            if (accountOwner == null)
            {
                return null;
            }
            return new AccountOwnerDto()
            {
                Id = accountOwner.GetId(),
                Name = accountOwner.OwnerName,
                Surname = accountOwner.OwnerSurname,
                Email = accountOwner.OwnerEmail
            };
        }

        private AccountOwnerDto? ProcessGetAccountOwnerByLogin(string? login)
        {
            if (login == null)
            {
                return null;
            }
            AAccountOwner? accountOwner = logicLayer.GetAccountOwner(login);
            if (accountOwner == null)
            {
                return null;
            }
            return new AccountOwnerDto()
            {
                Id = accountOwner.GetId(),
                Name = accountOwner.OwnerName,
                Surname = accountOwner.OwnerSurname,
                Email = accountOwner.OwnerEmail
            };
        }

        private BankAccountDto? ProcessCreateBankAccount(int? ownerId)
        {
            if (ownerId == null)
            {
                return null;
            }
            ABankAccount? bankAccount = logicLayer.OpenNewBankAccount(ownerId.Value);
            if (bankAccount == null)
            {
                return null;
            }
            return new BankAccountDto()
            { 
                Id = bankAccount.GetId(),
                OwnerId = bankAccount.AccountOwner.GetId(),
                AccountNumber = bankAccount.AccountNumber,
                Balance = bankAccount.AccountBalance, 
            };
        }

        private List<BankAccountDto> ProcessGetBankAccounts(int? ownerId)
        {
            if (ownerId == null)
            {
                return null;
            }
            return logicLayer.GetAccountOwnerBankAccounts(ownerId.Value)
                                .Select(bankAccount => new BankAccountDto()
                                {
                                    Id = bankAccount.GetId(),
                                    OwnerId = bankAccount.AccountOwner.GetId(),
                                    AccountNumber = bankAccount.AccountNumber,
                                    Balance = bankAccount.AccountBalance,
                                })
                                .ToList();
        }

        private BankAccountDto? ProcessGetBankAccountByNumber(string? accountNumber)
        {
            if (accountNumber == null)
            {
                return null;
            }
            ABankAccount? bankAccount = logicLayer.GetBankAccountByAccountNumber(accountNumber);
            if (bankAccount == null)
            {
                return null;
            }
            return new BankAccountDto()
            {
                Id = bankAccount.GetId(),
                OwnerId = bankAccount.AccountOwner.GetId(),
                AccountNumber = bankAccount.AccountNumber,
                Balance = bankAccount.AccountBalance,
            };
        }

        private bool? ProcessCheckForReportsUpdate(int? ownerId)
        {
            if (ownerId == null)
            {
                return null;
            }
            return logicLayer.CheckForReportsUpdates(ownerId.Value);
        }

        private TransferResultCodes? ProcessTransfer(TransferData? transferData)
        {
            if (transferData == null)
            {
                return null;
            }
            return (TransferResultCodes)(int)logicLayer.PerformTransfer(transferData.SourceAccountNumber, 
                                                                        transferData.TargetAccountNumber, 
                                                                        transferData.Amount, 
                                                                        transferData.Description);
        }

        #endregion private
    }
}
