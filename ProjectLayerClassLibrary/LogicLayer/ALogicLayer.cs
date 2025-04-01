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
        public enum TransferCodes
        {
            SUCCESS,
            OWNER_ACCOUNT_DOESNT_EXISTS,
            TARGET_BANK_ACCOUNT_DOESNT_EXISTS,
            INSUFICIENT_BANK_ACCOUNT_FUNDS,
            TIMEOUT,
            TRANSFER_HAS_BEEN_INTERUPTED
        }

        [Flags]
        public enum CreationAccountOwnerFlags
        {
            SUCCESS = 0,
            INCORRECT_NAME = 1,
            INCORRECT_SURNAME = 2,
            INCORRECT_EMAIL = 3,
            INCORRECT_PASSWORD = 4
        }

        protected ADataLayer dataLayer;

        public static ALogicLayer createLogicLayerInstance(ADataLayer? dataLayer = default(ADataLayer))
        {
            return new Implementations.BasicLogicLayer(dataLayer);
        }

        public abstract AAccountOwner CreateNewAccountOwner(string name, string surname, string email, string  password, out CreationAccountOwnerFlags creationAccountOwnerFlags);
        public abstract ABankAccount OpenNewBankAccount(int ownerId);

        public abstract bool AuthenticateAccountOwner(int ownerId, string password);
        public abstract AAccountOwner GetAccountOwner(int ownerId);
        public abstract ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId);

        public abstract TransferCodes performTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description);
    }
}
