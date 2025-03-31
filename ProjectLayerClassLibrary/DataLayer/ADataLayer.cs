using ProjectLayerClassLibrary.DataLayer;
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
            return new Implementations.BasicDataLayer();
        }

        public abstract AAccountOwner CreateAccountOwner(int ownerId, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword);

        public abstract ABankAccount CreateBankAccount(string accountNumber, int ownerId);


        public abstract AAccountOwner? GetAccountOwner(int ownerId);
        public abstract ABankAccount? GetBankAccount(string accountNumber);
        public abstract ICollection<ABankAccount> GetBankAccounts(int ownerId);
        public abstract ICollection<AAccountOwner> GetAllAccountOwners();
        public abstract ICollection<ABankAccount> GetAllBankAccounts();
    }
}
