using ProjectLayerClassLibrary.PresentationLayer.ModelLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public bool IsEuroFiltered { get => viewModelLayer.ModelLayer.IsEuroShowed; 
            set
            { 
                viewModelLayer.ModelLayer.IsEuroShowed = value; 
                OnPropertyChanged(nameof(IsEuroFiltered));
                OnPropertyChanged(nameof(IsEuroShowed));
            }
        }
        public bool IsUsdFiltered { get => viewModelLayer.ModelLayer.IsUsdShowed;
            set
            {
                viewModelLayer.ModelLayer.IsUsdShowed = value;
                OnPropertyChanged(nameof(IsUsdFiltered));
                OnPropertyChanged(nameof(IsUsdShowed));
            }
        }
        public bool IsGbpFiltered { get => viewModelLayer.ModelLayer.IsGbpShowed;
            set
            {
                viewModelLayer.ModelLayer.IsGbpShowed = value;
                OnPropertyChanged(nameof(IsGbpFiltered));
                OnPropertyChanged(nameof(IsGbpShowed));
            }
        }
        public bool IsChfFiltered { get => viewModelLayer.ModelLayer.IsChfShowed;
            set
            {
                viewModelLayer.ModelLayer.IsChfShowed = value;
                OnPropertyChanged(nameof(IsChfFiltered));
                OnPropertyChanged(nameof(IsChfShowed));
            }
        }

        public string EuroPurchase => viewModelLayer.ModelLayer.EuroPurchase > 0.001f ? $"{viewModelLayer.ModelLayer.EuroPurchase} PLN" : "-- PLN";

        public string EuroSell => viewModelLayer.ModelLayer.EuroSell != 0.0f ? $"{viewModelLayer.ModelLayer.EuroSell} PLN" : "-- PLN";
               
        public string UsdPurchase => viewModelLayer.ModelLayer.UsdPurchase != 0.0f ? $"{viewModelLayer.ModelLayer.UsdPurchase} PLN" : "-- PLN";
               
        public string UsdSell => viewModelLayer.ModelLayer.UsdSell != 0.0f ? $"{viewModelLayer.ModelLayer.UsdSell} PLN" : "-- PLN";
               
        public string GbpPurchase => viewModelLayer.ModelLayer.GbpPurchase != 0.0f ? $"{viewModelLayer.ModelLayer.GbpPurchase} PLN" : "-- PLN";
               
        public string GbpSell => viewModelLayer.ModelLayer.GbpSell != 0.0f ? $"{viewModelLayer.ModelLayer.GbpSell} PLN" : "-- PLN";
               
        public string ChfPurchase => viewModelLayer.ModelLayer.ChfPurchase != 0.0f ? $"{viewModelLayer.ModelLayer.ChfPurchase} PLN" : "-- PLN";
               
        public string ChfSell => viewModelLayer.ModelLayer.ChfSell != 0.0f ? $"{viewModelLayer.ModelLayer.ChfSell} PLN" : "-- PLN";

        public Visibility IsEuroShowed => IsEuroFiltered ? Visibility.Visible : Visibility.Hidden;

        public Visibility IsUsdShowed => IsUsdFiltered ? Visibility.Visible : Visibility.Hidden;

        public Visibility IsGbpShowed => IsGbpFiltered ? Visibility.Visible : Visibility.Hidden;

        public Visibility IsChfShowed => IsChfFiltered ? Visibility.Visible : Visibility.Hidden;

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
