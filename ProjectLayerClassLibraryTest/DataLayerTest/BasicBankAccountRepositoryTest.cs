using ProjectLayerClassLibrary.DataLayer.Implementations.Repositories;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.DataLayer.Repositories;
using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ProjectLayerClassLibraryTest.DataLayerTest
{
    [TestClass]
    public class BasicBankAccountRepositoryTest
    {
        static AAccountOwner accountOwner = new BasicAccountOwner(1000, "IK123456", "Jan", "Kowalski", "jk@poczta.com", "12345678");
        static ABankAccount bankAccount = new BasicBankAccount(1, "12345678", accountOwner);

        [TestMethod]
        void shouldCreateCorrectAndEmpty()
        {
            IBankAccountRepository bankAccountRepository = new BasicBankAccountRepository();
            Assert.AreEqual(0, bankAccountRepository.GetAll().Count());
        }

        [TestMethod]
        public void shouldGetByIdReturn()
        {
            IBankAccountRepository bankAccountRepository = new BasicBankAccountRepository();
            bankAccountRepository.Save(bankAccount);
            Assert.AreEqual(1, bankAccountRepository.GetAll().Count());
        }

        [TestMethod]
        public void shouldGetByIdReturnEqual()
        {
            IBankAccountRepository bankAccountRepository = new BasicBankAccountRepository();
            bankAccountRepository.Save(bankAccount);
            ABankAccount? returned = bankAccountRepository.Get(bankAccount.GetId());
            Assert.IsNotNull(returned);
            Assert.AreEqual(bankAccount, returned);
        }

        [TestMethod]
        public void shouldGetByAccountNumberReturnEqual()
        {
            IBankAccountRepository bankAccountRepository = new BasicBankAccountRepository();
            bankAccountRepository.Save(bankAccount);
            ABankAccount? returned = bankAccountRepository.GetByAccountNumber(bankAccount.AccountNumber);
            Assert.IsNotNull(returned);
            Assert.AreEqual(bankAccount, returned);
        }

        [TestMethod]
        public void shouldGetByAccountOwnerIdReturnEqual()
        {
            IBankAccountRepository bankAccountRepository = new BasicBankAccountRepository();
            bankAccountRepository.Save(bankAccount);
            ICollection<ABankAccount> accounts = bankAccountRepository.GetByAccountOwnerId(accountOwner.GetId());
            Assert.IsNotNull(accounts);
            Assert.AreEqual(1, accounts.Count());
        }

        [TestMethod]
        public void shouldRemoveById()
        {
            IBankAccountRepository bankAccountRepository = new BasicBankAccountRepository();
            bankAccountRepository.Save(bankAccount);
            Assert.IsTrue(bankAccountRepository.Remove(bankAccount.GetId()));
            Assert.AreEqual(0, bankAccountRepository.GetAll().Count());
        }

        [TestMethod]
        public void shouldRemoveByObjectReference()
        {
            IBankAccountRepository bankAccountRepository = new BasicBankAccountRepository();
            bankAccountRepository.Save(bankAccount);
            Assert.IsTrue(bankAccountRepository.Remove(bankAccount));
            Assert.AreEqual(0, bankAccountRepository.GetAll().Count());
        }
    }
}
