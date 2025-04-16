using ProjectLayerClassLibrary.DataLayer.Additionals;
using ProjectLayerClassLibrary.DataLayer.Exceptions;
using ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class ServerComunicatingDataLayer : ADataLayer
    {
        private ClientWebSocket clientWebSocket;
        private CancellationTokenSource cts;
        private int portNo = 8080;
        private Task receiveLoopTask;
        private MyLogger myLogger;

        //private object accountOwnerLock = new object();
        //private object bankAccountLock = new object();

        private bool isConnected = false;
        private object checkConnectionLock = new();
        private object awaitingConnectionLock = new();
        private object reportsUpdateTrackerLock = new();
        private object euroRateUpdateLock = new();
        private object usdRateUpdateLock = new();
        private object gbpRateUpdateLock = new();
        private object chfRateUpdateLock = new();

        private CurrenciesOfInterest currenciesOfInterestFilter = (CurrenciesOfInterest)0;
        public override CurrenciesOfInterest CurrenciesOfInterestFilter { get => currenciesOfInterestFilter; set => currenciesOfInterestFilter = value; }

        internal enum ComunicationCodeFromClient
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

        internal enum ComunicationCodeFromServer
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
            PERFORM_TRANSFER_CODE,
            REACTIVE_REPORTS_UPDATE_CODE,
            REACTIVE_BROADCAST_TO_FILTER_CURRENCY_UPDATE_CODE,
            CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE,
            BANK_ACCOUNTS_UPDATES_CODE
        }

        //private const string CREATE_ACCOUNT_OWNER_CODE = "_CAO";
        //private const string CREATE_BANK_ACCOUNT_CODE = "_CBA";
        //private const string GET_ACCOUNT_OWNER_CODE = "_GAO";
        //private const string GET_ACCOUNT_OWNER_LOGIN_CODE = "GAOL";
        //private const string GET_ALL_ACCOUNT_OWNERS_CODE = "GAAO";
        //private const string GET_BANK_ACCOUNT_CODE = "GBAN";
        //private const string GET_ALL_BANK_ACCOUNTS_CODE = "GABA";
        //private const string GET_BANK_ACCOUNTS_CODE = "GBAS";
        //private const string AUTHENTICATE_ACCOUNT_OWNER = "_AAO";
        //private const string CHECK_FOR_REPORTS_UPDATES = "CFRU";
        //private const string TRANSFER = "___T";
        //private const string REACTIVE_REPORTS_UPDATE = "_RRU";
        //private const string BANK_ACCOUNTS_UPDATES = "_BAU";

        #region EVENTS

        protected AReportsUpdateDataLayerTracker reportsUpdateTracker;
        public override AReportsUpdateDataLayerTracker ReportsUpdateTracker { get { return reportsUpdateTracker; } }

        public override event Action BankAccountsUpdate;
        public override event CurrencyRatesUpdateAction EuroRatesUpdateEvent;
        public override event CurrencyRatesUpdateAction UsdRatesUpdateEvent;
        public override event CurrencyRatesUpdateAction GbpRatesUpdateEvent;
        public override event CurrencyRatesUpdateAction ChfRatesUpdateEvent;

        protected void CallBankAccountsUpdate()
        {
            BankAccountsUpdate?.Invoke();
        }

        #endregion

        public ServerComunicatingDataLayer(int portNo = 8080)
        {
            this.portNo = portNo;
            clientWebSocket = new ClientWebSocket();
            cts = new CancellationTokenSource();
            reportsUpdateTracker = new BasicReportsUpdateDataLayerTracker();


            //myLogger = new MyLogger("C:\\Users\\lukas\\Desktop\\ServerComunicatingDataLayerLogger.txt");
            myLogger = new MyLogger("ServerComunicatingDataLayerLog.txt");
            myLogger.Log("\t\tNEW RUN");

            _ = SetUp();
        }

        public void Dispose()
        {
            lock (checkConnectionLock)
            {
                isConnected = false;
            }
            reportsUpdateTracker.EndAllObservations();
            receiveLoopTask.Dispose();
            cts.Cancel();
            clientWebSocket.Dispose();
            cts.Dispose();
        }

        public async Task SetUp()
        {
            if (!isConnected)
            {
                await clientWebSocket.ConnectAsync(new Uri($"ws://localhost:{portNo}/ws"), CancellationToken.None);
                lock (checkConnectionLock)
                {
                    isConnected = true;
                }

                receiveLoopTask = Task.Factory.StartNew(() => { ClientReceiveLoop(clientWebSocket); });
            }
        }

        private void OnClose()
        {

        }

        private void ClientReceiveLoop(WebSocket clientWebSocket)
        {
            try
            {
                byte[] buffer = new byte[8192];
                while (true)
                {
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult result = clientWebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        OnClose();
                        clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None).Wait();
                        return;
                    }
                    int count = result.Count;
                    while (!result.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            OnClose();
                            clientWebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None).Wait();
                            return;
                        }
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = clientWebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
                        count += result.Count;
                    }
                    //string _message = Encoding.UTF8.GetString(buffer, 0, count);
                    processReceivedData((byte[])buffer.Clone(), count);
                }
            }
            catch (Exception _ex)
            {
                //m_Log($"Connection has been broken because of an exception {_ex}");
                clientWebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Connection has been broken because of an exception", CancellationToken.None).Wait();
            }
        }

        private void processReceivedData(byte[] data, int numberOfReceived)
        {
            //string respondType = Encoding.UTF8.GetString(data, 0, 4);
            ComunicationCodeFromServer respondType = (ComunicationCodeFromServer)BitConverter.ToInt32(data, 0);
            int sequenceNo = BitConverter.ToInt32(data, 4);
            int resultCode = BitConverter.ToInt32(data, 8);
            int dataSize = BitConverter.ToInt32(data, 12);
            string dataMessage = Encoding.UTF8.GetString(data, 16, dataSize);
            myLogger.Log($"ResponseType: {respondType}, SequenceNo: {sequenceNo}, resultCode: {resultCode}, dataSize: {dataSize},\nData Message: {dataMessage}");

            XmlSerializer serializer;

            using (var reader = new StringReader(dataMessage))
            {
                switch (respondType)
                {
                    case ComunicationCodeFromServer.CREATE_ACCOUNT_OWNER_CODE:
                        myLogger.Log($"CREATE_ACCOUNT_OWNER");
                        serializer = new XmlSerializer(typeof(CreationAccountOwnerResponse));
                        lock (createAccountOwnerResponseLock)
                        {
                            createAccountOwnerReponses.Add(sequenceNo, AAccountOwner.CreateAcountOwnerFromXml(((CreationAccountOwnerResponse?)serializer.Deserialize(reader)).AccountOwner));
                        }
                        //Monitor.PulseAll(createAccountOwnerMonitorLock);
                        lock (createAccountOwnerMonitorLock)
                        {
                            for (; createAccountOwnerWaitingThreadsCounter > 0; --createAccountOwnerWaitingThreadsCounter)
                            {
                                createAccountOwnerAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.CREATE_BANK_ACCOUNT_CODE:
                        myLogger.Log($"CREATE_BANK_ACCOUNT");
                        serializer = new XmlSerializer(typeof(BankAccountDto));
                        lock (createBankAccountResponseLock)
                        {
                            createBankAccountReponses.Add(sequenceNo, ABankAccount.CreateBankAccountFromXml((BankAccountDto?)serializer.Deserialize(reader)));
                        }
                        //Monitor.PulseAll(createBankAccountMonitorLock);
                        lock (createBankAccountMonitorLock)
                        {
                            for (; createBankAccountWaitingThreadsCounter > 0; --createBankAccountWaitingThreadsCounter)
                            {
                                createBankAccountAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.GET_ACCOUNT_OWNER_CODE:
                        myLogger.Log($"GET_ACCOUNT_OWNER");
                        serializer = new XmlSerializer(typeof(AccountOwnerDto));
                        lock (getAccountOwnerResponseLock)
                        {
                            getAccountOwnerReponses.Add(sequenceNo, AAccountOwner.CreateAcountOwnerFromXml((AccountOwnerDto?)serializer.Deserialize(reader)));
                        }
                        //Monitor.PulseAll(getAccountOwnerMonitorLock);
                        lock (getAccountOwnerMonitorLock)
                        {
                            for (; getAccountOwnerWaitingThreadsCounter > 0; --getAccountOwnerWaitingThreadsCounter)
                            {
                                getAccountOwnerAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.GET_ACCOUNT_OWNER_LOGIN_CODE:
                        myLogger.Log($"GET_ACCOUNT_OWNER_LOGIN");
                        serializer = new XmlSerializer(typeof(AccountOwnerDto));
                        lock (getAccountOwnerLoginResponseLock)
                        {
                            getAccountOwnerLoginReponses.Add(sequenceNo, AAccountOwner.CreateAcountOwnerFromXml((AccountOwnerDto?)serializer.Deserialize(reader)));
                        }
                        //Monitor.PulseAll(getAccountOwnerLoginMonitorLock);
                        lock (getAccountOwnerLoginMonitorLock)
                        {
                            for (; getAccountOwnerLoginWaitingThreadsCounter > 0; --getAccountOwnerLoginWaitingThreadsCounter)
                            {
                                getAccountOwnerLoginAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.GET_ALL_ACCOUNT_OWNERS_CODE:
                        myLogger.Log($"GET_ALL_ACCOUNT_OWNERS");
                        serializer = new XmlSerializer(typeof(List<AccountOwnerDto>));
                        lock (getAllAccountOwnersResponseLock)
                        {
                            List<AccountOwnerDto> accountOwnerDtos = (List<AccountOwnerDto>)serializer.Deserialize(reader);
                            getAllAccountOwnersReponses.Add(sequenceNo, accountOwnerDtos.Select((aODto) => AAccountOwner.CreateAcountOwnerFromXml(aODto)).ToList());
                        }
                        //Monitor.PulseAll(getAllAccountOwnersMonitorLock);
                        lock (getAllAccountOwnersMonitorLock)
                        {
                            for (; getAllAccountOwnersWaitingThreadsCounter > 0; --getAllAccountOwnersWaitingThreadsCounter)
                            {
                                getAllAccountOwnersAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.GET_BANK_ACCOUNT_CODE:
                        myLogger.Log($"GET_BANK_ACCOUNT");
                        serializer = new XmlSerializer(typeof(BankAccountDto));
                        lock (gtBankAccountResponseLock)
                        {
                            gtBankAccountReponses.Add(sequenceNo, ABankAccount.CreateBankAccountFromXml((BankAccountDto?)serializer.Deserialize(reader)));
                        }
                        //Monitor.PulseAll(gtBankAccountMonitorLock);
                        lock (gtBankAccountMonitorLock)
                        {
                            for (; gtBankAccountWaitingThreadsCounter > 0; --gtBankAccountWaitingThreadsCounter)
                            {
                                gtBankAccountAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.GET_ALL_BANK_ACCOUNTS_CODE:
                        myLogger.Log($"GET_ALL_BANK_ACCOUNTS");
                        serializer = new XmlSerializer(typeof(List<BankAccountDto>));
                        lock (getAllBankAccountsResponseLock)
                        {
                            getAllBankAccountsReponses.Add(sequenceNo, ((List<BankAccountDto>)serializer.Deserialize(reader)).Select((bADto) => ABankAccount.CreateBankAccountFromXml(bADto)).ToList());
                        }
                        //Monitor.PulseAll(getAllBankAccountsMonitorLock);
                        lock (getAllBankAccountsMonitorLock)
                        {
                            for (; getAllBankAccountsWaitingThreadsCounter > 0; --getAllBankAccountsWaitingThreadsCounter)
                            {
                                getAllBankAccountsAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.GET_BANK_ACCOUNTS_CODE:
                        myLogger.Log($"GET_BANK_ACCOUNTS");
                        serializer = new XmlSerializer(typeof(List<BankAccountDto>));
                        lock (getBankAccountsResponseLock)
                        {
                            getBankAccountsReponses.Add(sequenceNo, ((List<BankAccountDto>)serializer.Deserialize(reader)).Select((bADto) => ABankAccount.CreateBankAccountFromXml(bADto)).ToList());
                        }
                        //Monitor.PulseAll(getBankAccountsMonitorLock);
                        lock (getBankAccountsMonitorLock)
                        {
                            for (; getBankAccountsWaitingThreadsCounter > 0; --getBankAccountsWaitingThreadsCounter)
                            {
                                getBankAccountsAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.AUTHENTICATE_ACCOUNT_OWNER_CODE:
                        myLogger.Log($"AUTHENTICATE_ACCOUNT_OWNER");
                        serializer = new XmlSerializer(typeof(bool));
                        lock (authenticateAccountOwnerResponseLock)
                        {
                            authenticateAccountOwnerReponses.Add(sequenceNo, (bool)serializer.Deserialize(reader));
                        }
                        //Monitor.PulseAll(authenticateAccountOwnerMonitorLock);
                        myLogger.Log($"AUTHENTICATE_ACCOUNT_OWNER \nChecking Lock");
                        lock (authenticateAccountOwnerMonitorLock)
                        {
                            myLogger.Log($"AUTHENTICATE_ACCOUNT_OWNER \nAwaiting Threads {authenticateAccountOwnerWaitingThreadsCounter}");
                            for (; authenticateAccountOwnerWaitingThreadsCounter > 0; --authenticateAccountOwnerWaitingThreadsCounter)
                            {
                                authenticateAccountOwnerAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE:
                        myLogger.Log($"CHECK_FOR_REPORTS_UPDATES");
                        serializer = new XmlSerializer(typeof(bool));
                        lock (checkForReportsUpdatesResponseLock)
                        {
                            checkForReportsUpdatesReponses.Add(sequenceNo, (bool)serializer.Deserialize(reader));
                        }
                        //Monitor.PulseAll(checkForReportsUpdatesMonitorLock);
                        lock (checkForReportsUpdatesMonitorLock)
                        {
                            for (; checkForReportsUpdatesWaitingThreadsCounter > 0; --checkForReportsUpdatesWaitingThreadsCounter)
                            {
                                checkForReportsUpdatesAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.PERFORM_TRANSFER_CODE:
                        myLogger.Log($"TRANSFER");
                        serializer = new XmlSerializer(typeof(ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures.TransferResultCodes));
                        lock (performTransferResponseLock)
                        {
                            performTransferReponses.Add(sequenceNo, (ADataLayer.TransferResultCodes)serializer.Deserialize(reader));
                        }
                        //Monitor.PulseAll(performTransferMonitorLock);
                        lock (performTransferMonitorLock)
                        {
                            for (; performTransferWaitingThreadsCounter > 0; --performTransferWaitingThreadsCounter)
                            {
                                performTransferAutoResetEvent.Set();
                            }
                        }
                        break;

                    case ComunicationCodeFromServer.REACTIVE_REPORTS_UPDATE_CODE:
                        myLogger.Log($"REACTIVE_REPORTS_UPDATE");
                        serializer = new XmlSerializer(typeof(List<BankAccountReportDto>));
                        lock (reportsUpdateTrackerLock)
                        {
                            //reportsUpdateTracker.TrackWhetherReportsUpdatesChanged((bool)serializer.Deserialize(reader));
                            List<ABankAccountReport> reports = ((List<BankAccountReportDto>)serializer.Deserialize(reader)).Select(bARDto => ABankAccountReport.CreateBankAccountReportFromXml(bARDto)).ToList();
                            Task.Factory.StartNew(() => { reportsUpdateTracker.TrackWhetherReportsUpdatesChanged(reports); });
                        }
                        break;

                    case ComunicationCodeFromServer.BANK_ACCOUNTS_UPDATES_CODE:
                        myLogger.Log($"BANK_ACCOUNTS_UPDATES");
                        Task.Factory.StartNew(() => { CallBankAccountsUpdate(); });
                        break;

                    case ComunicationCodeFromServer.REACTIVE_BROADCAST_TO_FILTER_CURRENCY_UPDATE_CODE:
                        myLogger.Log($"BANK_ACCOUNTS_UPDATES");
                        serializer = new XmlSerializer(typeof(int));
                        Task.Factory.StartNew(() =>
                        {
                            lock (euroRateUpdateLock)
                            {
                                if ((CurrenciesOfInterestFilter | CurrenciesOfInterest.EURO) == CurrenciesOfInterest.EURO)
                                {

                                }
                            }

                            lock (usdRateUpdateLock)
                            {
                                if ((CurrenciesOfInterestFilter | CurrenciesOfInterest.USD) == CurrenciesOfInterest.USD)
                                {

                                }
                            }
                            lock (gbpRateUpdateLock)
                            {
                                if ((CurrenciesOfInterestFilter | CurrenciesOfInterest.GBP) == CurrenciesOfInterest.GBP)
                                {

                                }
                            }
                            lock (chfRateUpdateLock)
                            {
                                if ((CurrenciesOfInterestFilter | CurrenciesOfInterest.CHF) == CurrenciesOfInterest.CHF)
                                {

                                }
                            }
                        });
                        break;
                }
            }
        }

        private object createAccountOwnerLock = new();
        private object createAccountOwnerMonitorLock = new();
        private object createAccountOwnerResponseLock = new();
        private readonly AutoResetEvent createAccountOwnerAutoResetEvent = new AutoResetEvent(false);
        private static int createAccountOwnerWaitingThreadsCounter = 0;
        private static int createAccountOwnerLockSequenceNoCounter = 0;
        private Dictionary<int, AAccountOwner?> createAccountOwnerReponses = new Dictionary<int, AAccountOwner?>();
        public override AAccountOwner CreateAccountOwner(string ownerName, string ownerSurname, string ownerEmail, string ownerPassword, out CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (createAccountOwnerLock)
                {
                    sequenceNo = createAccountOwnerLockSequenceNoCounter++;
                }
                AccountOwnerCreationData accountOwnerCreationData = new AccountOwnerCreationData();
                accountOwnerCreationData.Name = ownerName;
                accountOwnerCreationData.Surname = ownerSurname;
                accountOwnerCreationData.Email = ownerEmail;
                accountOwnerCreationData.Password = ownerPassword;
                XmlSerializer serializer = new XmlSerializer(typeof(AccountOwnerCreationData));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, accountOwnerCreationData);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.CREATE_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"CREATE_ACCOUNT_OWNER_CODE\t{sequenceNo}\tRestart loop");
                    lock (createAccountOwnerMonitorLock)
                    {
                        createAccountOwnerWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"CREATE_ACCOUNT_OWNER_CODE\t{sequenceNo}\tNow {createAccountOwnerWaitingThreadsCounter} threads awaitng!");
                    createAccountOwnerAutoResetEvent.WaitOne();
                    lock (createAccountOwnerResponseLock)
                    {
                        if (createAccountOwnerReponses.ContainsKey(sequenceNo))
                        {
                            AAccountOwner? aAccountOwner = createAccountOwnerReponses[sequenceNo];
                            if (aAccountOwner == null)
                            {
                                creationAccountOwnerFlags = CreationAccountOwnerDataLayerFlags.EMPTY;
                            }
                            else
                            {
                                creationAccountOwnerFlags = CreationAccountOwnerDataLayerFlags.SUCCESS;
                            }
                            createAccountOwnerReponses.Remove(sequenceNo);
                            return aAccountOwner;
                        }
                    }
                    myLogger.Log($"CREATE_ACCOUNT_OWNER_CODE\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object createBankAccountLock = new object();
        private object createBankAccountMonitorLock = new object();
        private object createBankAccountResponseLock = new();
        private readonly AutoResetEvent createBankAccountAutoResetEvent = new AutoResetEvent(false);
        private int createBankAccountWaitingThreadsCounter = 0;
        private static int createBankAccountSequenceNoCounter = 0;
        private Dictionary<int, ABankAccount?> createBankAccountReponses = new Dictionary<int, ABankAccount?>();
        public override ABankAccount CreateBankAccount(int ownerId)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (createBankAccountLock)
                {
                    sequenceNo = createBankAccountSequenceNoCounter++;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(int));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, ownerId);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.CREATE_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"CREATE_BANK_ACCOUNT_CODE\t{sequenceNo}\tRestart loop");
                    lock (createBankAccountMonitorLock)
                    {
                        createBankAccountWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"CREATE_BANK_ACCOUNT_CODE\t{sequenceNo}\tNow {createBankAccountWaitingThreadsCounter} threads awaitng!");
                    createBankAccountAutoResetEvent.WaitOne();
                    lock (createBankAccountResponseLock)
                    {
                        if (createBankAccountReponses.ContainsKey(sequenceNo))
                        {
                            ABankAccount response = createBankAccountReponses[sequenceNo];
                            createBankAccountReponses.Remove(sequenceNo);
                            return response;
                        }
                    }
                    myLogger.Log($"CREATE_BANK_ACCOUNT_CODE\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAccountOwnerLock = new object();
        private object getAccountOwnerMonitorLock = new object();
        private object getAccountOwnerResponseLock = new();
        private readonly AutoResetEvent getAccountOwnerAutoResetEvent = new AutoResetEvent(false);
        private int getAccountOwnerWaitingThreadsCounter = 0;
        private static int getAccountOwnerSequenceNoCounter = 0;
        private Dictionary<int, AAccountOwner?> getAccountOwnerReponses = new Dictionary<int, AAccountOwner?>();
        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (getAccountOwnerLock)
                {
                    sequenceNo = getAccountOwnerSequenceNoCounter++;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(int));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, ownerId);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.GET_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"GET_ACCOUNT_OWNER_CODE\t{sequenceNo}\tRestart loop");
                    lock (getAccountOwnerMonitorLock)
                    {
                        getAccountOwnerWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"GET_ACCOUNT_OWNER_CODE\t{sequenceNo}\tNow {getAccountOwnerWaitingThreadsCounter} threads awaitng!");
                    getAccountOwnerAutoResetEvent.WaitOne();
                    lock (getAccountOwnerResponseLock)
                    {
                        if (getAccountOwnerReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = getAccountOwnerReponses[sequenceNo];
                            getAccountOwnerReponses.Remove(sequenceNo);
                            return reponse;
                        }
                    }
                    myLogger.Log($"GET_ACCOUNT_OWNER_CODE\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAccountOwnerLoginLock = new object();
        private object getAccountOwnerLoginMonitorLock = new object();
        private object getAccountOwnerLoginResponseLock = new();
        private readonly AutoResetEvent getAccountOwnerLoginAutoResetEvent = new AutoResetEvent(false);
        private int getAccountOwnerLoginWaitingThreadsCounter = 0;
        private static int getAccountOwnerLoginSequenceNoCounter = 0;
        private Dictionary<int, AAccountOwner?> getAccountOwnerLoginReponses = new Dictionary<int, AAccountOwner?>();
        public override AAccountOwner? GetAccountOwner(string ownerLogin)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (getAccountOwnerLoginLock)
                {
                    sequenceNo = getAccountOwnerLoginSequenceNoCounter++;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(string));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, ownerLogin);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.GET_ACCOUNT_OWNER_LOGIN_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"GET_ACCOUNT_OWNER_LOGIN_CODE\t{sequenceNo}\tRestart loop");
                    lock (getAccountOwnerLoginMonitorLock)
                    {
                        getAccountOwnerLoginWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"GET_ACCOUNT_OWNER_LOGIN_CODE\t{sequenceNo}\tNow {getAccountOwnerLoginWaitingThreadsCounter} threads awaitng!");
                    getAccountOwnerLoginAutoResetEvent.WaitOne();
                    lock (getAccountOwnerLoginResponseLock)
                    {
                        if (getAccountOwnerLoginReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = getAccountOwnerLoginReponses[sequenceNo];
                            getAccountOwnerLoginReponses.Remove(sequenceNo);
                            return reponse;
                        }
                    }
                    myLogger.Log($"GET_ACCOUNT_OWNER_LOGIN_CODE\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAllAccountOwnersLock = new object();
        private object getAllAccountOwnersMonitorLock = new object();
        private object getAllAccountOwnersResponseLock = new();
        private readonly AutoResetEvent getAllAccountOwnersAutoResetEvent = new AutoResetEvent(false);
        private int getAllAccountOwnersWaitingThreadsCounter = 0;
        private static int getAllAccountOwnersSequenceNoCounter = 0;
        private Dictionary<int, ICollection<AAccountOwner>> getAllAccountOwnersReponses = new Dictionary<int, ICollection<AAccountOwner>>();
        public override ICollection<AAccountOwner> GetAllAccountOwners()
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (getAllAccountOwnersLock)
                {
                    sequenceNo = getAllAccountOwnersSequenceNoCounter++;
                }
                byte[] sendBuffer = [];
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.GET_ALL_ACCOUNT_OWNERS_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"GET_ALL_ACCOUNT_OWNERS_CODE\t{sequenceNo}\tRestart loop");
                    lock (getAllAccountOwnersMonitorLock)
                    {
                        getAllAccountOwnersWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"GET_ALL_ACCOUNT_OWNERS_CODE\t{sequenceNo}\tNow {getAllAccountOwnersWaitingThreadsCounter} threads awaitng!");
                    getAllAccountOwnersAutoResetEvent.WaitOne();
                    lock (getAllAccountOwnersResponseLock)
                    {
                        if (getAllAccountOwnersReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = getAllAccountOwnersReponses[sequenceNo];
                            getAllAccountOwnersReponses.Remove(sequenceNo);
                            return reponse;
                        }
                    }
                    myLogger.Log($"GET_ALL_ACCOUNT_OWNERS_CODE\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAllBankAccountsLock = new object();
        private object getAllBankAccountsMonitorLock = new object();
        private object getAllBankAccountsResponseLock = new();
        private readonly AutoResetEvent getAllBankAccountsAutoResetEvent = new AutoResetEvent(false);
        private int getAllBankAccountsWaitingThreadsCounter = 0;
        private static int getAllBankAccountsSequenceNoCounter = 0;
        private Dictionary<int, ICollection<ABankAccount>> getAllBankAccountsReponses = new Dictionary<int, ICollection<ABankAccount>>();
        public override ICollection<ABankAccount> GetAllBankAccounts()
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (getAllBankAccountsLock)
                {
                    sequenceNo = getAllBankAccountsSequenceNoCounter++;
                }
                byte[] sendBuffer = [];
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.GET_ALL_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"GET_ALL_BANK_ACCOUNTS_CODE\t{sequenceNo}\tRestart loop");
                    lock (getAllBankAccountsMonitorLock)
                    {
                        getAllBankAccountsWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"GET_ALL_BANK_ACCOUNTS_CODE\t{sequenceNo}\tNow {getAllBankAccountsWaitingThreadsCounter} threads awaitng!");
                    getAllBankAccountsAutoResetEvent.WaitOne();
                    lock (getAllBankAccountsResponseLock)
                    {
                        if (getAllBankAccountsReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = getAllBankAccountsReponses[sequenceNo];
                            getAllBankAccountsReponses.Remove(sequenceNo);  
                            return reponse;
                        }
                    }
                    myLogger.Log($"GET_ALL_BANK_ACCOUNTS_CODE\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object gtBankAccountLock = new object();
        private object gtBankAccountMonitorLock = new object();
        private object gtBankAccountResponseLock = new();
        private readonly AutoResetEvent gtBankAccountAutoResetEvent = new AutoResetEvent(false);
        private int gtBankAccountWaitingThreadsCounter = 0;
        private static int getBankAccountSequenceNoCounter = 0;
        private Dictionary<int, ABankAccount?> gtBankAccountReponses = new Dictionary<int, ABankAccount?>();
        public override ABankAccount? GetBankAccount(string accountNumber)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (gtBankAccountLock)
                {
                    sequenceNo = getBankAccountSequenceNoCounter++;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(string));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, accountNumber);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.GET_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"GET_BANK_ACCOUNT_CODE\t{sequenceNo}\tRestart loop");
                    lock (gtBankAccountMonitorLock)
                    {
                        gtBankAccountWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"GET_BANK_ACCOUNT_CODE\t{sequenceNo}\tNow {gtBankAccountWaitingThreadsCounter} threads awaitng!");
                    gtBankAccountAutoResetEvent.WaitOne();
                    lock (gtBankAccountResponseLock)
                    {
                        if (gtBankAccountReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = gtBankAccountReponses[sequenceNo];
                            gtBankAccountReponses.Remove(sequenceNo);
                            return reponse;
                        }
                    }
                    myLogger.Log($"GET_BANK_ACCOUNT_CODE\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getBankAccountsLock = new object();
        private object getBankAccountsMonitorLock = new object();
        private object getBankAccountsResponseLock = new();
        private readonly AutoResetEvent getBankAccountsAutoResetEvent = new AutoResetEvent(false);
        private int getBankAccountsWaitingThreadsCounter = 0;
        private static int getBankAccountsSequenceNoCounter = 0;
        private Dictionary<int, ICollection<ABankAccount>> getBankAccountsReponses = new Dictionary<int, ICollection<ABankAccount>>();
        public override ICollection<ABankAccount> GetBankAccounts(int ownerId)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (getBankAccountsLock)
                {
                    sequenceNo = getBankAccountsSequenceNoCounter++;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(int));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, ownerId);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.GET_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"GET_BANK_ACCOUNTS_CODE\t{sequenceNo}\tRestart loop");
                    lock (getBankAccountsMonitorLock)
                    {
                        getBankAccountsWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"GET_BANK_ACCOUNTS_CODE\t{sequenceNo}\tNow {getBankAccountsWaitingThreadsCounter} threads awaitng!");
                    getBankAccountsAutoResetEvent.WaitOne();
                    lock (getBankAccountsResponseLock)
                    {
                        if (getBankAccountsReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = getBankAccountsReponses[sequenceNo];
                            getBankAccountsReponses.Remove(sequenceNo);
                            return reponse;
                        }
                    }
                    myLogger.Log($"GET_BANK_ACCOUNTS_CODE\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object authenticateAccountOwnerLock = new object();
        private object authenticateAccountOwnerMonitorLock = new object();
        private object authenticateAccountOwnerResponseLock = new();
        private readonly AutoResetEvent authenticateAccountOwnerAutoResetEvent = new AutoResetEvent(false);
        private int authenticateAccountOwnerWaitingThreadsCounter = 0;
        private static int authenticateAccountOwnerSequenceNoCounter = 0;
        private Dictionary<int, bool> authenticateAccountOwnerReponses = new Dictionary<int, bool>();
        public override bool AuthenticateAccountOwner(string login, string password)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (authenticateAccountOwnerLock)
                {
                    sequenceNo = authenticateAccountOwnerSequenceNoCounter++;
                }
                Credentials credentials = new Credentials() { Login = login, Password = password };
                XmlSerializer serializer = new XmlSerializer(typeof(Credentials));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, credentials);
                string payload = writer.ToString();
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.AUTHENTICATE_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Binary, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"AUTHENTICATE_ACCOUNT_OWNER\t{sequenceNo}\tRestart loop");
                    lock (authenticateAccountOwnerMonitorLock)
                    {
                        authenticateAccountOwnerWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"AUTHENTICATE_ACCOUNT_OWNER\t{sequenceNo}\tNow {authenticateAccountOwnerWaitingThreadsCounter} threads awaitng!");
                    authenticateAccountOwnerAutoResetEvent.WaitOne();
                    lock (authenticateAccountOwnerResponseLock)
                    {
                        if (authenticateAccountOwnerReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = authenticateAccountOwnerReponses[sequenceNo];
                            authenticateAccountOwnerReponses.Remove(sequenceNo);
                            return reponse;
                        }
                    }
                    myLogger.Log($"AUTHENTICATE_ACCOUNT_OWNER\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object performTransferLock = new object();
        private object performTransferMonitorLock = new object();
        private object performTransferResponseLock = new();
        private readonly AutoResetEvent performTransferAutoResetEvent = new AutoResetEvent(false);
        private int performTransferWaitingThreadsCounter = 0;
        private static int performTransferSequenceNoCounter = 0;
        private Dictionary<int, ADataLayer.TransferResultCodes> performTransferReponses = new Dictionary<int, ADataLayer.TransferResultCodes>();
        public override void PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description, TransferDataLayerCallback transferCallback)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (performTransferLock)
                {
                    sequenceNo = performTransferSequenceNoCounter++;
                }
                TransferData transferData = new TransferData()
                { 
                    SourceAccountNumber = ownerAccountNumber,
                    TargetAccountNumber = targetAccountNumber,
                    Amount = amount,
                    Description = description 
                };
                XmlSerializer serializer = new XmlSerializer(typeof(TransferData));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, transferData);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.PERFORM_TRANSFER_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"TRANSFER\t{sequenceNo}\tRestart loop");
                    lock (performTransferMonitorLock)
                    {
                        performTransferWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"TRANSFER\t{sequenceNo}\tNow {performTransferWaitingThreadsCounter} threads awaitng!");
                    performTransferAutoResetEvent.WaitOne();
                    lock (performTransferResponseLock)
                    {
                        if (performTransferReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = performTransferReponses[sequenceNo];
                            performTransferReponses.Remove(sequenceNo);
                            transferCallback(reponse, ownerAccountNumber, targetAccountNumber, amount, description);
                            //return performTransferReponses[sequenceNo];
                        }
                    }
                    myLogger.Log($"TRANSFER\t{sequenceNo}\tWill repeat loop");
                    return;
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object checkForReportsUpdatesLock = new object();
        private object checkForReportsUpdatesMonitorLock = new object();
        private object checkForReportsUpdatesResponseLock = new();
        private readonly AutoResetEvent checkForReportsUpdatesAutoResetEvent = new AutoResetEvent(false);
        private int checkForReportsUpdatesWaitingThreadsCounter = 0;
        private static int checkForReportsUpdatesSequenceNoCounter = 0;
        private Dictionary<int, bool> checkForReportsUpdatesReponses = new Dictionary<int, bool>();
        public override bool CheckForReportsUpdates(int ownerId)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                int sequenceNo;
                lock (checkForReportsUpdatesLock)
                {
                    sequenceNo = checkForReportsUpdatesSequenceNoCounter++;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(int));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, ownerId);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = BitConverter.GetBytes((int)ComunicationCodeFromClient.CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"CHECK_FOR_REPORTS_UPDATES\t{sequenceNo}\tRestart loop");
                    lock (checkForReportsUpdatesMonitorLock)
                    {
                        checkForReportsUpdatesWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"CHECK_FOR_REPORTS_UPDATES\t{sequenceNo}\tNow {checkForReportsUpdatesWaitingThreadsCounter} threads awaitng!");
                    checkForReportsUpdatesAutoResetEvent.WaitOne();
                    lock (checkForReportsUpdatesResponseLock)
                    {
                        if (checkForReportsUpdatesReponses.ContainsKey(sequenceNo))
                        {
                            var reponse = checkForReportsUpdatesReponses[sequenceNo];
                            checkForReportsUpdatesReponses.Remove(sequenceNo);
                            return reponse;
                        }
                    }
                    myLogger.Log($"CHECK_FOR_REPORTS_UPDATES\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }
    }
}
