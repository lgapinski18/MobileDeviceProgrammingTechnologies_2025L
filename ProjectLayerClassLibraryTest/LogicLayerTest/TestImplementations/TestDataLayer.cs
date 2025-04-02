using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibraryTest.LogicLayerTest.TestImplementations
{
    internal class TestDataLayer : ADataLayer
    {
        public override AAccountOwner CreateAccountOwner(string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            throw new NotImplementedException();
        }

        public override ABankAccount CreateBankAccount(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(string ownerLogin)
        {
            throw new NotImplementedException();
        }

        public override ICollection<AAccountOwner> GetAllAccountOwners()
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetAllBankAccounts()
        {
            throw new NotImplementedException();
        }

        public override ABankAccount? GetBankAccount(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetBankAccounts(int ownerId)
        {
            throw new NotImplementedException();
        }
    }
}
