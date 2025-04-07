using ProjectLayerClassServerLibrary.DataLayer.Implementations.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.DataLayer.Repositories
{
    public static class RepositoryFactory
    {
        public static IAccountOwnerRepository CreateAccountOwnerRepository()
        {
            return new BasicAccountOwnerRepository();
        }

        public static IBankAccountRepository CreateBankAccountRepository()
        {
            return new BasicBankAccountRepository();
        }
    }
}
