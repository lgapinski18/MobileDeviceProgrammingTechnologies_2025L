using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures;
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
        [DataRow(5050, "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldCreateAccountOwnerSendCorrectComunicateANdReceiveCorectResponse(int portNo, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
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
        [DataRow(5051, "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldCreateBankAccountSendCorrectComunicateANdReceiveCorectResponse(int portNo, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner accountOwner = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwner);
            ABankAccount bankAccount = dataLayer.CreateBankAccount(accountOwner.GetId());

            Assert.IsNotNull(bankAccount);
            Assert.AreEqual(accountOwner.GetId(), bankAccount.AccountOwnerId);
            Assert.AreEqual(0.0f, bankAccount.AccountBalance);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5052, "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldGetAccountOwnerSendCorrectComunicateANdReceiveCorectResponse(int portNo, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner accountOwnerC = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwnerC);
            AAccountOwner? accountOwner = dataLayer.GetAccountOwner(accountOwnerC.GetId());

            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(accountOwnerC.GetId(), accountOwner.GetId());
            Assert.AreEqual(accountOwnerC.OwnerLogin, accountOwner.OwnerLogin);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5053, "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldGetAccountOwnerByLoginSendCorrectComunicateANdReceiveCorectResponse(int portNo, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner accountOwnerC = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwnerC);
            AAccountOwner? accountOwner = dataLayer.GetAccountOwner(accountOwnerC.OwnerLogin);

            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(accountOwnerC.GetId(), accountOwner.GetId());
            Assert.AreEqual(accountOwnerC.OwnerLogin, accountOwner.OwnerLogin);
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

            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            ICollection<AAccountOwner> accountOwners = dataLayer.GetAllAccountOwners();

            Assert.IsNotNull(accountOwners);
            Assert.AreEqual(2, accountOwners.Count);

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
            Assert.AreEqual(2, bankAccounts.Count);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5056, "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldGetBankAccountSendCorrectComunicateANdReceiveCorectResponse(int portNo, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner accountOwnerC = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwnerC);
            ICollection<ABankAccount> bankAccounts = dataLayer.GetAllBankAccounts();
            ABankAccount bankAccountFirst = bankAccounts.First();
            ABankAccount bankAccount = dataLayer.GetBankAccount(bankAccountFirst.AccountNumber);

            Assert.AreEqual(bankAccountFirst.GetId(), bankAccount.GetId());
            Assert.AreEqual(bankAccountFirst.AccountOwnerId, bankAccount.AccountOwnerId);
            Assert.AreEqual(bankAccountFirst.AccountNumber, bankAccount.AccountNumber);
            Assert.AreEqual(bankAccountFirst.AccountBalance, bankAccount.AccountBalance);

            await server.Finish();
        }

        [TestMethod]
        [DataRow(5057, "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldGetBankAccountsSendCorrectComunicateANdReceiveCorectResponse(int portNo, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner accountOwnerC = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwnerC);

            ICollection<ABankAccount> bankAccounts = dataLayer.GetBankAccounts(accountOwnerC.GetId());

            Assert.IsNotNull(bankAccounts);
            Assert.AreEqual(1, bankAccounts.Count);

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
        [DataRow(5059, "asdasfasfasf", "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldPerformTransferOwnerSendCorrectComunicate(int portNo,string description, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(portNo, ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer.CreateLogicLayerInstance());

            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);

            AAccountOwner accountOwner = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwner);
            ABankAccount bankAccount = dataLayer.CreateBankAccount(accountOwner.GetId());

            ICollection<ABankAccount> bankAccounts = dataLayer.GetBankAccounts(accountOwner.GetId());

            //Task task = Task.Run(() => dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags));
            dataLayer.PerformTransfer(bankAccounts.First().AccountNumber, bankAccounts.Last().AccountNumber, 0.0f, description, (tc, oAN, tAN, a, d) => {
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

            Assert.IsFalse(dataLayer.CheckForReportsUpdates(ownerId));

            await server.Finish();
        }
    }
}
