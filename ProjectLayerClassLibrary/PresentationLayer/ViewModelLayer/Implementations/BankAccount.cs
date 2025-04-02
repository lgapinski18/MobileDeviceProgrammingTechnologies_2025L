using ProjectLayerClassLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Implementations
{
    internal class BankAccount : IBankAccount
    {
        private ModelLayer.IBankAccount bankAccount;

        public string AccountNumber => bankAccount.AccountNumber;
        public float AccountBalance => bankAccount.AccountBalance;

        public BankAccount(ModelLayer.IBankAccount bankAccount) 
        { 
            this.bankAccount = bankAccount;
        }
    }
}
