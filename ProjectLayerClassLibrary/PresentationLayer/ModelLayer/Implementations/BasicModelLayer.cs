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

        public override object CurrentView => currentView;
        public override IUserContext UserContext => userContext;

        internal override ALogicLayer LogicLayer => logicLayer;

        public BasicModelLayer()
        {
            logicLayer = ALogicLayer.CreateLogicLayerInstance();

            modelLayerReporter = new BasicReportsUpdateModelLayerReporter(() => { }, (value) => OnPropertyChanged("ReportMessages"));
            modelLayerReporter.Subscribe(logicLayer.ReportsUpdateTracker);
            logicLayer.BankAccountsUpdate += UpdateBankAccounts;
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
            AAccountOwner? owner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out ALogicLayer.CreationAccountOwnerFlags creationAccountOwnerFlags);
            if (creationAccountOwnerFlags == ALogicLayer.CreationAccountOwnerFlags.SUCCESS)
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
    }
}
