using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public interface IIdentifiable
    {
<<<<<<< HEAD:ProjectLayerClassLibrary/DataLayer/IIdentifiable.cs
        public int GetId();
        public void SetId(int id);
=======
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
>>>>>>> 0db1557180464e1cc4e643bade5af6cf7731cd6d:ProjectLayerClassLibrary/DataLayer/BasicDataLayer.cs
    }
}
