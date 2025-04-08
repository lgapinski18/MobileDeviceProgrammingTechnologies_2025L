using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class ComunicatingWithServerLogicLayer : ALogicLayer
    {
        private ADataLayer dataLayer;
        public ComunicatingWithServerLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer ?? ADataLayer.CreateDataLayerInstance();
        }

        public override bool AuthenticateAccountOwner(string login, string password)
        {
            throw new NotImplementedException();
        }

        public override bool CheckForReportsUpdates()
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner? CreateNewAccountOwner(string name, string surname, string email, string password, out CreationAccountOwnerFlags creationAccountOwnerFlags)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(string login)
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override ABankAccount? OpenNewBankAccount(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override Thread PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description, TransferCallback transferCallback)
        {
            throw new NotImplementedException();
        }
    }
}
