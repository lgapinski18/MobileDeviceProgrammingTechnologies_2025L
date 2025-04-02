using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Implementations.DataContexts
{
    internal class RegisterDataContext : ADataContext, IRegisterDataContext
    {
        private string name = "";
        private string surname = "";
        private string email = "";
        private string mothersMaidenName = "";
        private string password = "";
        private string repeatPassword = "";
        private ICommand registerCommand;
        private Popup registerFailurePopup;

        public string Name { get => name; set => name = value; }

        public string Surname { get => surname; set => surname = value; }

        public string Email { get => email; set => email = value; }

        public string MothersMaidenSurname {  get => mothersMaidenName; set => mothersMaidenName = value; }

        public ICommand RegisterCommand => registerCommand;

        public string Password { get => password; set => password = value; }
        public string RepeatPassword { get => repeatPassword; set => repeatPassword = value; }

        public RegisterDataContext(AViewModelLayer viewModelLayer, Popup registerFailurePopup) : base(viewModelLayer)
        {
            this.registerFailurePopup = registerFailurePopup;
            registerCommand = new RelayCommand(ExecuteRegisterCommand);
        }

        private void ExecuteRegisterCommand(object? parameter)
        {
            viewModelLayer.ModelLayer.Register(Name, Surname, Email, Password, repeatPassword, parameter as Type, registerFailurePopup);
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - trying to register {name} - {surname} - {email} {mothersMaidenName}");
        }
    }
}
