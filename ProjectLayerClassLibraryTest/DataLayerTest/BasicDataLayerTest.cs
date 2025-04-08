using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectLayerClassLibrary.DataLayer.ADataLayer;

namespace ProjectLayerClassLibraryTest.DataLayerTest
{
    [TestClass]
    public class BasicDataLayerTest
    {
        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldCreateCorrectAccountOwnerAndCorrectlySave(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            AAccountOwner accountOwner = dataLayer.CreateAccountOwner(name, surname, email, password, out CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(name, accountOwner.OwnerName);
            Assert.AreEqual(surname, accountOwner.OwnerSurname);
            Assert.AreEqual(email, accountOwner.OwnerEmail);
            Assert.AreEqual(password, accountOwner.OwnerPassword);

            AAccountOwner? accountOwnerGetById = dataLayer.GetAccountOwner(accountOwner.GetId());
            Assert.IsNotNull(accountOwnerGetById);
            Assert.AreEqual(accountOwner, accountOwnerGetById);

            AAccountOwner? accountOwnerGetByLogin = dataLayer.GetAccountOwner(accountOwner.OwnerLogin);
            Assert.IsNotNull(accountOwnerGetByLogin);
            Assert.AreEqual(accountOwner, accountOwnerGetByLogin);

            ICollection<AAccountOwner> accountOwners = dataLayer.GetAllAccountOwners();
            Assert.IsNotNull(accountOwners);
            Assert.AreEqual(1, accountOwners.Count());
        }

        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldCreateCorrectBankAccountAndCorrectlySave(string name, string surname, string email, string password)
        {
            ADataLayer dataLayer = new BasicDataLayer(false);
            AAccountOwner accountOwner = dataLayer.CreateAccountOwner(name, surname, email, password, out CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags);
            ABankAccount bankAccount = dataLayer.CreateBankAccount(accountOwner.GetId());

            Assert.IsNotNull(bankAccount);
            Assert.AreEqual(accountOwner, bankAccount.AccountOwner);

            ABankAccount? bankAccountGetByAccountNumber = dataLayer.GetBankAccount(bankAccount.AccountNumber);
            Assert.IsNotNull(bankAccountGetByAccountNumber);
            Assert.AreEqual(bankAccount, bankAccountGetByAccountNumber);

            ICollection<ABankAccount> bankAccounts = dataLayer.GetBankAccounts(accountOwner.GetId());
            Assert.IsNotNull(bankAccounts);
            Assert.AreEqual(2, bankAccounts.Count());

            bankAccounts = dataLayer.GetAllBankAccounts();
            Assert.IsNotNull(bankAccounts);
            Assert.AreEqual(2, bankAccounts.Count());
        }
    }
}
