﻿using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.LogicLayer;
using ProjectLayerClassLibrary.LogicLayer.Exceptions;
using ProjectLayerClassLibrary.LogicLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectLayerClassLibrary.LogicLayer.ALogicLayer;

namespace ProjectLayerClassLibraryTest.LogicLayerTest
{
    [TestClass]
    public class ComunicatingWithServerLogicLayerTest
    {
        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldCreateCorrectAccount(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new ComunicatingWithServerLogicLayer(dataLayer);
            Assert.IsNotNull(logicLayer);

            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);
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
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldAddedExists(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new ComunicatingWithServerLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwnerReturned = logicLayer.GetAccountOwner(logicAccountOwner.OwnerLogin);
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
            ALogicLayer logicLayer = new ComunicatingWithServerLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.IsTrue(logicLayer.AuthenticateAccountOwner(logicAccountOwner.OwnerLogin, logicAccountOwner.OwnerPassword));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678", "87654321")]
        public void shouldAutheticationFail(string name, string surname, string email, string password, string failPassword)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new ComunicatingWithServerLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.IsFalse(logicLayer.AuthenticateAccountOwner(logicAccountOwner.OwnerLogin, failPassword));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldAutheticationThrowsException(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new ComunicatingWithServerLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.ThrowsException<ArgumentNullException>(() => logicLayer.AuthenticateAccountOwner(null, logicAccountOwner.OwnerPassword));
            Assert.ThrowsException<ArgumentNullException>(() => logicLayer.AuthenticateAccountOwner(logicAccountOwner.OwnerLogin, null));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void ShouldChangesInReportsExists(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new ComunicatingWithServerLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);

            Assert.IsTrue(logicLayer.CheckForReportsUpdates(logicAccountOwner.GetId()));
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void ShouldCreateAnotherBankAccount(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            ALogicLayer logicLayer = new ComunicatingWithServerLogicLayer(dataLayer);
            CreationAccountOwnerFlags creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out creationAccountOwnerFlags);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = logicLayer.OpenNewBankAccount(logicAccountOwner.GetId());
            Assert.IsNotNull(logicBankAccount);
            Assert.AreEqual(logicAccountOwner.DataLayerAccountOwner, logicBankAccount.AccountOwner.DataLayerAccountOwner);

            ICollection<ProjectLayerClassLibrary.LogicLayer.ABankAccount> bankAccounts = logicLayer.GetAccountOwnerBankAccounts(logicAccountOwner.GetId());
        }
    }
}
