using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer
{
    public abstract class AModelLayer : INotifyPropertyChanged
    {
        private static AModelLayer? instance;

        public static AModelLayer Instance { get
            {
                instance ??= new Implementations.BasicModelLayer();
                return instance;
            } 
        }



        #region EVENTS

        public abstract event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        public abstract IUserContext UserContext { get; }

        public abstract object CurrentView { get; }
        public abstract bool IsEuroShowed { get; set; }
        public abstract bool IsUsdShowed { get; set; }
        public abstract bool IsGbpShowed { get; set; }
        public abstract bool IsChfShowed { get; set; }
        public abstract float EuroPurchase { get; }
        public abstract float EuroSell { get; }
        public abstract float UsdPurchase { get; }
        public abstract float UsdSell { get; }
        public abstract float GbpPurchase { get; }
        public abstract float GbpSell { get; }
        public abstract float ChfPurchase { get; }
        public abstract float ChfSell { get; }

        public abstract void Redirect(Type? viewType);

        public abstract bool Login(string login, string password, Type? succesViewRedirection);
        public abstract void Logout(Type? loginView);
        public abstract void Register(string name, string surname, string email, string password, string repeatPassword, Type? succesViewRedirection, Popup registerFailurePopup);
        public abstract void MakeTransfer(string sourceAccountNumber, string destinationAccountNumber, float transferAmount, string transferTitle, Type? userBankAccountsView);
        public abstract void CreateTransferForBankAccount(string accountNumber, Type? createTransferView);
        public abstract void OpenNewBankAccount();

        internal abstract ALogicLayer LogicLayer { get; }
    }
}
