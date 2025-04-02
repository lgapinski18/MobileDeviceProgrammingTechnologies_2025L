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
        string Login { get; }
        string UserName { get; }
        string UserSurname { get; }
        string UserEmail { get; }

        public ICollection<IBankAccount> BankAccounts { get; }

    }
}
