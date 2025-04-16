using ComunicationApiXmlDto;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class ABankAccountReport
    {
        private DateTime timeOfReportCreation;
        public DateTime TimeOfReportCreation { get { return timeOfReportCreation; } }

        private float currentAccountBalance;
        public float CurrentAccountBalance { get { return currentAccountBalance;  } }

        private float previousAccountBalance;
        public float PreviousAccountBalance { get { return previousAccountBalance; } }

        private string ownerName;
        public string OwnerName { get { return ownerName; } }

        private string ownerSurname;
        public string OwnerSurname { get { return ownerSurname; } }

        private string ownerEmail;
        public string OwnerEmail { get { return ownerEmail; } }

        internal static ABankAccountReport CreateBankAccountReportFromXml(BankAccountReportDto bankAccountReportDto)
        {
            ABankAccountReport bankAccountReport = new BankAccountReportWithOwnerData(bankAccountReportDto.PreviousAccountBalance, bankAccountReportDto.CurrentAccountBalance, bankAccountReportDto.OwnerName, bankAccountReportDto.OwnerSurname, bankAccountReportDto.OwnerEmail);
            bankAccountReport.timeOfReportCreation = bankAccountReportDto.TimeOfReportCreation;
            return bankAccountReport;
        }

        public ABankAccountReport(float previousAccountBalance, float currentAccountBalance, string ownerName, string ownerSurname, string ownerEmail)
        {
            this.previousAccountBalance = previousAccountBalance;
            this.currentAccountBalance = currentAccountBalance;
            timeOfReportCreation = DateTime.UtcNow;
            this.ownerName = ownerName;
            this.ownerSurname = ownerSurname;
            this.ownerEmail = ownerEmail;
        }

        public abstract string GetReportContent();
    }
}
