using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public ICommand ShowMessageCommand { get; }

        public MainViewModel()
        {
            ShowMessageCommand = new RelayCommand(ShowMessage);
        }

        private void ShowMessage(object obj)
        {
            System.Windows.MessageBox.Show($"Hello, {Name}!");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}