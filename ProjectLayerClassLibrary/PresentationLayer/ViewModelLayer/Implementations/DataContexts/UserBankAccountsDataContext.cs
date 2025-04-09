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

        public string Login => viewModelLayer.ModelLayer.UserContext.Login;
        public string UserName => viewModelLayer.ModelLayer.UserContext.UserName;
        public string UserSurname => viewModelLayer.ModelLayer.UserContext.UserSurname;
        public ICollection<IBankAccount> BankAccounts =>
            viewModelLayer.ModelLayer.UserContext.BankAccounts.Select(bankAccount => new BankAccount(bankAccount, CreateTransferForBankAccount)).ToArray<IBankAccount>();

        public ICollection<string> ReportMessages => viewModelLayer.ModelLayer.UserContext.BankAccountsReports.ToArray<string>();

        #endregion

        #region COMMANDS

        private ICommand logoutCommand;
        private ICommand openNewBankAccountCommand;


        public ICommand LogoutCommand => logoutCommand;
        public ICommand OpenNewBankAccountCommand => openNewBankAccountCommand;

        #endregion

        public UserBankAccountsDataContext(AViewModelLayer viewModelLayer) : base(viewModelLayer)
        {
            //userContext = viewModelLayer.ModelLayer.UserContext;
            //bankAccounts = userContext.BankAccounts.Select(bankAccount => new BankAccount(bankAccount, CreateTransferForBankAccount)).ToArray<IBankAccount>();
            viewModelLayer.PropertyChanged += ViewModelLayer_PropertyChanged;
            logoutCommand = new RelayCommand(ExecuteLogoutCommand);
            openNewBankAccountCommand = new RelayCommand(ExecuteOpenNewBankAccountCommand);
        }

        private void ViewModelLayer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BankAccounts))
            {
                OnPropertyChanged(nameof(BankAccounts));
            }
            else if (e.PropertyName == nameof(ReportMessages))
            {
                OnPropertyChanged(nameof(ReportMessages));
            }
        }

        private void ExecuteLogoutCommand(object? parameter)
        {
            viewModelLayer.ModelLayer.Logout(parameter as Type);
        }
        private void ExecuteOpenNewBankAccountCommand(object? parameter)
        {
            viewModelLayer.ModelLayer.OpenNewBankAccount();
        }


        private void CreateTransferForBankAccount(IBankAccount bankAccount, Type? createTransferView)
        {
            viewModelLayer.ModelLayer.CreateTransferForBankAccount(bankAccount.AccountNumber, createTransferView);
        }
    }
}
