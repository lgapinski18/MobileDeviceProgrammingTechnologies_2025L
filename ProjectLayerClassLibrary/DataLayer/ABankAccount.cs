using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class ABankAccount
    {
        private string accountNumber;
        public string AccountNumber { get { return accountNumber; } set { accountNumber = value; } }

        private AAccountOwner accountOwner;
        public AAccountOwner AccountOwner { get { return accountOwner; } set { accountOwner = value; } }

        private float accountBalance;
        public float AccountBalance { get { return accountBalance; } }

        ICollection<ABankAccountRaport> bankAccountRaports;

        public ABankAccount(string accountNumber, AAccountOwner accountOwner)
        {
            this.accountNumber = accountNumber;
            this.accountOwner = accountOwner;
        }

        public abstract void IncreaseAccountBalance(float amount);

        public abstract void DecreaseAccountBalance(float amount);
    }
}
