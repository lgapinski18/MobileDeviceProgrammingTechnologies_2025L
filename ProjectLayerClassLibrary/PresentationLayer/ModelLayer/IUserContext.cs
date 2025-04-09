using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer
{
    public interface IUserContext
    {
        int Id { get; }
        string Login { get; }
        string UserName { get; }
        string UserSurname { get; }
        string UserEmail { get; }

        string BankAccountNumberForTransfer { get; internal set; }
        ICollection<string> BankAccountsReports { get; internal set; }
        ICollection<IBankAccount> BankAccounts { get; }

    }
}
