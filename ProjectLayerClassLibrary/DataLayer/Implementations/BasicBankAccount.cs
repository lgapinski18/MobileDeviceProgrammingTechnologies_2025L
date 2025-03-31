using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibrary")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class BasicBankAccount : ABankAccount
    {
        public BasicBankAccount(string accountNumber, AAccountOwner accountOwner) : base(accountNumber, accountOwner)
        {
        }

        public override void DecreaseAccountBalance(float amount)
        {
            throw new NotImplementedException();
        }

        public override ABankAccountReport GenerateBankAccountReport()
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccountReport> GetBankAccountReports()
        {
            throw new NotImplementedException();
        }

        public override void IncreaseAccountBalance(float amount)
        {
            throw new NotImplementedException();
        }
    }
}
