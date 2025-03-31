using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class ADataLayer
    {
        public static ADataLayer CreateDataLayerInstance()
        {
            return new BasicDataLayer();
        }

        public abstract AAccountOwner GetAccountOwner(int ownerId);
        public abstract ICollection<AAccountOwner> GetAccountOwners();

        public abstract ABankAccount GetBankAccount(string accountNumber);
        public abstract ICollection<ABankAccount> GetBankAccounts();
    }
}
