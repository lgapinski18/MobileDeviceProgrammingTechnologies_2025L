using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts
{
    public interface IUserBankAccountsDataContext
    {
        string Login { get; }
        string UserName { get; }
        string UserSurname { get; }
        ICollection<IBankAccount> BankAccounts { get; }
        ICollection<string> ReportMessages { get; }
        bool IsEuroShowed { get; set; }
        bool IsUsdShowed { get; set; }
        bool IsGbpShowed { get; set; }
        bool IsChfShowed { get; set; }
        float EuroPurchase { get; }
        float EuroSell { get; }
        float UsdPurchase { get; }
        float UsdSell { get; }
        float GbpPurchase { get; }
        float GbpSell { get; }
        float ChfPurchase { get; }
        float ChfSell { get; }

        ICommand LogoutCommand { get; }
        ICommand OpenNewBankAccountCommand { get; }
    }
}
