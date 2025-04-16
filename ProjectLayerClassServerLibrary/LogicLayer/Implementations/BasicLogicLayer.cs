using ProjectLayerClassServerLibrary.DataLayer;
using ProjectLayerClassServerLibrary.LogicLayer.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

[assembly: InternalsVisibleTo("ProjectLayerClassServerLibraryTest")]

namespace ProjectLayerClassServerLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayer : ALogicLayer
    {
        private ADataLayer dataLayer;
        private object accountOwnersLock = new object();
        private object bankAccountsLock = new object();
        private Timer bankAccountReportTimer;
        private bool reportsHasBeenUpdatedRecently = false;

        private readonly BasicReportsUpdateLogicLayerTracker reportsUpdateLogicLayerTracker = new BasicReportsUpdateLogicLayerTracker();

        public override AReportsUpdateLogicLayerTracker ReportsUpdateLogicLayerTracker => reportsUpdateLogicLayerTracker;


        public ADataLayer DataLayer { get { return dataLayer; } }

        private ACurrenciesRates currenciesRates;
        private Timer currenciesRatesChangeTimer;
        private ACurrenciesRatesChangeTracker currenciesRatesChangeTracker;
        public override ACurrenciesRatesChangeTracker CurrenciesRatesChangeTracker => currenciesRatesChangeTracker;

        public BasicLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer ?? ADataLayer.CreateDataLayerInstance();

            currenciesRates = new BasicCurrenciesRates(4.26f, 4.29f, 3.74f, 3.77f, 5.01f, 5.05f, 4.61f, 4.64f);
            currenciesRatesChangeTracker = new BasicCurrenciesRatesChangeTracker();
            currenciesRatesChangeTimer = new Timer(new TimeSpan(0, 0, 30));
            currenciesRatesChangeTimer.Elapsed += (Object? source, ElapsedEventArgs e) =>
            {
                Random random = new Random();
                float change = (float)((random.NextDouble() * 0.2) - 0.1);
                currenciesRates.EuroPurchaseRate += change;
                currenciesRates.EuroSellRate += change;
                change = (float)((random.NextDouble() * 0.2) - 0.1);
                currenciesRates.UsdPurchaseRate += change;
                currenciesRates.UsdSellRate += change;
                change = (float)((random.NextDouble() * 0.2) - 0.1);
                currenciesRates.GbpPurchaseRate += change;
                currenciesRates.GbpSellRate += change;
                change = (float)((random.NextDouble() * 0.2) - 0.1);
                currenciesRates.ChfPurchaseRate += change;
                currenciesRates.ChfSellRate += change;
                currenciesRatesChangeTracker.TrackCurrenciesRatesChanged(currenciesRates);
            };
            currenciesRatesChangeTimer.Enabled = true;
            currenciesRatesChangeTimer.AutoReset = true;
            currenciesRatesChangeTimer.Start();


            bankAccountReportTimer = new Timer(new TimeSpan(0, 1, 0));
            bankAccountReportTimer.Elapsed += (Object? source, ElapsedEventArgs e) =>
            {
                //if (DateTime.UtcNow.Day == 2)
                {
                    reportsHasBeenUpdatedRecently = true;
                    foreach (DataLayer.ABankAccount bankAccount in this.dataLayer.GetAllBankAccounts())
                    {
                        bankAccount.GenerateBankAccountReport();
                    }
                    reportsUpdateLogicLayerTracker.TrackWhetherReportsUpdatesChanged(true);
                }
            };
            bankAccountReportTimer.Enabled = true;
            bankAccountReportTimer.AutoReset = true;
            bankAccountReportTimer.Start();
        }

        public void Dispose()
        {
            bankAccountReportTimer.Stop();
            bankAccountReportTimer.Dispose();
            currenciesRatesChangeTimer.Stop();
            currenciesRatesChangeTimer.Dispose();
        }

        public override bool AuthenticateAccountOwner(string login, string password)
        {
            lock (accountOwnersLock)
            {
                DataLayer.AAccountOwner? accountOwner = dataLayer.GetAccountOwner(login);

                if (login == null)
                {
                    throw new ArgumentNullException("Podany obiekt string loginu jest null");
                }

                if (password == null)
                {
                    throw new ArgumentNullException("Podany obiekt string hasła jest null");
                }

                if (accountOwner == null)
                {
                    throw new ThereIsNoSuchOwnerException($"Nie znaleziono właściciela konta z podanym loginem: {login}");
                }

                return password == accountOwner.OwnerPassword;   
            }
        }

        public override AAccountOwner? CreateNewAccountOwner(string name, string surname, string email, string password, out CreationAccountOwnerFlags creationAccountOwnerFlags)
        {
            creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;

            string pattern = @"^[\p{Lu}][\p{L}'\- ]*[\p{L}]?$";
            if (!Regex.IsMatch(name, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_NAME;
            }
            pattern = @"^[\p{Lu}][\p{L}'\- ]*[\p{L}]?$";
            if (!Regex.IsMatch(surname, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_SURNAME;
            }
            pattern = @"^[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}$";
            if (!Regex.IsMatch(email, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_EMAIL;
            }
            if (password.Length < 8)
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_PASSWORD;
            }

            if (creationAccountOwnerFlags == CreationAccountOwnerFlags.EMPTY)
            {
                creationAccountOwnerFlags = CreationAccountOwnerFlags.SUCCESS;
                lock (accountOwnersLock)
                {
                    return AAccountOwner.CreateAccountOwner(dataLayer.CreateAccountOwner(name, surname, email, password));
                }
            }

            return null;
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            lock (accountOwnersLock)
            {
                return AAccountOwner.CreateAccountOwner(dataLayer.GetAccountOwner(ownerId));
            }
        }

        public override AAccountOwner? GetAccountOwner(string login)
        {
            lock (accountOwnersLock)
            {
                return AAccountOwner.CreateAccountOwner(dataLayer.GetAccountOwner(login));
            }
        }

        public override ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId)
        {
            ICollection<ABankAccount> bankAccounts = new List<ABankAccount>();
            lock (bankAccountsLock)
            {
                lock (accountOwnersLock)
                {
                    foreach (DataLayer.ABankAccount bankAccount in dataLayer.GetBankAccounts(ownerId))
                    {
                        bankAccounts.Add(ABankAccount.CreateBankAccount(bankAccount));
                    }
                }
            }
            return bankAccounts;
        }

        public override ABankAccount? OpenNewBankAccount(int ownerId)
        {
            lock (bankAccountsLock)
            {
                return ABankAccount.CreateBankAccount(dataLayer.CreateBankAccount(ownerId));
            }
        }

        public override TransferCodes PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description)
        {
            DataLayer.ABankAccount? ownerBankAccount = dataLayer.GetBankAccount(ownerAccountNumber);
            if (ownerBankAccount == null)
            {
                return TransferCodes.OWNER_ACCOUNT_DOESNT_EXISTS;
            }
            if (ownerBankAccount.AccountBalance < amount)
            {
                return TransferCodes.INSUFICIENT_BANK_ACCOUNT_FUNDS;
            }
            DataLayer.ABankAccount? targetBankAccount = dataLayer.GetBankAccount(targetAccountNumber);
            if (targetBankAccount == null)
            {
                return TransferCodes.TARGET_BANK_ACCOUNT_DOESNT_EXISTS;
            }

            object first = ownerBankAccount;
            object second = targetBankAccount;
            if (first.GetHashCode() > second.GetHashCode())
            {
                object temp = first;
                first = second;
                second = temp;
            }

            lock (first)
            {
                lock (second) 
                {
                    ownerBankAccount.DecreaseAccountBalance(amount);
                    targetBankAccount.IncreaseAccountBalance(amount);
                }
            }

            return TransferCodes.SUCCESS;
        }

        public override bool CheckForReportsUpdates(int ownerId)
        {
            bool temp = reportsHasBeenUpdatedRecently;
            reportsHasBeenUpdatedRecently = false;
            return temp;
        }

        public override ICollection<AAccountOwner> GetAllAccountsOwners()
        {
            return dataLayer.GetAllAccountOwners().Select(accountOwner => (AAccountOwner) new BasicLogicLayerAccountOwner(accountOwner)).ToList();
        }

        public override ICollection<ABankAccount> GetAllBankAccounts()
        {
            return dataLayer.GetAllBankAccounts().Select(bankAccount => (ABankAccount) new BasicLogicLayerBankAccount(bankAccount)).ToList();
        }

        public override ABankAccount? GetBankAccountByAccountNumber(string accountNumber)
        {
            ProjectLayerClassServerLibrary.DataLayer.ABankAccount? bankAccount = dataLayer.GetBankAccount(accountNumber);
            if (bankAccount == null)
            {
                return null;
            }
            return new BasicLogicLayerBankAccount(bankAccount);
        }
    }
}
