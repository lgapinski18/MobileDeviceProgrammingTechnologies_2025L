using ProjectLayerClassServerLibrary.DataLayer;
using ProjectLayerClassServerLibrary.DataLayer.Implementations;
using ProjectLayerClassServerLibrary.LogicLayer;
using ProjectLayerClassServerLibrary.LogicLayer.Exceptions;
using ProjectLayerClassServerLibrary.LogicLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer;

namespace ProjectLayerClassServerLibraryTest.LogicLayerTest
{
    [TestClass]
    public class BasicLogicLayerTest
    {
        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldCreateCorrectAccount(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            Assert.IsNotNull(logicLayer);

            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_NAME) == 0);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_SURNAME) == 0);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_EMAIL) == 0);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_PASSWORD) == 0);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.SUCCESS) != 0);
            Assert.IsNotNull(logicAccountOwner);
            Assert.AreEqual(name, logicAccountOwner.OwnerName);
            Assert.AreEqual(surname, logicAccountOwner.OwnerSurname);
            Assert.AreEqual(email, logicAccountOwner.OwnerEmail);
            Assert.AreEqual(password, logicAccountOwner.OwnerPassword);
        }

        [TestMethod]
        [DataRow("aJn", "oKwalski", "jk*poczta.com", "12344")]
        [DataRow("123aJn", "oKw1231alski", "jk@poczta.co1214m", "1")]
        [DataRow("123aJn", "oKw1231alski", "jk@pocztacom", "1")]
        public void shouldCreationAccountOwnerFailedDueToEveryParameterWrong(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            Assert.IsNotNull(logicLayer);

            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_NAME) != 0);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_SURNAME) != 0);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_EMAIL) != 0);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_PASSWORD) != 0);
            Assert.IsTrue((creationAccountOwnerFlags & CreationAccountOwnerFlags.SUCCESS) == 0);
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldAddedExists(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwnerReturned = logicLayer.GetAccountOwner(logicAccountOwner.OwnerLogin);
            Assert.IsNotNull(logicAccountOwnerReturned);
            Assert.AreEqual(logicAccountOwner.DataLayerAccountOwner, logicAccountOwnerReturned.DataLayerAccountOwner);
            logicAccountOwnerReturned = logicLayer.GetAccountOwner(logicAccountOwner.GetId());
            Assert.IsNotNull(logicAccountOwnerReturned);
            Assert.AreEqual(logicAccountOwner.DataLayerAccountOwner, logicAccountOwnerReturned.DataLayerAccountOwner);
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldAutheticationPass(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.IsTrue(logicLayer.AuthenticateAccountOwner(logicAccountOwner.OwnerLogin, logicAccountOwner.OwnerPassword));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678", "87654321")]
        public void shouldAutheticationFail(string name, string surname, string email, string password, string failPassword)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.IsFalse(logicLayer.AuthenticateAccountOwner(logicAccountOwner.OwnerLogin, failPassword));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678", "Ik000000")]
        public void shouldAutheticationFailThrowsException(string name, string surname, string email, string password, string failLogin)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.ThrowsException<ThereIsNoSuchOwnerException>(() => logicLayer.AuthenticateAccountOwner(failLogin, logicAccountOwner.OwnerPassword));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldAutheticationThrowsException(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.ThrowsException<ArgumentNullException>(() => logicLayer.AuthenticateAccountOwner(null, logicAccountOwner.OwnerPassword));
            Assert.ThrowsException<ArgumentNullException>(() => logicLayer.AuthenticateAccountOwner(logicAccountOwner.OwnerLogin, null));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void ShouldChangesInReportsExists(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.IsFalse(logicLayer.CheckForReportsUpdates(logicAccountOwner.GetId()));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void ShouldCreateAnotherBankAccount(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new BasicLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassServerLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);
            ProjectLayerClassServerLibrary.LogicLayer.ABankAccount? logicBankAccount = logicLayer.OpenNewBankAccount(logicAccountOwner.GetId());
            Assert.IsNotNull(logicBankAccount);
            Assert.AreEqual(logicAccountOwner.DataLayerAccountOwner, logicBankAccount.AccountOwner.DataLayerAccountOwner);

            ICollection<ProjectLayerClassServerLibrary.LogicLayer.ABankAccount> bankAccounts = logicLayer.GetAccountOwnerBankAccounts(logicAccountOwner.GetId());
        }

    }
}
