using ProjectLayerClassLibrary.PresentationLayer.ModelLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Register;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
                    instance = new BasicViewModelLayer();
                }
                return instance;
            }
        }

        public abstract object CurrentView { get; protected set; }

        internal abstract AModelLayer? ModelLayer { get; }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Redirect(Type? viewType);

        public abstract ILoginDataContext GetLoginDataContext();
        public abstract IRegisterDataContext GetRegisterDataContext();

    }
}
