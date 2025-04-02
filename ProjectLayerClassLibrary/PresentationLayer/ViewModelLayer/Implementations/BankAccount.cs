using ProjectLayerClassLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Implementations
{
    internal class BankAccount : IBankAccount
    {
        private ModelLayer.IBankAccount bankAccount;
        private ICommand transferCommand;
        private CreateTransferForBankAccount createTransferForBankAccountCallback;

        public string AccountNumber => bankAccount.AccountNumber;
        public float AccountBalance => bankAccount.AccountBalance;

        public delegate void CreateTransferForBankAccount(IBankAccount bankAccount, Type? createTransferView);

        public ICommand TransferCommand => transferCommand;

        public BankAccount(ModelLayer.IBankAccount bankAccount, CreateTransferForBankAccount createTransferForBankAccountCallback) 
        { 
            this.bankAccount = bankAccount;
            this.createTransferForBankAccountCallback = createTransferForBankAccountCallback;
            transferCommand = new RelayCommand(ExecuteTransferCommand);
        }

        private void ExecuteTransferCommand(object? parameter)
        {
            createTransferForBankAccountCallback?.Invoke(this, parameter as Type);
        }
    }
}
