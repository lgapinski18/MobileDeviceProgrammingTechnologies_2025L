using ProjectLayerClassLibrary.PresentationLayer.ViewLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Register
{
    internal class RegisterDataContext : ADataContext, IRegisterDataContext
    {
        private string name = "";
        private string surname = "";
        private string email = "";
        private string mothersMaidenName = "";
        private ICommand registerCommand;

        public string Name { get => name; set => name = value; }

        public string Surname { get => surname; set => surname = value; }

        public string Email { get => email; set => email = value; }

        public string MothersMaidenSurname {  get => mothersMaidenName; set => mothersMaidenName = value; }

        public ICommand RegisterCommand => registerCommand;

        public RegisterDataContext(AViewModelLayer viewModelLayer) : base(viewModelLayer)
        {
            registerCommand = new RelayCommand(ExecuteRegisterCommand);
        }

        private void ExecuteRegisterCommand(object? parameter)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - trying to register {name} - {surname} - {email} {mothersMaidenName}");
        }
    }
}
