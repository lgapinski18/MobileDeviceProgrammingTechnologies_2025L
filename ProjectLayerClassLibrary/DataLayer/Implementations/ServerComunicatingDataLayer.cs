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

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class ServerComunicatingDataLayer : ADataLayer
    {
        private ClientWebSocket clientWebSocket;
        private CancellationTokenSource cts;
        private int portNo = 8080;

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
  

        public ServerComunicatingDataLayer()
        {
            clientWebSocket = new ClientWebSocket();
            cts = new CancellationTokenSource();

            SetUp();
        }

        public void Dispose()
        {
            lock (checkConnectionLock)
            {
                isConnected = false;
            }
            cts.Cancel();
            clientWebSocket.Dispose();
            cts.Dispose();
        }

        public async void SetUp()
        {
            if (!isConnected)
            {
                await clientWebSocket.ConnectAsync(new Uri($"ws://localhost:{portNo}/ws"), CancellationToken.None);
                lock (checkConnectionLock)
                {
                    isConnected = true;
                }
            }
        }

        private void OnClose()
        {

        }

        //private async Task ReceiveLoopAsync()
        //{
        //    var buffer = new byte[1024 * 4];
        //
        //    while (clientWebSocket.State == WebSocketState.Open)
        //    {
        //        var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
        //
        //        if (result.MessageType == WebSocketMessageType.Close)
        //        {
        //            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
        //        }
        //        else
        //        {
        //            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //            //onMessage?.Invoke(message); // 🔔 Call your callback
        //        }
        //    }
        //}

        private void ClientReceiveLoop()
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
                    processReceivedData((byte[])buffer.Clone(), count);
                    //onMessage?.Invoke(_message);
                }
            }
            catch (Exception _ex)
            {
                //m_Log($"Connection has been broken because of an exception {_ex}");
                clientWebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Connection has been broken because of an exception", CancellationToken.None).Wait();
            }
        }

        private void processReceivedData(byte[] data, int count)
        {
            string respondType = Encoding.UTF8.GetString(data, 0, 4);
            int sequenceNo = BitConverter.ToInt32(data, 4);
            int resultCode = BitConverter.ToInt32(data, 8);
            int dataSize = BitConverter.ToInt32(data, 12);

            switch (respondType)
            {
                case CREATE_ACCOUNT_OWNER_CODE:
                    //createAccountOwnerReponses.Add(sequenceNo, );
                    Monitor.PulseAll(createAccountOwnerMonitorLock);
                    break;
            
                case CREATE_BANK_ACCOUNT_CODE:
                    //createBankAccountReponses.Add(sequenceNo, );
                    Monitor.PulseAll(createBankAccountMonitorLock);
                    break;
            
                case GET_ACCOUNT_OWNER_CODE:
                    //getAccountOwnerReponses.Add(sequenceNo, );
                    Monitor.PulseAll(getAccountOwnerMonitorLock);
                    break;
            
                case GET_ACCOUNT_OWNER_LOGIN_CODE:
                    //getAccountOwnerLoginReponses.Add(sequenceNo, );
                    Monitor.PulseAll(getAccountOwnerLoginMonitorLock);
                    break;
            
                case GET_ALL_ACCOUNT_OWNERS_CODE:
                    //getAllAccountOwnersReponses.Add(sequenceNo, );
                    Monitor.PulseAll(getAllAccountOwnersMonitorLock);
                    break;
            
                case GET_BANK_ACCOUNT_CODE:
                    //gtBankAccountReponses.Add(sequenceNo, );
                    Monitor.PulseAll(gtBankAccountMonitorLock);
                    break;
            
                case GET_ALL_BANK_ACCOUNTS_CODE:
                    //getAllBankAccountsReponses.Add(sequenceNo, );
                    Monitor.PulseAll(getAllBankAccountsMonitorLock);
                    break;
            
                case GET_BANK_ACCOUNTS_CODE:
                    //getBankAccountsReponses.Add(sequenceNo, );
                    Monitor.PulseAll(getBankAccountsMonitorLock);
                    break;
            }
        }

        private object createAccountOwnerLock = new object();
        private object createAccountOwnerMonitorLock = new object();
        private static int createAccountOwnerLockSequenceNoCounter = 0;
        private Dictionary<int, AAccountOwner> createAccountOwnerReponses = new Dictionary<int, AAccountOwner>();
        public override AAccountOwner CreateAccountOwner(string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
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
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{ownerName};{ownerSurname};{ownerEmail};{ownerPassword}");
                byte[] header = Encoding.ASCII.GetBytes("_CAO").Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    Monitor.Wait(createAccountOwnerMonitorLock);
                    if (createAccountOwnerReponses.ContainsKey(sequenceNo))
                    {
                        return createAccountOwnerReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object createBankAccountLock = new object();
        private object createBankAccountMonitorLock = new object();
        private static int createBankAccountSequenceNoCounter = 0;
        private Dictionary<int, ABankAccount> createBankAccountReponses = new Dictionary<int, ABankAccount>();
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
                byte[] sendBuffer = BitConverter.GetBytes(ownerId);
                byte[] header = Encoding.ASCII.GetBytes(CREATE_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    Monitor.Wait(createBankAccountMonitorLock);
                    if (createBankAccountReponses.ContainsKey(sequenceNo))
                    {
                        return createBankAccountReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAccountOwnerLock = new object();
        private object getAccountOwnerMonitorLock = new object();
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
                byte[] sendBuffer = BitConverter.GetBytes(ownerId);
                byte[] header = Encoding.ASCII.GetBytes(GET_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    Monitor.Wait(getAccountOwnerMonitorLock);
                    if (getAccountOwnerReponses.ContainsKey(sequenceNo))
                    {
                        return getAccountOwnerReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAccountOwnerLoginLock = new object();
        private object getAccountOwnerLoginMonitorLock = new object();
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
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{ownerLogin}");
                byte[] header = Encoding.ASCII.GetBytes(GET_ACCOUNT_OWNER_LOGIN_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    Monitor.Wait(getAccountOwnerLoginMonitorLock);
                    if (getAccountOwnerLoginReponses.ContainsKey(sequenceNo))
                    {
                        return getAccountOwnerLoginReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAllAccountOwnersLock = new object();
        private object getAllAccountOwnersMonitorLock = new object();
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
                    Monitor.Wait(getAllAccountOwnersMonitorLock);
                    if (getAllAccountOwnersReponses.ContainsKey(sequenceNo))
                    {
                        return getAllAccountOwnersReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getAllBankAccountsLock = new object();
        private object getAllBankAccountsMonitorLock = new object();
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
                    Monitor.Wait(getAllBankAccountsMonitorLock);
                    if (getAllBankAccountsReponses.ContainsKey(sequenceNo))
                    {
                        return getAllBankAccountsReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object gtBankAccountLock = new object();
        private object gtBankAccountMonitorLock = new object();
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
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{accountNumber}");
                byte[] header = Encoding.ASCII.GetBytes(GET_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    Monitor.Wait(gtBankAccountMonitorLock);
                    if (gtBankAccountReponses.ContainsKey(sequenceNo))
                    {
                        return gtBankAccountReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object getBankAccountsLock = new object();
        private object getBankAccountsMonitorLock = new object();
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
                byte[] sendBuffer = BitConverter.GetBytes(ownerId);
                byte[] header = Encoding.ASCII.GetBytes(GET_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    Monitor.Wait(getBankAccountsMonitorLock);
                    if (getBankAccountsReponses.ContainsKey(sequenceNo))
                    {
                        return getBankAccountsReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }

        private object authenticateAccountOwnerLock = new object();
        private object authenticateAccountOwnerMonitorLock = new object();
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
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{login};{password}");
                byte[] header = Encoding.ASCII.GetBytes(AUTHENTICATE_ACCOUNT_OWNER).Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                while (true)
                {
                    Monitor.Wait(authenticateAccountOwnerMonitorLock);
                    if (authenticateAccountOwnerReponses.ContainsKey(sequenceNo))
                    {
                        return authenticateAccountOwnerReponses[sequenceNo];
                    }
                }
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
        }
    }
}
