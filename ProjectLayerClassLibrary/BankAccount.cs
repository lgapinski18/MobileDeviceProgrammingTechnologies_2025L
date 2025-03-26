using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary
{
    public class BankAccount
    {
        private string accountOwnerName;
        private string accountOwnerSurname;
        private float accountBalance = 0.0f;

        public BankAccount(string accountOwnerName, string accountOwnerSurname)
        {
            if (accountOwnerName == "")
            {
                throw new ArgumentException("Argument \"accountOwnerName\" is empty");
            }
            if (accountOwnerSurname == "")
            {
                throw new ArgumentException("Argument \"accountOwnerSurname\" is empty");
            }
            this.accountOwnerName = accountOwnerName;
            this.accountOwnerSurname = accountOwnerSurname;
        }

        public void setAccountOwnerName(string accountOwnerName)
        {
            if (accountOwnerName == "")
            {
                throw new ArgumentException("Argument \"accountOwnerName\" is empty");
            }
            this.accountOwnerName = accountOwnerName;
        }

        public void setAccountOwnerSurname(string accountOwnerSurname)
        {
            if (accountOwnerSurname == "")
            {
                throw new ArgumentException("Argument \"accountOwnerSurname\" is empty");
            }
            this.accountOwnerSurname = accountOwnerSurname;
        }

        public string getAccountOwnerName()
        {
            return accountOwnerName;
        }

        public string getAccountOwnerSurname() 
        { 
            return accountOwnerSurname; 
        }

        public float getAccountBalance()
        {
            return accountBalance;
        }

        public bool withdraw(float withdrawSum)
        {
            if (withdrawSum < 0)
            {
                throw new ArgumentException("Withdraw sum shouldn't be negtive");
            }
            if (withdrawSum <= accountBalance)
            {
                accountBalance -= withdrawSum;
                return true;
            }
            return false;
        }

        public bool deposit(float depositSum)
        {
            if (depositSum < 0)
            {
                throw new ArgumentException("Deposit sum shouldn't be negtive");
            }
            accountBalance += depositSum;
            return true;
        }
    }
}
