using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts
{
    public interface IRegisterDataContext : IDataContext
    {
        string Name { get; set; }
        string Surname { get; set; }
        string Email { get; set; }
        string MothersMaidenSurname { get; set; }
        string Password { get; set; }
        string RepeatPassword { get; set; }
        ICommand RegisterCommand { get; }
    }
}
