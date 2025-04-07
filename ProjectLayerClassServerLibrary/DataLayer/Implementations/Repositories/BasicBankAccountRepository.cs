using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProjectLayerClassServerLibrary.DataLayer.Repositories;

[assembly: InternalsVisibleTo("ProjectLayerClassServerLibraryTest")]

namespace ProjectLayerClassServerLibrary.DataLayer.Implementations.Repositories
{
    internal class BasicBankAccountRepository : ARepository<ABankAccount>, IBankAccountRepository
    {
        public BasicBankAccountRepository()
        {
            entities = new List<ABankAccount>();
        }

        public ABankAccount? GetByAccountNumber(string accountNumber)
        {
            return entities.Where(bankAccount => bankAccount.AccountNumber == accountNumber).FirstOrDefault();
        }

        public ICollection<ABankAccount> GetByAccountOwnerId(int ownerId)
        {
            ICollection<ABankAccount> bankAccounts = new List<ABankAccount>();
            foreach (ABankAccount bankAccount in entities.Where(bankAccount => bankAccount.AccountOwner.GetId() == ownerId))
            {
                bankAccounts.Add(bankAccount);
            }
            return bankAccounts;
        }
    }
}
