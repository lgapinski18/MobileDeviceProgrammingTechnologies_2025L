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
                await clientWebSocket.ConnectAsync(new Uri($"ws://localhost:{portNo}/"), cts.Token);
                lock (checkConnectionLock)
                {
                    isConnected = true;
                }
            }
        }

        public override AAccountOwner CreateAccountOwner(string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{ownerName};{ownerSurname};{ownerEmail};{ownerPassword}");
                byte[] header = Encoding.ASCII.GetBytes("_CAO").Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
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

        public override ABankAccount CreateBankAccount(int ownerId)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                byte[] sendBuffer = BitConverter.GetBytes(ownerId);
                byte[] header = Encoding.ASCII.GetBytes("_CBA").Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }
            throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                byte[] sendBuffer = BitConverter.GetBytes(ownerId);
                byte[] header = Encoding.ASCII.GetBytes("_GAO").Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            return null;
            //throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(string ownerLogin)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{ownerLogin}");
                byte[] header = Encoding.ASCII.GetBytes("GAOL").Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            return null;
            //throw new NotImplementedException();
        }

        public override ICollection<AAccountOwner> GetAllAccountOwners()
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                byte[] sendBuffer = [];
                byte[] header = Encoding.ASCII.GetBytes("GAAO").Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetAllBankAccounts()
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                byte[] sendBuffer = [];
                byte[] header = Encoding.ASCII.GetBytes("GABA").Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        public override ABankAccount? GetBankAccount(string accountNumber)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{accountNumber}");
                byte[] header = Encoding.ASCII.GetBytes("GBAN").Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
                sendBuffer = header.Concat(sendBuffer).ToArray();
                clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                throw new NotConnectedToAplicationServer();
            }

            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetBankAccounts(int ownerId)
        {
            bool localIsConnected = false;
            lock (checkConnectionLock)
            {
                localIsConnected = isConnected;
            }
            if (localIsConnected)
            {
                byte[] sendBuffer = BitConverter.GetBytes(ownerId);
                byte[] header = Encoding.ASCII.GetBytes("GBAS").Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
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
