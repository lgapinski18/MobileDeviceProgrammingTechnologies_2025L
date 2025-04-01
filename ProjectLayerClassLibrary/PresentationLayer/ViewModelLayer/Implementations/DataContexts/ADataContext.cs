using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    internal abstract class ADataContext : IDataContext
    {
        protected readonly AViewModelLayer viewModelLayer;

        private ICommand redirect;

        public ICommand Redirect { get => redirect; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ADataContext(AViewModelLayer viewModelLayer)
        {
            this.viewModelLayer = viewModelLayer;
            redirect = new RelayCommand(ExecuteRedirect);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExecuteRedirect(object? parameter)
        {
            viewModelLayer.Redirect(parameter as Type);
        }
    }
}
