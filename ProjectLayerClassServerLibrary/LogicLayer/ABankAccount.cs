using ProjectLayerClassLibrary.LogicLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.LogicLayer
{
    public abstract class ABankAccount
    {
        protected DataLayer.ABankAccount dataLayerBankAccount;

        internal static ABankAccount? CreateBankAccount(DataLayer.ABankAccount? dataLayerBankAccount)
        {
            if (dataLayerBankAccount == null)
            {
                return null;
            }
            return new BasicLogicLayerBankAccount(dataLayerBankAccount);
        }

        public string AccountNumber { get { return dataLayerBankAccount.AccountNumber; } set { dataLayerBankAccount.AccountNumber = value; } }

        public AAccountOwner AccountOwner { get { return AAccountOwner.CreateAccountOwner(dataLayerBankAccount.AccountOwner); } set { dataLayerBankAccount.AccountOwner = value.DataLayerAccountOwner; } }

        public float AccountBalance { get { return dataLayerBankAccount.AccountBalance; } set { dataLayerBankAccount.AccountBalance = value; } }

        public ABankAccount(DataLayer.ABankAccount dataLayerBankAccount)
        {
            this.dataLayerBankAccount = dataLayerBankAccount;
        }

        public abstract void IncreaseAccountBalance(float amount);

        public abstract void DecreaseAccountBalance(float amount);

        public abstract ICollection<ABankAccountReport> GetBankAccountReports();

        public abstract ABankAccountReport GenerateBankAccountReport();

        public int GetId()
        {
            return dataLayerBankAccount.GetId();
        }

        public void SetId(int id)
        {
            dataLayerBankAccount.SetId(id);
        }
    }
}
