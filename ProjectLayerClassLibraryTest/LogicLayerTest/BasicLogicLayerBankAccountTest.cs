using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.LogicLayer.Implementations;
using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.LogicLayer;
using ProjectLayerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectLayerClassLibrary.DataLayer.Exceptions;

namespace ProjectLayerClassLibraryTest.LogicLayerTest
{
    [TestClass]
    public class BasicLogicLayerBankAccountTest
    {
        static ProjectLayerClassLibrary.DataLayer.AAccountOwner accountOwner = new BasicAccountOwner(1000, "IK123456", "Jan", "Kowalski", "jk@poczta.com", "12345678");
        static ProjectLayerClassLibrary.DataLayer.AAccountOwner accountOwner2 = new BasicAccountOwner(1001, "IK123457", "Adam", "Nowak", "an@poczta.com", "87654321");

        [TestMethod]
        [DataRow(1, "12345678")]
        public void shouldCreateCorrect(int id, string accountNumber)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccount bankAccount = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicBankAccount(id, accountNumber, accountOwner);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = ProjectLayerClassLibrary.LogicLayer.ABankAccount.CreateBankAccount(bankAccount);
            Assert.IsNotNull(logicBankAccount);
            Assert.AreEqual(id, logicBankAccount.GetId());
            Assert.AreEqual(accountNumber, logicBankAccount.AccountNumber);
            Assert.AreEqual(0.0f, logicBankAccount.AccountBalance);
            Assert.AreEqual(accountOwner, logicBankAccount.AccountOwner.DataLayerAccountOwner);
        }

        [TestMethod]
        [DataRow(1, "12345678", 2, "87654321", 100.0f)]
        public void shouldChangeEveryProperty(int id, string accountNumber, int id2, string accountNumber2, float accountBalance2)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccount bankAccount = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicBankAccount(id, accountNumber, accountOwner);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = ProjectLayerClassLibrary.LogicLayer.ABankAccount.CreateBankAccount(bankAccount);
            Assert.IsNotNull(logicBankAccount);
            logicBankAccount.SetId(id2);
            logicBankAccount.AccountNumber = accountNumber2;
            logicBankAccount.AccountBalance = 100.0f;
            logicBankAccount.AccountOwner = ProjectLayerClassLibrary.LogicLayer.AAccountOwner.CreateAccountOwner(accountOwner2);

            Assert.AreEqual(id2, bankAccount.GetId());
            Assert.AreEqual(accountNumber2, logicBankAccount.AccountNumber);
            Assert.AreEqual(accountBalance2, logicBankAccount.AccountBalance);
            Assert.AreEqual(accountOwner2, logicBankAccount.AccountOwner.DataLayerAccountOwner);
        }

        [TestMethod]
        [DataRow(1, "12345678", 100.0f)]
        public void shouldIncreaseBalance(int id, string accountNumber, float change)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccount bankAccount = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicBankAccount(id, accountNumber, accountOwner);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = ProjectLayerClassLibrary.LogicLayer.ABankAccount.CreateBankAccount(bankAccount);
            Assert.AreEqual(0.0f, logicBankAccount.AccountBalance);
            logicBankAccount.IncreaseAccountBalance(Math.Abs(change));
            Assert.AreEqual(Math.Abs(change), logicBankAccount.AccountBalance);
        }

        [TestMethod]
        [DataRow(1, "12345678", -100.0f)]
        public void shouldNotIncreaseBalanceThrowsException(int id, string accountNumber, float change)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccount bankAccount = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicBankAccount(id, accountNumber, accountOwner);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = ProjectLayerClassLibrary.LogicLayer.ABankAccount.CreateBankAccount(bankAccount);
            Assert.AreEqual(0.0f, logicBankAccount.AccountBalance);
            Assert.ThrowsException<ArgumentException>(() => logicBankAccount.IncreaseAccountBalance(-Math.Abs(change)));
        }

        [TestMethod]
        [DataRow(1, "12345678", 100.0f)]
        public void shouldDecreaseBalance(int id, string accountNumber, float change)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccount bankAccount = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicBankAccount(id, accountNumber, accountOwner);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = ProjectLayerClassLibrary.LogicLayer.ABankAccount.CreateBankAccount(bankAccount);
            Assert.AreEqual(0.0f, logicBankAccount.AccountBalance);
            logicBankAccount.IncreaseAccountBalance(2 * Math.Abs(change));
            logicBankAccount.DecreaseAccountBalance(Math.Abs(change));
            Assert.AreEqual(Math.Abs(change), logicBankAccount.AccountBalance);
        }

        [TestMethod]
        [DataRow(1, "12345678", -100.0f)]
        public void shouldNotDecreaseBalanceThrowsExceptionNegativeAmount(int id, string accountNumber, float change)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccount bankAccount = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicBankAccount(id, accountNumber, accountOwner);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = ProjectLayerClassLibrary.LogicLayer.ABankAccount.CreateBankAccount(bankAccount);
            Assert.AreEqual(0.0f, logicBankAccount.AccountBalance);
            logicBankAccount.IncreaseAccountBalance(2 * Math.Abs(change));
            Assert.ThrowsException<ArgumentException>(() => logicBankAccount.DecreaseAccountBalance(-Math.Abs(change)));
        }

        [TestMethod]
        [DataRow(1, "12345678", 100.0f)]
        public void shouldNotDecreaseBalanceThrowsExceptionGreaterAmountThanBalance(int id, string accountNumber, float change)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccount bankAccount = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicBankAccount(id, accountNumber, accountOwner);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = ProjectLayerClassLibrary.LogicLayer.ABankAccount.CreateBankAccount(bankAccount);
            Assert.AreEqual(0.0f, logicBankAccount.AccountBalance);
            Assert.ThrowsException<InvalidBankAccountOperationException>(() => logicBankAccount.DecreaseAccountBalance(Math.Abs(change)));
        }

        [TestMethod]
        [DataRow(1, "12345678")]
        public void shouldCreateAdditionalReport(int id, string accountNumber)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccount bankAccount = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicBankAccount(id, accountNumber, accountOwner);
            ProjectLayerClassLibrary.LogicLayer.ABankAccount? logicBankAccount = ProjectLayerClassLibrary.LogicLayer.ABankAccount.CreateBankAccount(bankAccount);
            Assert.AreEqual(1, logicBankAccount.GetBankAccountReports().Count);
            logicBankAccount.GenerateBankAccountReport();
            Assert.AreEqual(2, logicBankAccount.GetBankAccountReports().Count);
        }
    }
}
