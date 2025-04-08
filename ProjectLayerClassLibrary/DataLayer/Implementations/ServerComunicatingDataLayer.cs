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

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class ServerComunicatingDataLayer : ADataLayer
    {
        private ClientWebSocket clientWebSocket;
        private CancellationTokenSource cts;
        private int portNo = 5000;

        private object accountOwnerLock = new object();
        private object bankAccountLock = new object();

        private bool isConnected = false;
        private object checkConnectionLock = new object();
        private object awaitingConnectionLock = new object();

  

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
                    //onMessage?.Invoke(_message);
                }
            }
            catch (Exception _ex)
            {
                //m_Log($"Connection has been broken because of an exception {_ex}");
                clientWebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Connection has been broken because of an exception", CancellationToken.None).Wait();
            }
        }

        private object createAccountOwnerLock = new object();
        private static int createAccountOwnerLockSequenceNoCounter = 0;
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
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
            ////await clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            //
            //var receiveBuffer = new byte[1024];
            //var result = clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            ////var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            ////Console.WriteLine("Received: " + Encoding.UTF8.GetString(receiveBuffer, 0, result.Count));
            throw new NotImplementedException();
        }

        private object createBankAccountLock = new object();
        private static int createBankAccountSequenceNoCounter = 0;
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
                byte[] header = Encoding.ASCII.GetBytes("_CBA").Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        private object getAccountOwnerLock = new object();
        private static int getAccountOwnerSequenceNoCounter = 0;
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
                byte[] header = Encoding.ASCII.GetBytes("_GAO").Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        private object getAccountOwnerLoginLock = new object();
        private static int getAccountOwnerLoginSequenceNoCounter = 0;
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
                byte[] header = Encoding.ASCII.GetBytes("GAOL").Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        private object getAllAccountOwnersLock = new object();
        private static int getAllAccountOwnersSequenceNoCounter = 0;
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
                byte[] header = Encoding.ASCII.GetBytes("GAAO").Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        private object getAllBankAccountsLock = new object();
        private static int getAllBankAccountsSequenceNoCounter = 0;
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
                byte[] header = Encoding.ASCII.GetBytes("GABA").Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        private object gtBankAccountLock = new object();
        private static int getBankAccountSequenceNoCounter = 0;
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
                byte[] header = Encoding.ASCII.GetBytes("GBAN").Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        private object getBankAccountsLock = new object();
        private static int getBankAccountsSequenceNoCounter = 0;
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
                byte[] header = Encoding.ASCII.GetBytes("GBAS").Concat(BitConverter.GetBytes(sequenceNo)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }
    }
}
