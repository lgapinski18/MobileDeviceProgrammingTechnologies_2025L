using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts
{
    public interface ICreateTransferDataContext : IDataContext
    {
        string SourceAccountNumber { get; set; }
        string DestinationAccountNumber { get; set; }
        string TransferRecipient { get; set; }
        string TransferTitle { get; set; }
        float TransferAmount { get; set; }

        ICommand MakeTransferCommand { get; }
    }
}
