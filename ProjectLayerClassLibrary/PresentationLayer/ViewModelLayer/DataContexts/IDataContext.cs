using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts
{
    public interface IDataContext : INotifyPropertyChanged
    {
        ICommand Redirect { get; }
    }
}
