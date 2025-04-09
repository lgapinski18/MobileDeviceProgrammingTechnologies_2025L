using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ProjectLayerClassLibrary.DataLayer.ADataLayer;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class ComunicatingWithServerLogicLayer : ALogicLayer
    {
        private ADataLayer dataLayer;
        private AReportsUpdateLogicLayerReporter reportsUpdateReporter;

        public ComunicatingWithServerLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer ?? ADataLayer.CreateDataLayerInstance();
            reportsUpdateTracker = new BasicReportsUpdateLogicLayerTracker();
            BasicReportsUpdateLogicLayerReporter reportsUpdateReporter = new BasicReportsUpdateLogicLayerReporter(reportsUpdateTracker);
            reportsUpdateReporter.Subscribe(dataLayer.ReportsUpdateTracker);
            this.reportsUpdateReporter = reportsUpdateReporter;
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

        public override Thread PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description, TransferCallback transferCallback)
        {

            Thread thread = new Thread(() =>
            {
                dataLayer.PerformTransfer(ownerAccountNumber, targetAccountNumber, amount, description, (transferDataLayerCodes, oAN, tAN, a, d) =>
                {
                    transferCallback((TransferCodes)transferDataLayerCodes, oAN, tAN, a, d);
                });

            }
            );
            thread.Start();
            return thread;
        }
    }
}
