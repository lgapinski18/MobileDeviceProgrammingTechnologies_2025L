using ProjectLayerClassLibrary.DataLayer.Repositories;
using ProjectLayerClassLibrary.DataLayer.Exceptions;
using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures;
using System.Xml.Serialization;
using System.IO;
using ProjectLayerClassLibrary.DataLayer.Additionals;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class ServerComunicatingDataLayer : ADataLayer
    {
        private ClientWebSocket clientWebSocket;
        private CancellationTokenSource cts;
        private int portNo = 8080;
        private Task receiveLoop;
        private MyLogger myLogger;

        private object accountOwnerLock = new object();
        private object bankAccountLock = new object();

        private bool isConnected = false;
        private object checkConnectionLock = new object();
        private object awaitingConnectionLock = new object();

        private const string CREATE_ACCOUNT_OWNER_CODE = "_CAO";
        private const string CREATE_BANK_ACCOUNT_CODE = "_CBA";
        private const string GET_ACCOUNT_OWNER_CODE = "_GAO";
        private const string GET_ACCOUNT_OWNER_LOGIN_CODE = "GAOL";
        private const string GET_ALL_ACCOUNT_OWNERS_CODE = "GAAO";
        private const string GET_BANK_ACCOUNT_CODE = "GBAN";
        private const string GET_ALL_BANK_ACCOUNTS_CODE = "GABA";
        private const string GET_BANK_ACCOUNTS_CODE = "GBAS";
        private const string AUTHENTICATE_ACCOUNT_OWNER = "_AAO";
        private const string CHECK_FOR_REPORTS_UPDATES = "CFRU";
        private const string TRANSFER = "___T";
  

        public ServerComunicatingDataLayer()
        {
            clientWebSocket = new ClientWebSocket();
            cts = new CancellationTokenSource();
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
            receiveLoop.Dispose();
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

                receiveLoop = Task.Factory.StartNew(() => { ClientReceiveLoop(clientWebSocket); });
            }
        }

        private void OnClose()
        {

        }

        private void ClientReceiveLoop(WebSocket clientWebSocket)
        {
            try
            {
                byte[] buffer = new byte[1024];
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
                    string _message = Encoding.UTF8.GetString(buffer, 0, count);
                    processReceivedData((byte[])buffer.Clone());
                }
            }
            catch (Exception _ex)
            {
                //m_Log($"Connection has been broken because of an exception {_ex}");
                clientWebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Connection has been broken because of an exception", CancellationToken.None).Wait();
            }
        }

        private void processReceivedData(byte[] data)
        {
            string respondType = Encoding.UTF8.GetString(data, 0, 4);
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
                    case CREATE_ACCOUNT_OWNER_CODE:
                        myLogger.Log($"CREATE_ACCOUNT_OWNER");
                        serializer = new XmlSerializer(typeof(AccountOwnerDto));
                        createAccountOwnerReponses.Add(sequenceNo, AAccountOwner.CreateAcountOwnerFromXml((AccountOwnerDto?)serializer.Deserialize(reader)));
                        //Monitor.PulseAll(createAccountOwnerMonitorLock);
                        lock (createAccountOwnerMonitorLock)
                        {
                            for (; createAccountOwnerWaitingThreadsCounter > 0; --createAccountOwnerWaitingThreadsCounter)
                            {
                                createAccountOwnerAutoResetEvent.Set();
                            }
                        }
                        break;

                    case CREATE_BANK_ACCOUNT_CODE:
                        myLogger.Log($"CREATE_BANK_ACCOUNT");
                        serializer = new XmlSerializer(typeof(BankAccountDto));
                        createBankAccountReponses.Add(sequenceNo, ABankAccount.CreateBankAccountFromXml((BankAccountDto?)serializer.Deserialize(reader)));
                        //Monitor.PulseAll(createBankAccountMonitorLock);
                        lock (createBankAccountMonitorLock)
                        {
                            for (; createBankAccountWaitingThreadsCounter > 0; --createBankAccountWaitingThreadsCounter)
                            {
                                createBankAccountAutoResetEvent.Set();
                            }
                        }
                        break;

                    case GET_ACCOUNT_OWNER_CODE:
                        myLogger.Log($"GET_ACCOUNT_OWNER");
                        serializer = new XmlSerializer(typeof(AccountOwnerDto));
                        getAccountOwnerReponses.Add(sequenceNo, AAccountOwner.CreateAcountOwnerFromXml((AccountOwnerDto?)serializer.Deserialize(reader)));
                        //Monitor.PulseAll(getAccountOwnerMonitorLock);
                        lock (getAccountOwnerMonitorLock)
                        {
                            for (; getAccountOwnerWaitingThreadsCounter > 0; --getAccountOwnerWaitingThreadsCounter)
                            {
                                getAccountOwnerAutoResetEvent.Set();
                            }
                        }
                        break;

                    case GET_ACCOUNT_OWNER_LOGIN_CODE:
                        myLogger.Log($"GET_ACCOUNT_OWNER_LOGIN");
                        serializer = new XmlSerializer(typeof(AccountOwnerDto));
                        getAccountOwnerLoginReponses.Add(sequenceNo, AAccountOwner.CreateAcountOwnerFromXml((AccountOwnerDto?)serializer.Deserialize(reader)));
                        //Monitor.PulseAll(getAccountOwnerLoginMonitorLock);
                        lock (getAccountOwnerLoginMonitorLock)
                        {
                            for (; getAccountOwnerLoginWaitingThreadsCounter > 0; --getAccountOwnerLoginWaitingThreadsCounter)
                            {
                                getAccountOwnerLoginAutoResetEvent.Set();
                            }
                        }
                        break;

                    case GET_ALL_ACCOUNT_OWNERS_CODE:
                        myLogger.Log($"GET_ALL_ACCOUNT_OWNERS");
                        serializer = new XmlSerializer(typeof(List<AccountOwnerDto>));
                        getAllAccountOwnersReponses.Add(sequenceNo, ((List<AccountOwnerDto>)serializer.Deserialize(reader)).Select((aODto) => AAccountOwner.CreateAcountOwnerFromXml(aODto)).ToList());
                        //Monitor.PulseAll(getAllAccountOwnersMonitorLock);
                        lock (getAllAccountOwnersMonitorLock)
                        {
                            for (; getAllAccountOwnersWaitingThreadsCounter > 0; --getAllAccountOwnersWaitingThreadsCounter)
                            {
                                getAllAccountOwnersAutoResetEvent.Set();
                            }
                        }
                        break;

                    case GET_BANK_ACCOUNT_CODE:
                        myLogger.Log($"GET_BANK_ACCOUNT");
                        serializer = new XmlSerializer(typeof(BankAccountDto));
                        gtBankAccountReponses.Add(sequenceNo, ABankAccount.CreateBankAccountFromXml((BankAccountDto?)serializer.Deserialize(reader)));
                        //Monitor.PulseAll(gtBankAccountMonitorLock);
                        lock (gtBankAccountMonitorLock)
                        {
                            for (; gtBankAccountWaitingThreadsCounter > 0; --gtBankAccountWaitingThreadsCounter)
                            {
                                gtBankAccountAutoResetEvent.Set();
                            }
                        }
                        break;

                    case GET_ALL_BANK_ACCOUNTS_CODE:
                        myLogger.Log($"GET_ALL_BANK_ACCOUNTS");
                        serializer = new XmlSerializer(typeof(List<BankAccountDto>));
                        getAllBankAccountsReponses.Add(sequenceNo, ((List<BankAccountDto>)serializer.Deserialize(reader)).Select((bADto) => ABankAccount.CreateBankAccountFromXml(bADto)).ToList());
                        //Monitor.PulseAll(getAllBankAccountsMonitorLock);
                        lock (getAllBankAccountsMonitorLock)
                        {
                            for (; getAllBankAccountsWaitingThreadsCounter > 0; --getAllBankAccountsWaitingThreadsCounter)
                            {
                                getBankAccountsAutoResetEvent.Set();
                            }
                        }
                        break;

                    case GET_BANK_ACCOUNTS_CODE:
                        myLogger.Log($"GET_BANK_ACCOUNTS");
                        serializer = new XmlSerializer(typeof(List<BankAccountDto>));
                        getBankAccountsReponses.Add(sequenceNo, ((List<BankAccountDto>)serializer.Deserialize(reader)).Select((bADto) => ABankAccount.CreateBankAccountFromXml(bADto)).ToList());
                        //Monitor.PulseAll(getBankAccountsMonitorLock);
                        lock (getBankAccountsMonitorLock)
                        {
                            for (; getBankAccountsWaitingThreadsCounter > 0; --getBankAccountsWaitingThreadsCounter)
                            {
                                getBankAccountsAutoResetEvent.Set();
                            }
                        }
                        break;

                    case AUTHENTICATE_ACCOUNT_OWNER:
                        myLogger.Log($"AUTHENTICATE_ACCOUNT_OWNER");
                        serializer = new XmlSerializer(typeof(bool));
                        authenticateAccountOwnerReponses.Add(sequenceNo, (bool)serializer.Deserialize(reader));
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

                    case CHECK_FOR_REPORTS_UPDATES:
                        myLogger.Log($"CHECK_FOR_REPORTS_UPDATES");
                        serializer = new XmlSerializer(typeof(bool));
                        checkForReportsUpdatesReponses.Add(sequenceNo, (bool)serializer.Deserialize(reader));
                        //Monitor.PulseAll(checkForReportsUpdatesMonitorLock);
                        lock (checkForReportsUpdatesMonitorLock)
                        {
                            for (; checkForReportsUpdatesWaitingThreadsCounter > 0; --checkForReportsUpdatesWaitingThreadsCounter)
                            {
                                checkForReportsUpdatesAutoResetEvent.Set();
                            }
                        }
                        break;

                    case TRANSFER:
                        myLogger.Log($"TRANSFER");
                        serializer = new XmlSerializer(typeof(TransferDataLayerCodes));
                        performTransferReponses.Add(sequenceNo, (TransferDataLayerCodes)serializer.Deserialize(reader));
                        //Monitor.PulseAll(performTransferMonitorLock);
                        lock (performTransferMonitorLock)
                        {
                            for (; performTransferWaitingThreadsCounter > 0; --performTransferWaitingThreadsCounter)
                            {
                                performTransferAutoResetEvent.Set();
                            }
                        }
                        break;
                }
            }
        }

        private object createAccountOwnerLock = new object();
        private object createAccountOwnerMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(CREATE_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{CREATE_ACCOUNT_OWNER_CODE}\t{sequenceNo}\tRestart loop");
                    lock (createAccountOwnerMonitorLock)
                    {
                        createAccountOwnerWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{CREATE_ACCOUNT_OWNER_CODE}\t{sequenceNo}\tNow {createAccountOwnerWaitingThreadsCounter} threads awaitng!");
                    createAccountOwnerAutoResetEvent.WaitOne();
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
                        return aAccountOwner;
                    }
                    myLogger.Log($"{CREATE_ACCOUNT_OWNER_CODE}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object createBankAccountLock = new object();
        private object createBankAccountMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(CREATE_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{CREATE_BANK_ACCOUNT_CODE}\t{sequenceNo}\tRestart loop");
                    lock (createBankAccountMonitorLock)
                    {
                        createBankAccountWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{CREATE_BANK_ACCOUNT_CODE}\t{sequenceNo}\tNow {createBankAccountWaitingThreadsCounter} threads awaitng!");
                    createBankAccountAutoResetEvent.WaitOne();
                    if (createBankAccountReponses.ContainsKey(sequenceNo))
                    {
                        return createBankAccountReponses[sequenceNo];
                    }
                    myLogger.Log($"{CREATE_BANK_ACCOUNT_CODE}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAccountOwnerLock = new object();
        private object getAccountOwnerMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(GET_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{GET_ACCOUNT_OWNER_CODE}\t{sequenceNo}\tRestart loop");
                    lock (getAccountOwnerMonitorLock)
                    {
                        getAccountOwnerWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{GET_ACCOUNT_OWNER_CODE}\t{sequenceNo}\tNow {getAccountOwnerWaitingThreadsCounter} threads awaitng!");
                    getAccountOwnerAutoResetEvent.WaitOne();
                    if (getAccountOwnerReponses.ContainsKey(sequenceNo))
                    {
                        return getAccountOwnerReponses[sequenceNo];
                    }
                    myLogger.Log($"{GET_ACCOUNT_OWNER_CODE}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAccountOwnerLoginLock = new object();
        private object getAccountOwnerLoginMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(GET_ACCOUNT_OWNER_LOGIN_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{GET_ACCOUNT_OWNER_LOGIN_CODE}\t{sequenceNo}\tRestart loop");
                    lock (getAccountOwnerLoginMonitorLock)
                    {
                        getAccountOwnerLoginWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{GET_ACCOUNT_OWNER_LOGIN_CODE}\t{sequenceNo}\tNow {getAccountOwnerLoginWaitingThreadsCounter} threads awaitng!");
                    getAccountOwnerLoginAutoResetEvent.WaitOne();
                    if (getAccountOwnerLoginReponses.ContainsKey(sequenceNo))
                    {
                        return getAccountOwnerLoginReponses[sequenceNo];
                    }
                    myLogger.Log($"{GET_ACCOUNT_OWNER_LOGIN_CODE}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAllAccountOwnersLock = new object();
        private object getAllAccountOwnersMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(GET_ALL_ACCOUNT_OWNERS_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{GET_ALL_ACCOUNT_OWNERS_CODE}\t{sequenceNo}\tRestart loop");
                    lock (getAllAccountOwnersMonitorLock)
                    {
                        getAllAccountOwnersWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{GET_ALL_ACCOUNT_OWNERS_CODE}\t{sequenceNo}\tNow {getAllAccountOwnersWaitingThreadsCounter} threads awaitng!");
                    getAllAccountOwnersAutoResetEvent.WaitOne();
                    if (getAllAccountOwnersReponses.ContainsKey(sequenceNo))
                    {
                        return getAllAccountOwnersReponses[sequenceNo];
                    }
                    myLogger.Log($"{GET_ALL_ACCOUNT_OWNERS_CODE}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAllBankAccountsLock = new object();
        private object getAllBankAccountsMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(GET_ALL_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{GET_ALL_BANK_ACCOUNTS_CODE}\t{sequenceNo}\tRestart loop");
                    lock (getAllBankAccountsMonitorLock)
                    {
                        getAllBankAccountsWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{GET_ALL_BANK_ACCOUNTS_CODE}\t{sequenceNo}\tNow {getAllBankAccountsWaitingThreadsCounter} threads awaitng!");
                    getAllBankAccountsAutoResetEvent.WaitOne();
                    if (getAllBankAccountsReponses.ContainsKey(sequenceNo))
                    {
                        return getAllBankAccountsReponses[sequenceNo];
                    }
                    myLogger.Log($"{GET_ALL_BANK_ACCOUNTS_CODE}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object gtBankAccountLock = new object();
        private object gtBankAccountMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(GET_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{GET_BANK_ACCOUNT_CODE}\t{sequenceNo}\tRestart loop");
                    lock (gtBankAccountMonitorLock)
                    {
                        gtBankAccountWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{GET_BANK_ACCOUNT_CODE}\t{sequenceNo}\tNow {gtBankAccountWaitingThreadsCounter} threads awaitng!");
                    gtBankAccountAutoResetEvent.WaitOne();
                    if (gtBankAccountReponses.ContainsKey(sequenceNo))
                    {
                        return gtBankAccountReponses[sequenceNo];
                    }
                    myLogger.Log($"{GET_BANK_ACCOUNT_CODE}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getBankAccountsLock = new object();
        private object getBankAccountsMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(GET_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{GET_BANK_ACCOUNTS_CODE}\t{sequenceNo}\tRestart loop");
                    lock (getBankAccountsMonitorLock)
                    {
                        getBankAccountsWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{GET_BANK_ACCOUNTS_CODE}\t{sequenceNo}\tNow {getBankAccountsWaitingThreadsCounter} threads awaitng!");
                    getBankAccountsAutoResetEvent.WaitOne();
                    if (getBankAccountsReponses.ContainsKey(sequenceNo))
                    {
                        return getBankAccountsReponses[sequenceNo];
                    }
                    myLogger.Log($"{GET_BANK_ACCOUNTS_CODE}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object authenticateAccountOwnerLock = new object();
        private object authenticateAccountOwnerMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(AUTHENTICATE_ACCOUNT_OWNER).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Binary, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{AUTHENTICATE_ACCOUNT_OWNER}\t{sequenceNo}\tRestart loop");
                    lock (authenticateAccountOwnerMonitorLock)
                    {
                        authenticateAccountOwnerWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{AUTHENTICATE_ACCOUNT_OWNER}\t{sequenceNo}\tNow {authenticateAccountOwnerWaitingThreadsCounter} threads awaitng!");
                    authenticateAccountOwnerAutoResetEvent.WaitOne();
                    if (authenticateAccountOwnerReponses.ContainsKey(sequenceNo))
                    {
                        return authenticateAccountOwnerReponses[sequenceNo];
                    }
                    myLogger.Log($"{AUTHENTICATE_ACCOUNT_OWNER}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object performTransferLock = new object();
        private object performTransferMonitorLock = new object();
        private readonly AutoResetEvent performTransferAutoResetEvent = new AutoResetEvent(false);
        private int performTransferWaitingThreadsCounter = 0;
        private static int performTransferSequenceNoCounter = 0;
        private Dictionary<int, TransferDataLayerCodes> performTransferReponses = new Dictionary<int, TransferDataLayerCodes>();
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
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{ownerAccountNumber};{targetAccountNumber}");
                //XmlSerializer serializer = new XmlSerializer(typeof(AccountOwnerCreationData));
                //StringWriter writer = new StringWriter();
                //serializer.Serialize(writer, accountOwnerCreationData);
                //byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
                byte[] header = Encoding.ASCII.GetBytes(TRANSFER).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{TRANSFER}\t{sequenceNo}\tRestart loop");
                    lock (performTransferMonitorLock)
                    {
                        performTransferWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{TRANSFER}\t{sequenceNo}\tNow {performTransferWaitingThreadsCounter} threads awaitng!");
                    performTransferAutoResetEvent.WaitOne();
                    if (performTransferReponses.ContainsKey(sequenceNo))
                    {
                        transferCallback(performTransferReponses[sequenceNo], ownerAccountNumber, targetAccountNumber, amount, description);
                        //return performTransferReponses[sequenceNo];
                    }
                    myLogger.Log($"{TRANSFER}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object checkForReportsUpdatesLock = new object();
        private object checkForReportsUpdatesMonitorLock = new object();
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
                byte[] header = Encoding.ASCII.GetBytes(CHECK_FOR_REPORTS_UPDATES).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    myLogger.Log($"{CHECK_FOR_REPORTS_UPDATES}\t{sequenceNo}\tRestart loop");
                    lock (checkForReportsUpdatesMonitorLock)
                    {
                        checkForReportsUpdatesWaitingThreadsCounter += 1;
                    }
                    myLogger.Log($"{CHECK_FOR_REPORTS_UPDATES}\t{sequenceNo}\tNow {checkForReportsUpdatesWaitingThreadsCounter} threads awaitng!");
                    checkForReportsUpdatesAutoResetEvent.WaitOne();
                    if (checkForReportsUpdatesReponses.ContainsKey(sequenceNo))
                    {
                        return checkForReportsUpdatesReponses[sequenceNo];
                    }
                    myLogger.Log($"{CHECK_FOR_REPORTS_UPDATES}\t{sequenceNo}\tWill repeat loop");
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }
    }
}
