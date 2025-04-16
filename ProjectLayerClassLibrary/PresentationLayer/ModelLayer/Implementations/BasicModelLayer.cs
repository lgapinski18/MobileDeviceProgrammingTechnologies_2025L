using ProjectLayerClassLibrary.LogicLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls.Primitives;
using static ProjectLayerClassLibrary.LogicLayer.ALogicLayer;
using Timer = System.Timers.Timer;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer.Implementations
{
    internal class BasicModelLayer : AModelLayer
    {
        private ALogicLayer logicLayer;
        private IUserContext? userContext = null;
        private object currentView;
        private AReportsUpdateModelLayerReporter modelLayerReporter;

        // Currencies
        private ALogicLayer.CurrenciesOfInterest currenciesOfInterest = 0;
        private float euroPurchase = 0.0f;
        private float euroSell = 0.0f;
        private float usdPurchase = 0.0f;
        private float usdSell = 0.0f;
        private float gbpPurchase = 0.0f;
        private float gbpSell = 0.0f;
        private float chfPurchase = 0.0f;
        private float chfSell = 0.0f;


        public override object CurrentView => currentView;
        public override IUserContext UserContext => userContext;

        internal override ALogicLayer LogicLayer => logicLayer;

        public override bool IsEuroShowed { get => (currenciesOfInterest & CurrenciesOfInterest.EURO) == CurrenciesOfInterest.EURO;
            set
            {
                if (value)
                {
                    currenciesOfInterest &= CurrenciesOfInterest.EURO;
                }
                else
                {
                    currenciesOfInterest &= ~CurrenciesOfInterest.EURO;
                }
                logicLayer.CurrenciesOfInterestFilter = currenciesOfInterest;
            }
        }
        public override bool IsUsdShowed { get => (currenciesOfInterest & CurrenciesOfInterest.USD) == CurrenciesOfInterest.USD;
            set
            {
                if (value)
                {
                    currenciesOfInterest &= CurrenciesOfInterest.USD;
                }
                else
                {
                    currenciesOfInterest &= ~CurrenciesOfInterest.USD;
                }
                logicLayer.CurrenciesOfInterestFilter = currenciesOfInterest;
            }
        }
        public override bool IsGbpShowed { get => (currenciesOfInterest & CurrenciesOfInterest.GBP) == CurrenciesOfInterest.GBP;
            set
            {
                if (value)
                {
                    currenciesOfInterest &= CurrenciesOfInterest.GBP;
                }
                else
                {
                    currenciesOfInterest &= ~CurrenciesOfInterest.GBP;
                }
                logicLayer.CurrenciesOfInterestFilter = currenciesOfInterest;
            }
        }
        public override bool IsChfShowed { get => (currenciesOfInterest & CurrenciesOfInterest.CHF) == CurrenciesOfInterest.CHF;
            set
            {
                if (value)
                {
                    currenciesOfInterest &= CurrenciesOfInterest.CHF;
                }
                else
                {
                    currenciesOfInterest &= ~CurrenciesOfInterest.CHF;
                }
                logicLayer.CurrenciesOfInterestFilter = currenciesOfInterest;
            }
        }

        public override float EuroPurchase => euroPurchase;

        public override float EuroSell => euroSell;

        public override float UsdPurchase => usdPurchase;

        public override float UsdSell => usdSell;

        public override float GbpPurchase => gbpPurchase;

        public override float GbpSell => gbpSell;

        public override float ChfPurchase => chfPurchase;

        public override float ChfSell => chfSell;

        public BasicModelLayer()
        {
            logicLayer = ALogicLayer.CreateLogicLayerInstance();

            modelLayerReporter = new BasicReportsUpdateModelLayerReporter(() => { },
                (value) => { 
                    UserContext.BankAccountsReports = value.Select(report => report.GetReportContent()).ToList();
                    OnPropertyChanged("ReportMessages"); 
                });
            modelLayerReporter.Subscribe(logicLayer.ReportsUpdateTracker);
            logicLayer.BankAccountsUpdate += UpdateBankAccounts;

            logicLayer.EuroRatesUpdateEvent += OnEuroUpdate;
            logicLayer.UsdRatesUpdateEvent += OnUsdUpdate;
            logicLayer.GbpRatesUpdateEvent += OnGbpUpdate;
            logicLayer.ChfRatesUpdateEvent += OnChfUpdate;
        }

        #region EVENTS

        public override event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        public override bool Login(string login, string password, Type? succesViewRedirection)
        {
            try
            {
                if (logicLayer.AuthenticateAccountOwner(login, password))
                {
                    AAccountOwner owner = logicLayer.GetAccountOwner(login);
                    userContext = new UserContext(owner, logicLayer.GetAccountOwnerBankAccounts(owner.GetId()));
                    Redirect(succesViewRedirection);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public override void Logout(Type? loginView)
        {
            userContext = null;
            Redirect(loginView);
        }

        public override void Register(string name, string surname, string email, string password, string repeatPassword, Type? succesViewRedirection, Popup registerFailurePopup)
        {
            AAccountOwner? owner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out CreationAccountOwnerFlags creationAccountOwnerFlags);
            if (creationAccountOwnerFlags == CreationAccountOwnerFlags.SUCCESS)
            {
                userContext = new UserContext(owner, logicLayer.GetAccountOwnerBankAccounts(owner.GetId()));
                Redirect(succesViewRedirection);
            }
            else
            {
                if ((creationAccountOwnerFlags & CreationAccountOwnerFlags.EMPTY) != 0)
                {
                    registerFailurePopup.DataContext = new PopupMessage("Empty field");
                }
                else if((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_NAME) != 0)
                {
                    registerFailurePopup.DataContext = new PopupMessage("Incorrect Name");
                }
                else if ((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_SURNAME) != 0)
                {
                    registerFailurePopup.DataContext = new PopupMessage("Incorrect Surname");
                }
                else if ((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_EMAIL) != 0)
                {
                    registerFailurePopup.DataContext = new PopupMessage("Incorrect Email");
                }
                else if ((creationAccountOwnerFlags & CreationAccountOwnerFlags.INCORRECT_PASSWORD) != 0)
                {
                    registerFailurePopup.DataContext = new PopupMessage("Incorrect Password");
                }
                registerFailurePopup.IsOpen = true;
            }
        }

        public override void Redirect(Type? viewType)
        {
            if (viewType != null)
            {
                object? newView = Activator.CreateInstance(viewType);
                if (newView != null)
                {
                    currentView = newView;
                    OnPropertyChanged(nameof(CurrentView));
                }
            }
        }

        public override void MakeTransfer(string sourceAccountNumber, string destinationAccountNumber, float transferAmount, string transferTitle, Type? userBankAccountsView)
        {
            logicLayer.PerformTransfer(sourceAccountNumber, destinationAccountNumber, transferAmount, transferTitle, TransferCallback);
            userContext = new UserContext(logicLayer.GetAccountOwner(UserContext.Id), logicLayer.GetAccountOwnerBankAccounts(UserContext.Id));
            Redirect(userBankAccountsView);
        }

        private void TransferCallback(TransferCodes transferResult, string ownerAccountNumber, string targetAccountNumber, float amount, string description)
        {

        }

        public override void CreateTransferForBankAccount(string bankAccountNumber, Type? createTransferView)
        {
            UserContext.BankAccountNumberForTransfer = bankAccountNumber;
            Redirect(createTransferView);
        }

        public override void OpenNewBankAccount()
        {
            logicLayer.OpenNewBankAccount(UserContext.Id);
            userContext = new UserContext(logicLayer.GetAccountOwner(UserContext.Id), logicLayer.GetAccountOwnerBankAccounts(UserContext.Id));
            OnPropertyChanged("BankAccounts");
        }

        private void UpdateBankAccounts()
        {
            AAccountOwner accountOwner = logicLayer.GetAccountOwner(UserContext.Id);
            ICollection<ABankAccount> bankAccounts = logicLayer.GetAccountOwnerBankAccounts(UserContext.Id);
            userContext = new UserContext(accountOwner, bankAccounts);
            OnPropertyChanged("BankAccounts");
        }

        private void OnEuroUpdate(LogicLayer.ACurrencyRateOfPurchaseAndSell currency)
        {
            euroPurchase = currency.CurrencyRateOfPurchase;
            euroSell = currency.CurrencyRateOfSell;
            OnPropertyChanged(nameof(EuroPurchase));
            OnPropertyChanged(nameof(EuroSell));
        }

        private void OnUsdUpdate(LogicLayer.ACurrencyRateOfPurchaseAndSell currency)
        {
            usdPurchase = currency.CurrencyRateOfPurchase;
            usdSell = currency.CurrencyRateOfSell;
            OnPropertyChanged(nameof(UsdPurchase));
            OnPropertyChanged(nameof(UsdSell));
        }

        private void OnGbpUpdate(LogicLayer.ACurrencyRateOfPurchaseAndSell currency)
        {
            gbpPurchase = currency.CurrencyRateOfPurchase;
            gbpSell = currency.CurrencyRateOfSell;
            OnPropertyChanged(nameof(GbpPurchase));
            OnPropertyChanged(nameof(GbpSell));
        }

        private void OnChfUpdate(LogicLayer.ACurrencyRateOfPurchaseAndSell currency)
        {
            chfPurchase = currency.CurrencyRateOfPurchase;
            chfSell = currency.CurrencyRateOfSell;
            OnPropertyChanged(nameof(ChfPurchase));
            OnPropertyChanged(nameof(ChfSell));
        }
    }
}
