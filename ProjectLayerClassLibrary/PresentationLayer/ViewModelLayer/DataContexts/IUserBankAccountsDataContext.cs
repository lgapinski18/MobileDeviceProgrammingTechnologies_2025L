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

        ICommand LogoutCommand { get; }
        ICommand OpenNewBankAccountCommand { get; }
    }
}
