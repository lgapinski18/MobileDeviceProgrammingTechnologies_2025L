using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Exceptions;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibraryTest.DataLayerTest
{
    [TestClass]
    public class BasicBankAccountTest
    {
        static AAccountOwner accountOwner = new BasicAccountOwner(1000, "IK123456", "Jan", "Kowalski", "jk@poczta.com", "12345678");
        static AAccountOwner accountOwner2 = new BasicAccountOwner(1001, "IK123457", "Adam", "Nowak", "an@poczta.com", "87654321");

        [TestMethod]
        [DataRow(1, "12345678")]
        public void shouldCreateCorrect(int id, string accountNumber)
        {
            ABankAccount bankAccount = new BasicBankAccount(id, accountNumber, accountOwner);
            Assert.AreEqual(id, bankAccount.GetId());
            Assert.AreEqual(accountNumber, bankAccount.AccountNumber);
            Assert.AreEqual(0.0f, bankAccount.AccountBalance);
            Assert.AreEqual(accountOwner, bankAccount.AccountOwner);
        }

        [TestMethod]
        [DataRow(1, "12345678", 2, "87654321", 100.0f)]
        public void shouldChangeEveryProperty(int id, string accountNumber, int id2, string accountNumber2, float accountBalance2)
        {
            ABankAccount bankAccount = new BasicBankAccount(id, accountNumber, accountOwner);
            bankAccount.SetId(id2);
            bankAccount.AccountNumber = accountNumber2;
            bankAccount.AccountBalance = 100.0f;
            bankAccount.AccountOwner = accountOwner2;

            Assert.AreEqual(id2, bankAccount.GetId());
            Assert.AreEqual(accountNumber2, bankAccount.AccountNumber);
            Assert.AreEqual(accountBalance2, bankAccount.AccountBalance);
            Assert.AreEqual(accountOwner2, bankAccount.AccountOwner);
        }

        [TestMethod]
        [DataRow(1, "12345678", 100.0f)]
        public void shouldIncreaseBalance(int id, string accountNumber, float change)
        {
            ABankAccount bankAccount = new BasicBankAccount(id, accountNumber, accountOwner);
            Assert.AreEqual(0.0f, bankAccount.AccountBalance);
            bankAccount.IncreaseAccountBalance(Math.Abs(change));
            Assert.AreEqual(Math.Abs(change), bankAccount.AccountBalance);
        }

        [TestMethod]
        [DataRow(1, "12345678", -100.0f)]
        public void shouldNotIncreaseBalanceThrowsException(int id, string accountNumber, float change)
        {
            ABankAccount bankAccount = new BasicBankAccount(id, accountNumber, accountOwner);
            Assert.AreEqual(0.0f, bankAccount.AccountBalance);
            Assert.ThrowsException<ArgumentException>(() => bankAccount.IncreaseAccountBalance(-Math.Abs(change)));
        }

        [TestMethod]
        [DataRow(1, "12345678", 100.0f)]
        public void shouldDecreaseBalance(int id, string accountNumber, float change)
        {
            ABankAccount bankAccount = new BasicBankAccount(id, accountNumber, accountOwner);
            Assert.AreEqual(0.0f, bankAccount.AccountBalance);
            bankAccount.IncreaseAccountBalance(2 * Math.Abs(change));
            bankAccount.DecreaseAccountBalance(Math.Abs(change));
            Assert.AreEqual(Math.Abs(change), bankAccount.AccountBalance);
        }

        [TestMethod]
        [DataRow(1, "12345678", -100.0f)]
        public void shouldNotDecreaseBalanceThrowsExceptionNegativeAmount(int id, string accountNumber, float change)
        {
            ABankAccount bankAccount = new BasicBankAccount(id, accountNumber, accountOwner);
            Assert.AreEqual(0.0f, bankAccount.AccountBalance);
            bankAccount.IncreaseAccountBalance(2 * Math.Abs(change));
            Assert.ThrowsException<ArgumentException>(() => bankAccount.DecreaseAccountBalance(-Math.Abs(change)));
        }

        [TestMethod]
        [DataRow(1, "12345678", 100.0f)]
        public void shouldNotDecreaseBalanceThrowsExceptionGreaterAmountThanBalance(int id, string accountNumber, float change)
        {
            ABankAccount bankAccount = new BasicBankAccount(id, accountNumber, accountOwner);
            Assert.AreEqual(0.0f, bankAccount.AccountBalance);
            Assert.ThrowsException<InvalidBankAccountOperationException>(() => bankAccount.DecreaseAccountBalance(Math.Abs(change)));
        }

        [TestMethod]
        [DataRow(1, "12345678")]
        public void shouldCreateAdditionalReport(int id, string accountNumber)
        {
            ABankAccount bankAccount = new BasicBankAccount(id, accountNumber, accountOwner);
            Assert.AreEqual(1, bankAccount.GetBankAccountReports().Count);
            bankAccount.GenerateBankAccountReport();
            Assert.AreEqual(2, bankAccount.GetBankAccountReports().Count);
        }
    }
}
