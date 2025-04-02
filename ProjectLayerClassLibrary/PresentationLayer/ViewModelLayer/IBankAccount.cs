using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    public interface IBankAccount
    {
        string AccountNumber { get; }
        float AccountBalance { get; }
    }
}
