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

        public ABankAccountReport()
        {
            timeOfReportCreation = DateTime.UtcNow;
        }

        public abstract string GetRaportContent();
    }
}
