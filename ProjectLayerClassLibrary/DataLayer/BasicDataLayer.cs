using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    internal class BasicDataLayer : ADataLayer
    {
        public BasicDataLayer()
        {

        }

        public override AAccountOwner GetAccountOwner(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override ICollection<AAccountOwner> GetAccountOwners()
        {
            throw new NotImplementedException();
        }

        public override ABankAccount GetBankAccount(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetBankAccounts()
        {
            throw new NotImplementedException();
        }
    }
}
