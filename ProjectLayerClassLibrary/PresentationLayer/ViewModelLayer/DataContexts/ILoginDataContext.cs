using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    public interface ILoginDataContext : IDataContext
    {
        string Login { get; set; }
        string Password { get; set; }
        ICommand LoginCommand { get; }
    }
}
