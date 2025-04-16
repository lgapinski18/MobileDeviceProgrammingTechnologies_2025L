using ComunicationApiXmlDto;
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
            return $"Czas wyg.:{TimeOfReportCreation};\nImię: {OwnerName} Nazwisko: {OwnerSurname}\nEmail: {OwnerEmail};\nPoprz. stan konta: {PreviousAccountBalance}\nObecny stan konta: {CurrentAccountBalance}\nSaldo: {CurrentAccountBalance - PreviousAccountBalance}";
        }
    }
}
