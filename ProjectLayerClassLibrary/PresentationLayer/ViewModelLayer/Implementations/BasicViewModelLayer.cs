using ProjectLayerClassLibrary.PresentationLayer.ModelLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Register;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    internal class BasicViewModelLayer : AViewModelLayer
    {
        private AModelLayer? modelLayer;
        private object currentView;

        private ILoginDataContext loginDataContext;
        private IRegisterDataContext registerDataContext;

        public override object CurrentView
        {
            get => currentView;
            protected set
            {
                currentView = value;
                OnPropertyChanged();
            }
        }

        internal override AModelLayer? ModelLayer => modelLayer;

        public BasicViewModelLayer()
        {
            //this.modelLayer = AModelLayer.createModelLayerInstance();
            modelLayer = AModelLayer.Instance;
            loginDataContext = new LoginDataContext(this);
            registerDataContext = new RegisterDataContext(this);
        }

        public override void Redirect(Type? viewType)
        {
            if (viewType != null)
            {
                object? newView = Activator.CreateInstance(viewType);
                if (newView != null)
                {
                    CurrentView = newView;
                }
            }
        }

        public override ILoginDataContext GetLoginDataContext()
        {
            return loginDataContext;
        }

        public override IRegisterDataContext GetRegisterDataContext()
        {
            return registerDataContext;
        }
    }
}
