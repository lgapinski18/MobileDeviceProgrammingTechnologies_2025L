using ProjectLayerClassLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer.Implementations
{
    internal class UserContext : IUserContext
    {
        private AAccountOwner owner;
        private ICollection<IBankAccount> bankAccounts;
        private string bankAccountNumberForTransfer;
        ICollection<string> bankAccountsReports = [];

        public int Id => owner.GetId();
        public string Login => owner.OwnerLogin;
        public string UserName => owner.OwnerName;
        public string UserSurname => owner.OwnerSurname;
        public string UserEmail => owner.OwnerEmail;
        public ICollection<IBankAccount> BankAccounts => bankAccounts;

        string IUserContext.BankAccountNumberForTransfer { get => bankAccountNumberForTransfer; set => bankAccountNumberForTransfer = value; }
        ICollection<string> IUserContext.BankAccountsReports
        {
            get {
                return bankAccountsReports;
            }
            set
            {
                lock (bankAccountsReports)
                {
                    foreach (string report in value)
                    {
                        bankAccountsReports.Add(report);
                    }
                }

            }
        }

        public UserContext(AAccountOwner owner, ICollection<ABankAccount> bankAccounts)
        {
            this.owner = owner;
            this.bankAccounts = bankAccounts.Select(bankAccount => new BankAccount(bankAccount)).ToArray<IBankAccount>();
            this.bankAccountNumberForTransfer = this.bankAccounts.First().AccountNumber;
        }
    }
}
