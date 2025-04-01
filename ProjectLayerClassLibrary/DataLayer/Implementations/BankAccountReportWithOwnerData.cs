using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class BankAccountReportWithOwnerData : ABankAccountReport
    {
        public BankAccountReportWithOwnerData(float previousAccountBalance, float currentAccountBalance, string ownerName, string ownerSurname, string ownerEmail) 
            : base(previousAccountBalance, currentAccountBalance, ownerName, ownerSurname, ownerEmail)
        {
        }

        public override string GetReportContent()
        {
            return $"Czas wyg.:{TimeOfReportCreation}; Imię: {OwnerName} Nazwisko: {OwnerSurname} Email: {OwnerEmail}; Poprz. stan konta: {PreviousAccountBalance} Obecny stan konta: {CurrentAccountBalance} Saldo: {CurrentAccountBalance - PreviousAccountBalance}";
        }
    }
}
