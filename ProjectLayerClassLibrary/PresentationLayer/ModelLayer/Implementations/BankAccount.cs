using ProjectLayerClassLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer.Implementations
{
    internal class BankAccount : IBankAccount
    {
        private ABankAccount bankAccount;

        public string AccountNumber => bankAccount.AccountNumber;
        public float AccountBalance => bankAccount.AccountBalance;

        public BankAccount(ABankAccount bankAccount) 
        { 
            this.bankAccount = bankAccount;
        }
    }
}
