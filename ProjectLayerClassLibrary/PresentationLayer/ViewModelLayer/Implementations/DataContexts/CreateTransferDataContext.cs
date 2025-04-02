using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Implementations.DataContexts
{
    class CreateTransferDataContext : ADataContext, ICreateTransferDataContext
    {
        #region DATA

        private string sourceAccountNumber;
        private string destinationAccountNumber;
        private string transferRecipient;
        private string transferTitle;
        private float transferAmount;

        public string SourceAccountNumber { get => sourceAccountNumber; set => sourceAccountNumber = value; }
        public string DestinationAccountNumber { get => destinationAccountNumber; set => destinationAccountNumber = value; }
        public string TransferRecipient { get => transferRecipient; set => transferRecipient = value; }
        public string TransferTitle { get => transferTitle; set => transferTitle = value; }
        public float TransferAmount { get => transferAmount; set => transferAmount = value; }

        #endregion

        #region COMMANDS

        private ICommand makeTransferCommand;

        public ICommand MakeTransferCommand => makeTransferCommand;

        #endregion

        public CreateTransferDataContext(AViewModelLayer viewModelLayer) : base(viewModelLayer)
        {
            makeTransferCommand = new RelayCommand(ExecuteMakeTransferCommand);
            sourceAccountNumber = viewModelLayer.ModelLayer.UserContext.BankAccountNumberForTransfer;
        }

        private void ExecuteMakeTransferCommand(object? parameter)
        {
            viewModelLayer.ModelLayer.MakeTransfer(SourceAccountNumber, DestinationAccountNumber, TransferAmount, TransferTitle, parameter as Type);
        }
    }
}
