﻿using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class ADataLayer
    {
        [Flags]
        public enum CreationAccountOwnerDataLayerFlags
        {
            EMPTY = 1,
            SUCCESS = 2,
            INCORRECT_NAME = 4,
            INCORRECT_SURNAME = 8,
            INCORRECT_EMAIL = 16,
            INCORRECT_PASSWORD = 32
        }

        public enum TransferResultCodes
        {
            SUCCESS,
            OWNER_ACCOUNT_DOESNT_EXISTS,
            TARGET_BANK_ACCOUNT_DOESNT_EXISTS,
            INSUFICIENT_BANK_ACCOUNT_FUNDS,
            TIMEOUT,
            TRANSFER_HAS_BEEN_INTERUPTED
        }

        [Flags]
        public enum CurrenciesOfInterest
        {
            EMPTY = 0,
            EURO = 1,
            USD = 2,
            CHF = 4,
            GBP = 8,
        }

        public abstract CurrenciesOfInterest CurrenciesOfInterestFilter { get; set; }

        public delegate void TransferDataLayerCallback(ADataLayer.TransferResultCodes transferResult, string ownerAccountNumber, string targetAccountNumber, float amount, string description);

        #region EVENTS

        public abstract AReportsUpdateDataLayerTracker ReportsUpdateTracker { get; }
        public abstract event Action BankAccountsUpdate;

        public delegate void CurrencyRatesUpdateAction(DataLayer.ACurrencyRateOfPurchaseAndSell currencyRateOfPurchaseAndSell);
        public abstract event CurrencyRatesUpdateAction EuroRatesUpdateEvent;
        public abstract event CurrencyRatesUpdateAction UsdRatesUpdateEvent;
        public abstract event CurrencyRatesUpdateAction GbpRatesUpdateEvent;
        public abstract event CurrencyRatesUpdateAction ChfRatesUpdateEvent;


        #endregion

        public static ADataLayer CreateDataLayerInstance()
        {
            return new Implementations.ServerComunicatingDataLayer();
        }

        public abstract AAccountOwner? CreateAccountOwner(string ownerName, string ownerSurname, string ownerEmail, string ownerPassword, out CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags);

        public abstract ABankAccount? CreateBankAccount(int ownerId);

        public abstract bool AuthenticateAccountOwner(string login, string password);


        public abstract AAccountOwner? GetAccountOwner(int ownerId);
        public abstract AAccountOwner? GetAccountOwner(string ownerLogin);
        public abstract ABankAccount? GetBankAccount(string accountNumber);
        public abstract ICollection<ABankAccount> GetBankAccounts(int ownerId);
        public abstract ICollection<AAccountOwner> GetAllAccountOwners();
        public abstract ICollection<ABankAccount> GetAllBankAccounts();

        public abstract void PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description, TransferDataLayerCallback transferCallback);
        public abstract bool CheckForReportsUpdates(int ownerId);
    }
}
