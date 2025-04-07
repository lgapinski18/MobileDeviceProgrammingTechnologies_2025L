using ProjectLayerClassLibrary.DataLayer.Repositories;
using ProjectLayerClassLibrary.DataLayer.Exceptions;
using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class ServerComunicatingDataLayer : ADataLayer
    {
        private ClientWebSocket clientWebSocket;
        private int portNo = 5000;

        private object accountOwnerLock = new object();
        private object bankAccountLock = new object();

        private bool connected = false;
        private object awaitingConnectionLock = new object();

        public ServerComunicatingDataLayer()
        {
            clientWebSocket = new ClientWebSocket();
        }

        public void Dispose()
        {
            clientWebSocket.Dispose();
        }

        public async void SetUp()
        {
            lock (awaitingConnectionLock)
            {
            }
            await clientWebSocket.ConnectAsync(new Uri($"ws://localhost:{portNo}/ws"), CancellationToken.None);
        }

        public override AAccountOwner CreateAccountOwner(string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            var sendBuffer = Encoding.UTF8.GetBytes("Hello Server!");
            clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            //await clientWebSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

            var receiveBuffer = new byte[1024];
            var result = clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            //var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            //Console.WriteLine("Received: " + Encoding.UTF8.GetString(receiveBuffer, 0, result.Count));
            throw new NotImplementedException();
        }

        public override ABankAccount CreateBankAccount(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(string ownerLogin)
        {
            throw new NotImplementedException();
        }

        public override ICollection<AAccountOwner> GetAllAccountOwners()
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetAllBankAccounts()
        {
            throw new NotImplementedException();
        }

        public override ABankAccount? GetBankAccount(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetBankAccounts(int ownerId)
        {
            throw new NotImplementedException();
        }
    }
}
