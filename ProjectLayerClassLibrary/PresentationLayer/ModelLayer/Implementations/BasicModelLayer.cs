using ProjectLayerClassLibrary.LogicLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ProjectLayerClassLibrary.LogicLayer.ALogicLayer;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer.Implementations
{
    internal class BasicModelLayer : AModelLayer
    {
        private ALogicLayer logicLayer;
        private IUserContext? userContext = null;

        public BasicModelLayer()
        {
            logicLayer = ALogicLayer.CreateLogicLayerInstance();
        }
        private object currentView;

        public override object CurrentView => currentView;
        public override IUserContext UserContext => userContext;

        internal override ALogicLayer LogicLayer => logicLayer;

        #region EVENTS

        public override event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public override bool Login(string login, string password, Type? succesViewRedirection)
        {
            if (logicLayer.AuthenticateAccountOwner(login, password))
            {
                AAccountOwner owner = logicLayer.GetAccountOwner(login);
                userContext = new UserContext(owner, logicLayer.GetAccountOwnerBankAccounts(owner.GetId()));
                Redirect(succesViewRedirection);
            }
            else
            {

            }
            return true;
        }

        public override void Register(string name, string surname, string email, string password, string repeatPassword, Type? succesViewRedirection)
        {
            AAccountOwner? owner = logicLayer.CreateNewAccountOwner(name, surname, email, password, out ALogicLayer.CreationAccountOwnerFlags creationAccountOwnerFlags);
            if (creationAccountOwnerFlags == ALogicLayer.CreationAccountOwnerFlags.SUCCESS)
            {
                userContext = new UserContext(owner, logicLayer.GetAccountOwnerBankAccounts(owner.GetId()));
                Redirect(succesViewRedirection);
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

        public override void MakeTransfer(string sourceAccountNumber, string destinationAccountNumber, float transferAmount, string transferTitle)
        {
            logicLayer.PerformTransfer(sourceAccountNumber, destinationAccountNumber, transferAmount, transferTitle, TransferCallback);
        }

        private void TransferCallback(TransferCodes transferResult, string ownerAccountNumber, string targetAccountNumber, float amount, string description)
        {

        }
    }
}
