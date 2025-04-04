﻿using ProjectLayerClassLibrary.PresentationLayer.ModelLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    public abstract class AViewModelLayer : INotifyPropertyChanged
    {
        private static AViewModelLayer? instance;
        public static AViewModelLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Implementations.BasicViewModelLayer();
                }
                return instance;
            }
        }

        public abstract object CurrentView { get; }

        internal abstract AModelLayer ModelLayer { get; }


        public abstract event PropertyChangedEventHandler? PropertyChanged;

        public abstract void Redirect(Type? viewType);

        public abstract ILoginDataContext CreateLoginDataContext(Popup loginFailurePopUp);
        public abstract IRegisterDataContext CreateRegisterDataContext(Popup registerFailurePopup);
        public abstract IUserBankAccountsDataContext CreateUserBankAccountDataContext();
        public abstract ICreateTransferDataContext CreateCreateTransferDataContext();

    }
}
