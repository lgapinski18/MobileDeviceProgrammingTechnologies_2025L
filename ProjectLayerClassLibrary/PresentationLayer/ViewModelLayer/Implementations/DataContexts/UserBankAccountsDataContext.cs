using ProjectLayerClassLibrary.PresentationLayer.ModelLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Implementations.DataContexts
{
    internal class UserBankAccountsDataContext : ADataContext, IUserBankAccountsDataContext
    {
        #region DATA

        private IUserContext userContext;
        private ICollection<IBankAccount> bankAccounts;

        public string Login => userContext.Login;
        public string UserName => userContext.UserName;
        public string UserSurname => userContext.UserSurname;
        public ICollection<IBankAccount> BankAccounts => bankAccounts;

        #endregion

        #region COMMANDS
        private ICommand transferCommand;

        public ICommand TransferCommand => transferCommand;

        #endregion

        public UserBankAccountsDataContext(AViewModelLayer viewModelLayer) : base(viewModelLayer)
        {
            userContext = viewModelLayer.ModelLayer.UserContext;
            bankAccounts = userContext.BankAccounts.Select(bankAccount => new BankAccount(bankAccount)).ToArray<IBankAccount>();

            transferCommand = new RelayCommand(ExecuteTransferCommand);
        }

        private void ExecuteTransferCommand(object? parameter)
        {

        }
    }
}
