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

        public string Login => owner.OwnerLogin;
        public string UserName => owner.OwnerName;
        public string UserSurname => owner.OwnerSurname;
        public string UserEmail => owner.OwnerEmail;
        public ICollection<IBankAccount> BankAccounts => bankAccounts;

        public UserContext(AAccountOwner owner, ICollection<ABankAccount> bankAccounts)
        {
            this.owner = owner;
            this.bankAccounts = bankAccounts.Select(bankAccount => new BankAccount(bankAccount)).ToArray<IBankAccount>();
        }
    }
}
