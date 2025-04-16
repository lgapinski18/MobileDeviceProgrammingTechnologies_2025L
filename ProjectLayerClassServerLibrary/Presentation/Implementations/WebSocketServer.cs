using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProjectLayerClassServerLibrary.LogicLayer;
using System.Xml;
using System.Security.Principal;
using ProjectLayerClassLibrary.PresentationLayer.ModelLayer.Implementations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static ProjectLayerClassServerLibrary.Presentation.WebSocketConnection;


namespace ProjectLayerClassServerLibrary.Presentation.Implementations
{
    internal class WebSocketServer : IWebSocketServer
    {
        private Task serverLoopTask;
        private List<WebSocketConnection> connections = new();
        private ALogicLayer logicLayer;

        private enum ComunicationCodeFromClient
        {
            CODE_NOT_SELECTED,
            CREATE_ACCOUNT_OWNER_CODE,
            CREATE_BANK_ACCOUNT_CODE,
            GET_ACCOUNT_OWNER_CODE,
            GET_ACCOUNT_OWNER_LOGIN_CODE,
            GET_ALL_ACCOUNT_OWNERS_CODE,
            GET_BANK_ACCOUNT_CODE,
            GET_ALL_BANK_ACCOUNTS_CODE,
            GET_BANK_ACCOUNTS_CODE,
            AUTHENTICATE_ACCOUNT_OWNER_CODE,
            LOGOUT_ACCOUNT_OWNER_CODE,
            PERFORM_TRANSFER_CODE,
            CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE
        }

        public bool IsRunning { get => !serverLoopTask.IsCompleted; }

        public WebSocketServer(int portNo, ALogicLayer logicLayer)
        {
            this.logicLayer = logicLayer;
            Uri _uri = new Uri($@"http://localhost:{portNo}/");
            serverLoopTask = Task.Factory.StartNew(() => ServerLoop(_uri, OnConnection));
        }

        private static async Task ServerLoop(Uri uri, Action<WebSocketConnection> onConnection)
        {
            try
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
            catch (Exception ex)
            {
                string? source = ex.Source;
            }
        }

        private void OnConnection(WebSocketConnection connection)
        {
            Console.WriteLine($"Connection: {connection}");
            connections.Add(connection);
            BasicReportsUpdateModelLayerReporter observer = new BasicReportsUpdateModelLayerReporter(connection, logicLayer, () => { });
            observer.Subscribe(logicLayer.ReportsUpdateLogicLayerTracker);
            connection.onClose = () => {
                Console.WriteLine($"Closing connection: {connection}");
                observer.Unsubscribe();
                connections.Remove(connection);
            };
            connection.onError = () => Console.WriteLine("Error happened");
            connection.onMessage = message => Task.Factory.StartNew(() => ProcessConnectionMessage(connection, message)).Wait();
        }

        const int MESSAGE_TYPE_POSITION = 0;
        const int MESSAGE_TYPE_LENGTH = 4;
        const int MESSAGE_SEQUENCE_NUMBER_POSITION = 4;
        const int MESSAGE_SIZE_POSITION = 8;
        const int MESSAGE_CONTENT_POSITION = 12;

