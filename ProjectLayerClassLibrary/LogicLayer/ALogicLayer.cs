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
            EMPTY = 0,
            SUCCESS = 1,
            INCORRECT_NAME = 2,
            INCORRECT_SURNAME = 3,
            INCORRECT_EMAIL = 4,
            INCORRECT_PASSWORD = 5,
        }

        public delegate void TransferCallback(TransferCodes transferResult, string ownerAccountNumber, string targetAccountNumber, float amount, string description);

        public static ALogicLayer CreateLogicLayerInstance(ADataLayer? dataLayer = default)
        {
            return new Implementations.BasicLogicLayer(dataLayer);
        }

        public abstract AAccountOwner? CreateNewAccountOwner(string name, string surname, string email, string  password, out CreationAccountOwnerFlags creationAccountOwnerFlags);
        public abstract ABankAccount? OpenNewBankAccount(int ownerId);

        public abstract bool AuthenticateAccountOwner(string login, string password);
        public abstract AAccountOwner? GetAccountOwner(int ownerId);
        public abstract AAccountOwner? GetAccountOwner(string login);
        public abstract ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId);

        public abstract void PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description, TransferCallback transferCallback);
    }
}
