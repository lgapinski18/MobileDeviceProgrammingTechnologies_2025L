using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.LogicLayer.Exceptions;
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

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayer : ALogicLayer
    {
        private ADataLayer dataLayer;
        private object accountOwnersLock = new object();
        private object bankAccountsLock = new object();
        private Timer bankAccountReportTimer;
        private bool reportsHasBeenUpdatedRecently = false;

        public ADataLayer DataLayer { get { return dataLayer; } }

        public BasicLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer ?? ADataLayer.CreateDataLayerInstance();

            bankAccountReportTimer = new Timer(new TimeSpan(24, 0, 0));
            bankAccountReportTimer.Elapsed += (Object? source, ElapsedEventArgs e) =>
            {
                if (DateTime.UtcNow.Day == 2)
                {
                    reportsHasBeenUpdatedRecently = true;
                    foreach (DataLayer.ABankAccount bankAccount in this.dataLayer.GetAllBankAccounts())
                    {
                        bankAccount.GenerateBankAccountReport();
                    }
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

            string pattern = @"^[\p{L}][\p{L}'\- ]*[\p{L}]?$";
            if (!Regex.IsMatch(name, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_NAME;
            }
            pattern = @"^[\p{L}][\p{L}'\- ]*[\p{L}]?$";
            if (!Regex.IsMatch(surname, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_SURNAME;
            }
            pattern = @"^[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}$";
            if (!Regex.IsMatch(email, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_EMAIL;
            }
            if (password.Length >= 8)
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

        public override Thread PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description, TransferCallback transferCallback)
        {
            Thread thread = new Thread(() =>
            {
                DataLayer.ABankAccount? ownerBankAccount = dataLayer.GetBankAccount(ownerAccountNumber);
                if (ownerBankAccount == null)
                {
                    transferCallback(TransferCodes.OWNER_ACCOUNT_DOESNT_EXISTS, ownerAccountNumber, targetAccountNumber, amount, description);
                    return;
                }
                if (ownerBankAccount.AccountBalance < amount)
                {
                    transferCallback(TransferCodes.INSUFICIENT_BANK_ACCOUNT_FUNDS, ownerAccountNumber, targetAccountNumber, amount, description);
                    return;
                }
                DataLayer.ABankAccount? targetBankAccount = dataLayer.GetBankAccount(targetAccountNumber);
                if (targetBankAccount == null)
                {
                    transferCallback(TransferCodes.TARGET_BANK_ACCOUNT_DOESNT_EXISTS, ownerAccountNumber, targetAccountNumber, amount, description);
                    return;
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

                transferCallback(TransferCodes.SUCCESS, ownerAccountNumber, targetAccountNumber, amount, description);
            }
            );
            thread.Start();
            return thread;
        }

        public override bool CheckForReportsUpdates()
        {
            return reportsHasBeenUpdatedRecently;
        }
    }
}
