using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProjectLayerClassLibrary.DataLayer.Repositories;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations.Repositories
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
            return (ICollection<ABankAccount>)entities.Where(bankAccount => bankAccount.AccountOwner.GetId() == ownerId);
        }
    }
}
