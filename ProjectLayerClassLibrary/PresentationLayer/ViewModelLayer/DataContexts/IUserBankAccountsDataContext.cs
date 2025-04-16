using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        bool IsEuroFiltered { get; set; }
        bool IsUsdFiltered { get; set; }
        bool IsGbpFiltered { get; set; }
        bool IsChfFiltered { get; set; }
        Visibility IsEuroShowed { get; }
        Visibility IsUsdShowed { get; }
        Visibility IsGbpShowed { get; }
        Visibility IsChfShowed { get; }
        string EuroPurchase { get; }
        string EuroSell { get; }
        string UsdPurchase { get; }
        string UsdSell { get; }
        string GbpPurchase { get; }
        string GbpSell { get; }
        string ChfPurchase { get; }
        string ChfSell { get; }

        ICommand LogoutCommand { get; }
        ICommand OpenNewBankAccountCommand { get; }
    }
}
