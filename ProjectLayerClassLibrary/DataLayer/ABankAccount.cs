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

        private AAccountOwner accountOwner;
        public AAccountOwner AccountOwner { get { return accountOwner; } set { accountOwner = value; } }

        private float accountBalance;
        public float AccountBalance { get { return accountBalance; } }

        private ICollection<ABankAccountReport> bankAccountReports;

        public ABankAccount(string accountNumber, AAccountOwner accountOwner)
        {
            this.accountNumber = accountNumber;
            this.accountOwner = accountOwner;
            this.accountBalance = 0.0f;
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
    }
}
