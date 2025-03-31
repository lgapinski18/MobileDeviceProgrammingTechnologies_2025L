using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer
{
    public abstract class ALogicLayer
    {
        protected ADataLayer dataLayer;

        public static ALogicLayer createLogicLayerInstance(ADataLayer? dataLayer = default(ADataLayer))
        {
            return new Implementations.BasicLogicLayer(dataLayer);
        }

        public abstract bool AuthenticateAccountOwner(int ownerId, string password);
        public abstract AAccountOwner GetAccountOwner(int ownerId);
        public abstract ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId);
    }
}
