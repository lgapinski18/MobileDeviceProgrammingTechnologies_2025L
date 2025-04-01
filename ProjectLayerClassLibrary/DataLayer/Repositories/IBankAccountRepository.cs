using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Repositories
{
    public interface IBankAccountRepository : IRepository<ABankAccount>
    {
        public ABankAccount? GetByAccountNumber(string accountNumber);
        public ICollection<ABankAccount> GetByAccountOwnerId(int ownerId);
    }
}
