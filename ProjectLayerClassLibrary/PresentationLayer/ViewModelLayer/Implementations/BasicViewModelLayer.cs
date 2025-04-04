﻿using ProjectLayerClassLibrary.PresentationLayer.ModelLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Implementations.DataContexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Implementations
{
    internal class BasicViewModelLayer : AViewModelLayer
    {
        private AModelLayer modelLayer;

        public override object CurrentView { get => modelLayer.CurrentView; }

        internal override AModelLayer ModelLayer => modelLayer;

        #region EVENTS

        public override event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public BasicViewModelLayer()
        {
            //this.modelLayer = AModelLayer.createModelLayerInstance();
            modelLayer = AModelLayer.Instance;
            modelLayer.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, args);
        }

        public override void Redirect(Type? viewType)
        {
            modelLayer.Redirect(viewType);
        }

        public override ILoginDataContext CreateLoginDataContext(Popup loginFailurePopUp)
        {
            return new LoginDataContext(this, loginFailurePopUp);
        }

        public override IRegisterDataContext CreateRegisterDataContext(Popup registerFailurePopup)
        {
            return new RegisterDataContext(this, registerFailurePopup);
        }

        public override IUserBankAccountsDataContext CreateUserBankAccountDataContext()
        {
            return new UserBankAccountsDataContext(this);
        }

        public override ICreateTransferDataContext CreateCreateTransferDataContext()
        {
            return new CreateTransferDataContext(this);
        }
    }
}
