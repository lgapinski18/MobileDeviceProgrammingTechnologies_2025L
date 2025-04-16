using ComunicationApiXmlDto;
using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibraryTest.DataLayerTest.TestUtilities;
using ProjectLayerClassServerLibrary.Presentation.Factory;
using ProjectLayerClassServerLibrary.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassLibraryTest.ServerIntegration
{
    [TestClass]
    public class ClientDataLayerServerPresentationLayerTest
    {
        [TestMethod]
        [DataRow(5050, 1, "Jan", "IK123456", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldCreateAccountOwnerSendCorrectComunicateANdReceiveCorectResponse(int portNo, int id, string ownerName, string ownerLogin, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner accountOwner = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);

            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5051, 1, 1, "18273645", 100.0f)]
        public async Task ShouldCreateBankAccountSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId, int accountId, string accountNumber, float balance)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            ABankAccount bankAccount = dataLayer.CreateBankAccount(ownerId);

            Assert.IsNotNull(bankAccount);
            Assert.AreEqual(accountId, bankAccount.GetId());
            Assert.AreEqual(ownerId, bankAccount.AccountOwnerId);
            Assert.AreEqual(accountNumber, bankAccount.AccountNumber);
            Assert.AreEqual(balance, bankAccount.AccountBalance);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5052, 1, "Jan", "IK123456", "Kowalski", "jk@poczta.com")]
        public async Task ShouldGetAccountOwnerSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId, string ownerName, string ownerLogin, string ownerSurname, string ownerEmail)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner? accountOwner = dataLayer.GetAccountOwner(ownerId);

            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(ownerId, accountOwner.GetId());
            Assert.AreEqual(ownerLogin, accountOwner.OwnerLogin);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5053, 1, "Jan", "IK123456", "Kowalski", "jk@poczta.com")]
        public async Task ShouldGetAccountOwnerByLoginSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId, string ownerName, string ownerLogin, string ownerSurname, string ownerEmail)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner? accountOwner = dataLayer.GetAccountOwner(ownerLogin);

            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(ownerId, accountOwner.GetId());
            Assert.AreEqual(ownerLogin, accountOwner.OwnerLogin);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5054)]
        public async Task ShouldGetAllAccountOwnersSendCorrectComunicateANdReceiveCorectResponse(int portNo)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            ICollection<AAccountOwner> accountOwners = dataLayer.GetAllAccountOwners();

            Assert.IsNotNull(accountOwners);
            Assert.AreEqual(0, accountOwners.Count);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5055)]
        public async Task ShouldGetAllBankAccountsSendCorrectComunicateANdReceiveCorectResponse(int portNo)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            ICollection<ABankAccount> bankAccounts = dataLayer.GetAllBankAccounts();

            Assert.IsNotNull(bankAccounts);
            Assert.AreEqual(0, bankAccounts.Count);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5056, 1, 1, "18273645", 100.0f)]
        public async Task ShouldGetBankAccountSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId, int accountId, string accountNumber, float balance)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            ABankAccount bankAccount = dataLayer.GetBankAccount(accountNumber);

            Assert.AreEqual(accountId, bankAccount.GetId());
            Assert.AreEqual(ownerId, bankAccount.AccountOwnerId);
            Assert.AreEqual(accountNumber, bankAccount.AccountNumber);
            Assert.AreEqual(balance, bankAccount.AccountBalance);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5057, 1)]
        public async Task ShouldGetBankAccountsSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            ICollection<ABankAccount> bankAccounts = dataLayer.GetBankAccounts(ownerId);

            Assert.IsNotNull(bankAccounts);
            Assert.AreEqual(0, bankAccounts.Count);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5058, "IK123456", "12345678")]
        public async Task ShouldAuthenticateAccountOwnerSendCorrectComunicateANdReceiveCorectResponse(int portNo, string login, string password)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            bool b = dataLayer.AuthenticateAccountOwner(login, password);

            Assert.IsTrue(b);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5059, "87654321", "12345678", 1000.0f, "asdasfasfasf")]
        public async Task ShouldPerformTransferOwnerSendCorrectComunicate(int portNo, string ownerAccountNumber, string targetAccountNumber, float amount, string description)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);
            //Task task = Task.Run(() => dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags));
            dataLayer.PerformTransfer(ownerAccountNumber, targetAccountNumber, amount, description, (tc, oAN, tAN, a, d) => {
                Assert.AreEqual(ADataLayer.TransferResultCodes.SUCCESS, tc);
            });

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5060, 1)]
        public async Task ShouldCheckForReportsUpdatesSendCorrectComunicateeANdReceiveCorectResponse(int portNo, int ownerId)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            Assert.IsTrue(dataLayer.CheckForReportsUpdates(ownerId));

            await server.Finish();
        }
    }
}
