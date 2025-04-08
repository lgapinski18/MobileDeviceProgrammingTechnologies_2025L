using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class ABankAccount : IIdentifiable
    {
        private int id;

        private string accountNumber;
        public string AccountNumber { get { return accountNumber; } set { accountNumber = value; } }

        private int accountOwnerId;
        public int AccountOwnerId { get { return accountOwnerId; } set { accountOwnerId = value; } }

        private AAccountOwner accountOwner;
        public AAccountOwner AccountOwner { get { return accountOwner; } set { accountOwner = value; } }

        private float accountBalance;
        public float AccountBalance { get { return accountBalance; } set { accountBalance = value; } }

        protected ICollection<ABankAccountReport> bankAccountReports;

        public ABankAccount(int id, string accountNumber, int accountOwnerId)
        {
            this.id = id;
            AccountNumber = accountNumber;
            AccountOwnerId = accountOwnerId;
            AccountOwner = null;
            AccountBalance = 0.0f;
        }

        public ABankAccount(int id, string accountNumber, AAccountOwner accountOwner)
        {
            this.id = id;
            AccountNumber = accountNumber;
            AccountOwnerId = accountOwner.GetId();
            AccountOwner = accountOwner;
            AccountBalance = 0.0f;
        }

        internal static ABankAccount? CreateBankAccountFromXml(BankAccountDto? bankAccountDto)
        {
            if (bankAccountDto == null)
            {
                return null;
            }
            ABankAccount bankAccount = new BasicBankAccount(bankAccountDto.Id, bankAccountDto.AccountNumber, bankAccountDto.OwnerId);
            bankAccount.AccountBalance = bankAccountDto.Balance;
            return bankAccount;
        }

        public abstract void IncreaseAccountBalance(float amount);

        public abstract void DecreaseAccountBalance(float amount);

        public abstract ICollection<ABankAccountReport> GetBankAccountReports();

        public abstract ABankAccountReport GenerateBankAccountReport();

        public int GetId()
        {
            return id;
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            ABankAccount other = (ABankAccount)obj;
            return id == other.id
                   && accountBalance == other.accountBalance
                   && accountNumber == other.accountNumber
                   && accountOwnerId.Equals(other.accountOwnerId);
        }
    }
}