        private void ProcessConnectionMessage(WebSocketConnection connection, byte[] message)
        {
            Console.WriteLine(message);


            //string messageType = Encoding.UTF8.GetString(message, MESSAGE_TYPE_POSITION, MESSAGE_TYPE_LENGTH);
            ComunicationCodeFromClient messageType = (ComunicationCodeFromClient)BitConverter.ToInt32(message, MESSAGE_TYPE_POSITION);
            int messageSequenceNo = BitConverter.ToInt32(message, MESSAGE_SEQUENCE_NUMBER_POSITION);
            int messageSize = BitConverter.ToInt32(message, MESSAGE_SIZE_POSITION);
            string messageContent = Encoding.UTF8.GetString(message, MESSAGE_CONTENT_POSITION, messageSize);

            Console.WriteLine($"MessageType: {messageType}, MessageSequenceNo: {messageSequenceNo}, MessageSize: {messageSize}\nMessageContent:\n{messageContent}\n");


            object? responseContent = null;
            XmlSerializer? serializer = null;
            string serializedMessage = "";
            int responseCode = -1;
            bool responde = true;
            ComunicationCodeFromServer responseType = ComunicationCodeFromServer.CODE_NOT_SELECTED;

            switch (messageType)
            {
                case ComunicationCodeFromClient.AUTHENTICATE_ACCOUNT_OWNER_CODE:
                    //AAO - authethicate account owner, data: string login, string password
                    responseContent = ProcessAuthenthicateAccountOwner(GetData<Credentials>(messageContent), connection);
                    serializer = new XmlSerializer(typeof(Success)); //////////
                    responseType = ComunicationCodeFromServer.AUTHENTICATE_ACCOUNT_OWNER_CODE;
                    break;

                case ComunicationCodeFromClient.LOGOUT_ACCOUNT_OWNER_CODE:
                    //AAO - authethicate account owner, data: string login, string password
                    ProcessLogOutAccountOwner(connection);
                    serializer = new XmlSerializer(typeof(Success)); //////////
                    responde = false;
                    break;

                case ComunicationCodeFromClient.CREATE_ACCOUNT_OWNER_CODE:
                    //_CAO - create account owner; Dane: "{ownerName};{ownerSurname};{ownerEmail};{ownerPassword}"
                    responseContent = ProcessCreateAccountOwner(GetData<AccountOwnerCreationData>(messageContent));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    responseType = ComunicationCodeFromServer.CREATE_ACCOUNT_OWNER_CODE;
                    break;

                case ComunicationCodeFromClient.CREATE_BANK_ACCOUNT_CODE:
                    //_CBA - create bank account; Dane: int ownerId
                    responseContent = ProcessCreateBankAccount(GetData<Identificator>(messageContent));
                    serializer = new XmlSerializer(typeof(BankAccountDto)); //////////
                    responseType = ComunicationCodeFromServer.CREATE_BANK_ACCOUNT_CODE;
                    break;

                case ComunicationCodeFromClient.GET_ACCOUNT_OWNER_CODE:
                    //_GAO - get account owner; Dane: int ownerId
                    responseContent = ProcessGetAccountOwner(GetData<Identificator>(messageContent));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    responseType = ComunicationCodeFromServer.GET_ACCOUNT_OWNER_CODE;
                    break;

                case ComunicationCodeFromClient.GET_ALL_ACCOUNT_OWNERS_CODE:
                    //GAAO - get all account owners; 0 danych
                    responseContent = ProcessGetAllAccountOwners();
                    serializer = new XmlSerializer(typeof(List<AccountOwnerDto>));
                    responseType = ComunicationCodeFromServer.GET_ALL_ACCOUNT_OWNERS_CODE;
                    break;

                case ComunicationCodeFromClient.GET_BANK_ACCOUNT_CODE:
                    //GBAN - get bank account; dane: "{accountNumber}"
                    responseContent = ProcessGetBankAccountByNumber(GetData<AccountNumberDto>(messageContent));
                    serializer = new XmlSerializer(typeof(BankAccountDto)); /////////
                    responseType = ComunicationCodeFromServer.GET_BANK_ACCOUNT_CODE;
                    break;

                case ComunicationCodeFromClient.GET_BANK_ACCOUNTS_CODE:
                    //GBAS - get bank accounts for owner id; dane: int owner id;
                    responseContent = ProcessGetBankAccounts(GetData<Identificator>(messageContent));
                    serializer = new XmlSerializer(typeof(List<BankAccountDto>)); ////////
                    responseType = ComunicationCodeFromServer.GET_BANK_ACCOUNTS_CODE;
                    break;

                case ComunicationCodeFromClient.GET_ALL_BANK_ACCOUNTS_CODE:
                    //GABA - get all accounts; 0 danych
                    responseContent = ProcessGetAllBankAccounts();
                    serializer = new XmlSerializer(typeof(List<BankAccountDto>));
                    responseType = ComunicationCodeFromServer.GET_ALL_BANK_ACCOUNTS_CODE;
                    break;

                case ComunicationCodeFromClient.GET_ACCOUNT_OWNER_LOGIN_CODE:
                    //GAOL - get account owner by login, data: string login
                    responseContent = ProcessGetAccountOwnerByLogin(GetData<LoginDto>(messageContent));
                    serializer = new XmlSerializer(typeof(AccountOwnerDto));
                    responseType = ComunicationCodeFromServer.GET_ACCOUNT_OWNER_LOGIN_CODE;
                    break;

                case ComunicationCodeFromClient.CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE:
                    //CFRU - check for reports updates, data: int ownerId
                    responseContent = ProcessCheckForReportsUpdate(GetData<Identificator>(messageContent));
                    serializer = new XmlSerializer(typeof(bool)); ///////
                    responseType = ComunicationCodeFromServer.CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE;
                    break;

                case ComunicationCodeFromClient.PERFORM_TRANSFER_CODE:
                    //___T - string ownerAccountNumber, string targetAccountNumber, float amount, string description,
                    TransferResultCodes? transferResponseCode = ProcessTransfer(GetData<TransferData>(messageContent));
                    if (transferResponseCode != null)
                    {
                        responseContent = transferResponseCode.Value;
                    }
                    serializer = new XmlSerializer(typeof(TransferResultCodes));
                    responseType = ComunicationCodeFromServer.PERFORM_TRANSFER_CODE;
                    break;

                default:
                    responseCode = 1;
                    break;
            }

            if (responde)
            {
                if (responseContent != null && serializer != null)
                {
                    responseCode = 0;
                    StringWriter stringWriter = new StringWriter();
                    serializer.Serialize(stringWriter, responseContent);
                    serializedMessage = stringWriter.ToString();
                }
                else if (serializer != null)
                {
                    serializedMessage = "";
                }
                else if (responseCode == -1)
                {
                    responseCode = 2;
                }

                connection.SendAsync(responseType, messageSequenceNo, responseCode, serializedMessage).Wait();
            }
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

        private Success? ProcessAuthenthicateAccountOwner(Credentials? credentials, WebSocketConnection connection)
        {
            if (credentials == null)
            {
                return null;
            }
            bool? isLogged = logicLayer.AuthenticateAccountOwner(credentials.Login, credentials.Password);
            if (isLogged == true)
            {
                connection.LoggedOwnerId = logicLayer.GetAccountOwner(credentials.Login)?.GetId();
            }
            return new Success() { IsSuccess = isLogged.Value };
        }

        private void ProcessLogOutAccountOwner(WebSocketConnection connection)
        {
            connection.LoggedOwnerId = null;
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
            response.CreationFlags = (CreationAccountOwnerFlags)(int)creationAccountOwnerFlags;
            if (accountOwner != null)
            {
                response.AccountOwner = new AccountOwnerDto() { 
                    Id = accountOwner.GetId(), 
                    Login = accountOwner.OwnerLogin,
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
                                    Login = accountOwner.OwnerLogin,
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

        private AccountOwnerDto? ProcessGetAccountOwner(Identificator? ownerId)
        {
            if (ownerId == null)
            {
                return null;
            }
            AAccountOwner? accountOwner = logicLayer.GetAccountOwner(ownerId.Id);
            if (accountOwner == null)
            {
                return null;
            }
            return new AccountOwnerDto()
            {
                Id = accountOwner.GetId(),
                Login = accountOwner.OwnerLogin,
                Name = accountOwner.OwnerName,
                Surname = accountOwner.OwnerSurname,
                Email = accountOwner.OwnerEmail
            };
        }

        private AccountOwnerDto? ProcessGetAccountOwnerByLogin(LoginDto? login)
        {
            if (login == null)
            {
                return null;
            }
            AAccountOwner? accountOwner = logicLayer.GetAccountOwner(login.Login);
            if (accountOwner == null)
            {
                return null;
            }
            return new AccountOwnerDto()
            {
                Id = accountOwner.GetId(),
                Login = accountOwner.OwnerLogin,
                Name = accountOwner.OwnerName,
                Surname = accountOwner.OwnerSurname,
                Email = accountOwner.OwnerEmail
            };
        }

        private BankAccountDto? ProcessCreateBankAccount(Identificator? ownerId)
        {
            if (ownerId == null)
            {
                return null;
            }
            ABankAccount? bankAccount = logicLayer.OpenNewBankAccount(ownerId.Id);
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

        private List<BankAccountDto> ProcessGetBankAccounts(Identificator? ownerId)
        {
            if (ownerId == null)
            {
                return null;
            }
            return logicLayer.GetAccountOwnerBankAccounts(ownerId.Id)
                                .Select(bankAccount => new BankAccountDto()
                                {
                                    Id = bankAccount.GetId(),
                                    OwnerId = bankAccount.AccountOwner.GetId(),
                                    AccountNumber = bankAccount.AccountNumber,
                                    Balance = bankAccount.AccountBalance,
                                })
                                .ToList();
        }

        private BankAccountDto? ProcessGetBankAccountByNumber(AccountNumberDto? accountNumber)
        {
            if (accountNumber == null)
            {
                return null;
            }
            ABankAccount? bankAccount = logicLayer.GetBankAccountByAccountNumber(accountNumber.AccountNumber);
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

        private bool? ProcessCheckForReportsUpdate(Identificator? ownerId)
        {
            if (ownerId == null)
            {
                return null;
            }
            return logicLayer.CheckForReportsUpdates(ownerId.Id);
        }

        private TransferResultCodes? ProcessTransfer(TransferData? transferData)
        {
            if (transferData == null)
            {
                return null;
            }
            TransferResultCodes? result = (TransferResultCodes)(int)logicLayer.PerformTransfer(transferData.SourceAccountNumber,
                                                                        transferData.TargetAccountNumber,
                                                                        transferData.Amount,
                                                                        transferData.Description);

            if (result == TransferResultCodes.SUCCESS)
            {
                HashSet<int> ids = new HashSet<int>()
                {
                    logicLayer.GetBankAccountByAccountNumber(transferData.SourceAccountNumber).AccountOwner.GetId(),
                    logicLayer.GetBankAccountByAccountNumber(transferData.TargetAccountNumber).AccountOwner.GetId(),
                };

                foreach (WebSocketConnection connection in connections.Where(connection => connection.LoggedOwnerId != null && ids.Contains(connection.LoggedOwnerId.Value)))
                {
                    connection.SendAsync(ComunicationCodeFromServer.BANK_ACCOUNTS_UPDATES_CODE, 0, 0, "");
                }
            }
            return result;
        }

        #region REACTIVE_INTERACTION

        private static void NotifyClientAboutReportsUpdate()
        {

        }

        #endregion
    }
}
