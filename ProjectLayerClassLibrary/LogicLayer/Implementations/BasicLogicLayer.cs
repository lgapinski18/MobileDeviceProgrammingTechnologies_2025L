using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayer : ALogicLayer
    {
        public BasicLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer == null ? ADataLayer.CreateDataLayerInstance() : dataLayer;
        }

        public override bool AuthenticateAccountOwner(int ownerId, string password)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner CreateNewAccountOwner(string name, string surname, string email, string password, out CreationAccountOwnerFlags creationAccountOwnerFlags)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner GetAccountOwner(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override ABankAccount OpenNewBankAccount(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override TransferCodes performTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description)
        {
            throw new NotImplementedException();
        }
    }
}
