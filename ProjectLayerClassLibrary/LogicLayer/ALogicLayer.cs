﻿using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer
{
    public abstract class ALogicLayer
    {
        public enum TransferCodes
        {
            SUCCESS,
            OWNER_ACCOUNT_DOESNT_EXISTS,
            TARGET_BANK_ACCOUNT_DOESNT_EXISTS,
            INSUFICIENT_BANK_ACCOUNT_FUNDS,
            TIMEOUT,
            TRANSFER_HAS_BEEN_INTERUPTED
        }

        [Flags]
        public enum CreationAccountOwnerFlags
        {
            EMPTY = 1,
            SUCCESS = 2,
            INCORRECT_NAME = 4,
            INCORRECT_SURNAME = 8,
            INCORRECT_EMAIL = 16,
            INCORRECT_PASSWORD = 32,
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

        public delegate void TransferCallback(TransferCodes transferResult, string ownerAccountNumber, string targetAccountNumber, float amount, string description);

        #region EVENTS

        public abstract AReportsUpdateLogicLayerTracker ReportsUpdateTracker { get; }
        public abstract event Action BankAccountsUpdate;

        public delegate void CurrencyRatesUpdateAction(LogicLayer.ACurrencyRateOfPurchaseAndSell currencyRateOfPurchaseAndSell);
        public abstract event CurrencyRatesUpdateAction EuroRatesUpdateEvent;
        public abstract event CurrencyRatesUpdateAction UsdRatesUpdateEvent;
        public abstract event CurrencyRatesUpdateAction GbpRatesUpdateEvent;
        public abstract event CurrencyRatesUpdateAction ChfRatesUpdateEvent;

        #endregion

        public static ALogicLayer CreateLogicLayerInstance(ADataLayer? dataLayer = default)
        {
            return new Implementations.ComunicatingWithServerLogicLayer(dataLayer);
        }

        public abstract AAccountOwner? CreateNewAccountOwner(string name, string surname, string email, string  password, out CreationAccountOwnerFlags creationAccountOwnerFlags);
        public abstract ABankAccount? OpenNewBankAccount(int ownerId);

        public abstract bool AuthenticateAccountOwner(string login, string password);
        public abstract AAccountOwner? GetAccountOwner(int ownerId);
        public abstract AAccountOwner? GetAccountOwner(string login);
        public abstract ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId);

        public abstract void PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description, TransferCallback transferCallback);

        public abstract bool CheckForReportsUpdates(int ownerId);
    }
}
