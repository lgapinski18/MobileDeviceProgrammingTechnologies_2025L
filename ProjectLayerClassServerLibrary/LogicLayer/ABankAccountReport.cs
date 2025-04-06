using ProjectLayerClassLibrary.LogicLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.LogicLayer
{
    public abstract class ABankAccountReport
    {
        protected DataLayer.ABankAccountReport dataLayerBankAccountReport;

        internal static ABankAccountReport? CreateBankAccountReport(DataLayer.ABankAccountReport? dataLayerBankAccountReport)
        {
            if (dataLayerBankAccountReport == null)
            {
                return null;
            }
            return new BasicLogicLayerBankAccountReport(dataLayerBankAccountReport);
        }

        public DateTime TimeOfReportCreation { get { return dataLayerBankAccountReport.TimeOfReportCreation; } }

        public float CurrentAccountBalance { get { return dataLayerBankAccountReport.CurrentAccountBalance;  } }

        public float PreviousAccountBalance { get { return dataLayerBankAccountReport.PreviousAccountBalance; } }

        public string OwnerName { get { return dataLayerBankAccountReport.OwnerName; } }

        public string OwnerSurname { get { return dataLayerBankAccountReport.OwnerSurname; } }

        public string OwnerEmail { get { return dataLayerBankAccountReport.OwnerEmail; } }

        public ABankAccountReport(DataLayer.ABankAccountReport dataLayerBankAccountReport)
        {
            this.dataLayerBankAccountReport = dataLayerBankAccountReport;
        }

        public abstract string GetReportContent();
    }
}
