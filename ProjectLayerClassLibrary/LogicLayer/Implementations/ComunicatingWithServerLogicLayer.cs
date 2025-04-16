using ProjectLayerClassLibrary.DataLayer;
using System.Runtime.CompilerServices;
using static ProjectLayerClassLibrary.DataLayer.ADataLayer;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class ComunicatingWithServerLogicLayer : ALogicLayer
    {
        private ADataLayer dataLayer;
        private AReportsUpdateLogicLayerReporter reportsUpdateReporter;


        #region EVENTS

        protected AReportsUpdateLogicLayerTracker reportsUpdateTracker;
        public override AReportsUpdateLogicLayerTracker ReportsUpdateTracker { get { return reportsUpdateTracker; } }
        public override CurrenciesOfInterest CurrenciesOfInterestFilter { get => (ALogicLayer.CurrenciesOfInterest)dataLayer.CurrenciesOfInterestFilter; set => dataLayer.CurrenciesOfInterestFilter = (ADataLayer.CurrenciesOfInterest)value; }

        public override event Action BankAccountsUpdate;
        public override event CurrencyRatesUpdateAction EuroRatesUpdateEvent;
        public override event CurrencyRatesUpdateAction UsdRatesUpdateEvent;
        public override event CurrencyRatesUpdateAction GbpRatesUpdateEvent;
        public override event CurrencyRatesUpdateAction ChfRatesUpdateEvent;

        protected void CallBankAccountsUpdate()
        {
            BankAccountsUpdate?.Invoke();
        }

        #endregion

        public ComunicatingWithServerLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer ?? ADataLayer.CreateDataLayerInstance();
            reportsUpdateTracker = new BasicReportsUpdateLogicLayerTracker();
            BasicReportsUpdateLogicLayerReporter reportsUpdateReporter = new BasicReportsUpdateLogicLayerReporter(reportsUpdateTracker);
            reportsUpdateReporter.Subscribe(this.dataLayer.ReportsUpdateTracker);
            this.reportsUpdateReporter = reportsUpdateReporter;

            this.dataLayer.BankAccountsUpdate += CallBankAccountsUpdate;
            this.dataLayer.EuroRatesUpdateEvent += (rates) => { 
                EuroRatesUpdateEvent?.Invoke(new LogicLayer.Implementations.SimpleCurrencyRateOfPurchaseAndSell(rates.CurrencyRateOfPurchase, rates.CurrencyRateOfSell)); };
            this.dataLayer.UsdRatesUpdateEvent += (rates) => {
                UsdRatesUpdateEvent?.Invoke(new LogicLayer.Implementations.SimpleCurrencyRateOfPurchaseAndSell(rates.CurrencyRateOfPurchase, rates.CurrencyRateOfSell));
            };
            this.dataLayer.GbpRatesUpdateEvent += (rates) => {
                GbpRatesUpdateEvent?.Invoke(new LogicLayer.Implementations.SimpleCurrencyRateOfPurchaseAndSell(rates.CurrencyRateOfPurchase, rates.CurrencyRateOfSell));
            };
            this.dataLayer.ChfRatesUpdateEvent += (rates) => {
                ChfRatesUpdateEvent?.Invoke(new LogicLayer.Implementations.SimpleCurrencyRateOfPurchaseAndSell(rates.CurrencyRateOfPurchase, rates.CurrencyRateOfSell));
            };
        }

        public override bool AuthenticateAccountOwner(string login, string password)
        {
            return dataLayer.AuthenticateAccountOwner(login, password);
        }

        public override bool CheckForReportsUpdates(int ownerId)
        {
            return dataLayer.CheckForReportsUpdates(ownerId);
        }

        public override AAccountOwner? CreateNewAccountOwner(string name, string surname, string email, string password, out CreationAccountOwnerFlags creationAccountOwnerFlags)
        {
            CreationAccountOwnerDataLayerFlags creationAccountOwneDataLayerrFlags = CreationAccountOwnerDataLayerFlags.EMPTY;
            DataLayer.AAccountOwner? accountOwner = dataLayer.CreateAccountOwner(name, surname, email, password, out creationAccountOwneDataLayerrFlags);
            creationAccountOwnerFlags = (CreationAccountOwnerFlags)creationAccountOwneDataLayerrFlags;

            return LogicLayer.AAccountOwner.CreateAccountOwner(accountOwner);
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            return LogicLayer.AAccountOwner.CreateAccountOwner(dataLayer.GetAccountOwner(ownerId));
        }

        public override AAccountOwner? GetAccountOwner(string login)
        {
            return LogicLayer.AAccountOwner.CreateAccountOwner(dataLayer.GetAccountOwner(login));
        }

        public override ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId)
        {
            return dataLayer.GetBankAccounts(ownerId).Select(bankAccount => ABankAccount.CreateBankAccount(bankAccount)).ToArray();
        }

        public override ABankAccount? OpenNewBankAccount(int ownerId)
        {
            return LogicLayer.ABankAccount.CreateBankAccount(dataLayer.CreateBankAccount(ownerId));
        }

        public override void PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description, TransferCallback transferCallback)
        {
            dataLayer.PerformTransfer(ownerAccountNumber, targetAccountNumber, amount, description, (transferDataLayerCodes, oAN, tAN, a, d) =>
            {
                transferCallback((TransferCodes)transferDataLayerCodes, oAN, tAN, a, d);
            });
        }
    }
}
